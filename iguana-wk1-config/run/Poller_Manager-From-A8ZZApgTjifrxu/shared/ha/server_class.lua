require "html_entities"

-- These double check functions are used by apiCall to detect when we need
-- to login and get a session cookie. Some endpoints use different conven-
-- tions so a custom check is added for them.
function DoubleCheckResultDashboardData(Url, Result)
   if Url:sub(-14, -1) ~= 'dashboard_data' then 
      return true 
   end
   
   -- Dashboard data return Code = 200 and body =
   --   '{"ErrorDescription":"You are not logged in.","LoggedIn":false}'
   -- when not logged in.
   local JsonResult = json.parse{data=Result}
   if JsonResult.ErrorDescription and not JsonResult.LoggedIn then
      return false
   end
   
   return true
end

local function DoubleCheckResultApiQuery(Url, Result)
   if Url:sub(-9, -1) ~= 'api_query' then 
      return true 
   end

   local Status, Tree = pcall(xml.parse, {data=Result})
	   if not Status then
	      iguana.logWarning('Calling /api_query did not return the expected response, trying one more time.')
	      return false
	   end
   if Tree.export.success:nodeValue() == 'true' then
      return true
   end
   if Tree.export:childCount('error') > 0 
      and Tree.export:child('error').description:nodeValue() == 'Authentication failed.' then
      return false
   end   
   return true
end

local Server = {}
--
-- This module is a Server class object.
--
-- These server objects make it easy to make API calls. They know their URL,
-- their main configuration, have a list of the channels configurations, as
-- well as their login cookie for making API request with.
--
-- The Server constructor requires a URL. It fetches everything it needs from
-- This URL. The constructor (somewhat unusually) takes a filter function which
-- will cause it to return nil instead of the server object. This is useful for
-- managing a list of servers and came out while working on the Sync Manager
-- and the Poller Manager. If a server is not configured for syncing, it won't
-- bother constructing it.
--
function Server:new(Url, Cookie, filterServerFunc)
   if not Url then error("URL cannot be nil") end

   local ServerObject = {url = Url, LoginCookie = Cookie}
   setmetatable(ServerObject, self)
	self.__index = self

   local GotConfigs, ErrorMessage = ServerObject:getServerConfigs()
   if not GotConfigs then
      ServerObject.is_online = false
      ServerObject.error = ErrorMessage
      return ServerObject
   end

   ServerObject.is_online = true

   -- TODO: This should be in fillServerList. Move it there.
   if filterServerFunc then
      local ShouldFilter = filterServerFunc(Url, ServerObject.config)
      if ShouldFilter then
         return nil, "Server was filtered out."
      end
   end

   return ServerObject, "Success!"
end

function Server:getServerConfigs()
   local Success, ServerConfigXml, Code, Headers 
      = pcall(self.apiCall, self, net.http.get, {
         url   = self.url .. '/get_server_config',
         live  = true,
         cache_time = conf.cache_time,
         auth  = conf.auth,
         timeout    = conf.extended_timeout,
      })
	if not Success then
      return Success, ServerConfigXml -- ServerConfigXml is error message in this case.
   end

   local ServerConfig = xml.parse{data=ServerConfigXml}
   
   self.config = ServerConfig
   local ChannelConfigs = self:collectChannelConfigsForServer()
   self.channel_configs = ChannelConfigs
   
   -- Must return true to indicate we were able to retrieve server configs.
   return true, "Successfully retrieved server configs!"
end


-- Implement whatever consitutes a health check. You could hit an HTTP endpoint that does 
-- complex tasks like checking if a database can perform queries.
function Server:healthCheck()
   trace(self.imconf.peer_wk)
   local Success, Response, Code, Headers = pcall(self.apiCall, self, net.http.get, {
         url   = self.url .. '/status',
         live  = true,
         cache_time = conf.cache_time,
         auth  = conf.auth,
         timeout    = conf.extended_timeout,
      })
   return Success, Code
end


-- If the server was offline when the channel started config and channel_configs will be nil.
-- If it comes online afterwards, they will be fetched.
-- If the server goes offline afterwards they'll be set to false and is_online will be false.
function Server:ping()
	local Success, Code = self:healthCheck()
   
   if Success and Code == 200 then
      trace("Server is online.")
      self.is_online = true
      if not self.config or not self.channel_configs then
         trace("Server which was not online originally now is. Fetching its configs!")
         self:getServerConfigs()
      end
   else
      trace("Server is offline.")
      self.is_online = false
      self.config = false
      self.channel_configs = false
   end
   trace(self.config, self.channel_configs)
   return Success
end

-- Read the cookie to a file in development mode. This allows you to maintain
-- a session while you work in the Translator.
function Server:getLoginCookie()
   if iguana.isTest() then
      local f = io.open(filter.hex.enc(self.url) .. '-logincookie', 'r')
      if f then
         self.LoginCookie = f:read("*a")
         f:close()
      end
   end
   return self.LoginCookie
end

-- Write the cookie to a file in development mode. This allows you to maintain
-- a session while you work in the Translator.
function Server:setLoginCookie(Cookie)
   if iguana.isTest() then
      local f = io.open(filter.hex.enc(self.url) .. '-logincookie', 'w')
      f:write(Cookie)
      f:close()
   end
   self.LoginCookie = Cookie
end

-- Make an API call with this. It will login if it has to and use
-- the login cookie on subsequent API calls.
function Server:apiCall(Func, Params)
   trace(self.url)
   
   local LoginCookie = self:getLoginCookie()
   trace(LoginCookie)

   Params.headers = { Cookie = LoginCookie }
   local Result, Code, Headers = Func(Params)
	trace(Code)

   -- The different URLs return "Not logged in" in different
   -- manners, so if it's not a 200, try logging in and if it
   -- still fails it's back to regular behaviour.

   if Code ~= 200 
      or not DoubleCheckResultApiQuery(Params.url, Result)
      or not DoubleCheckResultDashboardData(Params.url, Result) then
      self:apiLogin()
      local LoginCookie = self:getLoginCookie()
      trace(LoginCookie)
      Params.headers = { Cookie = LoginCookie }
      Result, Code, Headers = Func(Params)
   end
   return Result, Code, Headers
end
help.set{input_function=Server.apiCall, help_data={Usage = "local R, C, H = apiCall(Server, net.http.get, SameTableNetHttpGetWouldTake)",}}

function Server:apiLogin()
   local Status, Code, Headers = net.http.get{
      url  = self.url .. '/login.html',
      auth = conf.auth,
      live = true,
      timeout    = conf.extended_timeout,
   }
   trace(Code)
   trace(Headers)
   
   local TheHeaders = Headers["Set-Cookie"]
   if not TheHeaders then
      error("Could not login. Check credentials. Or maybe it's something else...")
   end
   local LoginCookie = TheHeaders:match('Iguana%-SessionID%-.-;')
   self:setLoginCookie(LoginCookie)
end

--
-- Helper for collectChannelConfigsForServer()
--
function Server:collectChannelConfigs(Group, Collection) 
   for i=1, Group:childCount() do
      local Name = entUnescape(Group[i].channel_name:S())
      local Channel = self:apiCall(net.http.get, {
         url        = self.url .. '/get_channel_config',
         parameters = { name = Name },
         live       = true,
         cache_time = conf.cache_time,
         timeout    = conf.extended_timeout,   
      })

      Collection[Name] = xml.parse{data=Channel}
   end
end
--
-- Fetch the channel configurations for this remote Iguana.
--
function Server:collectChannelConfigsForServer()
   local Channels = {}
   local Grps = self.config.iguana_config.channel_groupings
   local GrpCount = Grps:childCount()

   for i=1, GrpCount do 
      if Grps[i].grouping_name:S() == "All Channels" then
         self:collectChannelConfigs(Grps[i].channels, Channels)
         break
      end
   end

   return Channels
end

--
-- Get XML configuration for remotes configured on this Iguana. This is (partly)
-- how we know which Iguanas to synchronize.
--
function Server:getRemotes()
   local Remotes = {}
   local RemotesConf = self.config.iguana_config.remote_iguana_list

   if not RemotesConf then
      error("No remote Iguanas are configured.")
   end

   local Count = RemotesConf:childCount("remote_iguana")
   for i = 1, Count do 
      Remotes[i] = RemotesConf:child('remote_iguana', i)
   end

   return Remotes
end

--
-- Static functions.
--
-- Assemble a URL from a local Iguana's config. 
--
function Server.makeUrl(WebInfo, ForChannel)
   local Loc = ForChannel 
               and 'https_channel_server' 
               or 'web_config'
   local UrlParts = {}
   UrlParts[1] = 'http'
   if WebInfo[Loc].use_https 
      then UrlParts[1] = UrlParts[1] .. 's'
   end
   UrlParts[2] = '://'
   UrlParts[3] = WebInfo.host
   UrlParts[4] = ':'
   UrlParts[5] = WebInfo[Loc].port
   UrlParts[6] = '/'
   return table.concat(UrlParts)
end
--
-- Assemble a URL for a Remote Iguana.
--
function Server.makeRemoteUrl(Remote) 
   local UrlParts = {}
   UrlParts[1] = 'http'
   if Remote.https:nodeValue() == 'true'
      then UrlParts[1] = UrlParts[1] .. 's'
   end
   UrlParts[2] = '://'
   UrlParts[3] = Remote.host:nodeValue()
   UrlParts[4] = ':'
   UrlParts[5] = Remote.port:nodeValue()
   UrlParts[6] = '/'
   trace(UrlParts)
   return table.concat(UrlParts)
end

function Server.thisServer()
   return server:new(server.makeUrl(iguana.webInfo()))
end

function Server.fillServerList(Servers, filterServerFunc)
   local ExistingCookie = Servers.this and Servers.this.LoginCookie or nil
   Servers.this = Server:new(Server.makeUrl(iguana.webInfo()), ExistingCookie)

   if not Servers.remotes then 
      Servers.remotes = {}
      Servers.remotesCount = 0
   end

   local GoodUrls = {}
   local Remotes = Servers.this:getRemotes()
   for i=1, #Remotes do
      local RemoteUrl = Server.makeRemoteUrl(Remotes[i])
      GoodUrls[RemoteUrl] = true
      local ExistingRemote = Servers.remotes[RemoteUrl]
      trace(ExistingRemote)

      ExistingCookie = ExistingRemote and ExistingRemote.LoginCookie or nil
	   trace(ExistingRemote, ExistingCookie)

      local TheServer = Server:new(RemoteUrl, ExistingCookie, filterServerFunc)

      if TheServer and not ExistingRemote then
         Servers.remotes[RemoteUrl] = TheServer
         Servers.remotesCount = Servers.remotesCount + 1
      elseif not TheServer and ExistingRemote then
         trace("A server that was once used has been filtered out by the filter function.")
         Servers.remotes[RemoteUrl] = nil
         Servers.remotesCount = Servers.remotesCount - 1
      end
   end

   for Url, Server in pairs(Servers.remotes) do 
      if not GoodUrls[Url] then
         trace("A server that was once in the remote list no longer is.")
         Servers.remotes[Url] = nil
         Servers.remotesCount = Servers.remotesCount - 1
      end
   end
   trace(Servers.remotesCount, Servers.remotes)

   return Servers
end

return Server
