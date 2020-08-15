--
-------------------------------
-- Private helpers functions --
-------------------------------
--
-- Filter the list of all this Iguana's channels to 
-- only include ones that may need to be synced.
--
local function removeExplicitlyIgnoredChannels(Server)
   local EligibleForSyncing = {}

   for Name, Config in pairs(Server.channel_configs) do 
      trace(Name, Config, conf.do_not_sync[Name])

      if not conf.do_not_sync[Name] then
         trace(Name .. " is eligible for syncing.")
         EligibleForSyncing[Name] = Config
      else
         trace("Do not sync " .. Name)
      end
   end

   Server.eligible_for_syncing = EligibleForSyncing
end
--
-- Some local channels may not be in exportable state, e.g. have
-- no commits yet. Remove them from consideration.
--
-- "TrulyExportable" channels are channels not ignored in conf.lua
-- as well as technically valid for export as returned by
-- '/sc/view_local_channels' with "for_export=true", i.e. Iguana
-- considers them exportable.
--
local function findExportables(Server, rServer)
   removeExplicitlyIgnoredChannels(Server)
   local ExportableChannelsJson = Server:apiCall(net.http.get, {
         url = Server.url .. 'sc/view_local_channels',
         parameters = {
            ["for_export"] = true
         }, 
         live       = true,
         cache_time = conf.cache_time,
         timeout = conf.extended_timeout,
      })
   -- See function comment.
   local Exportables = json.parse{data=ExportableChannelsJson}
   local TrueExportables = {}

   for i=1, #Exportables.channel_list do
      local EligibleForExport = Exportables.channel_list[i]
      trace(EligibleForExport.name)

      if Server.eligible_for_syncing[EligibleForExport.name] then
         trace(EligibleForExport.name .. " is truly exportable.")
         TrueExportables[EligibleForExport.name] = EligibleForExport
      else
         trace(EligibleForExport.name .. " is NOT truly exportable.")
      end
   end
   --local s = Server:collectChannelConfigs()
   Server.exportables = TrueExportables
   trace(TrueExportables)  
   local Status, Result = pcall(deleteIfRequired, Server, TrueExportables, rServer)
   if not status then
      --if rServer = nil then we have no channels for deletion.
      trace(Result)
   end
end

function deleteIfRequired (Server, thisServer, remoteServers)
   local channelListGUID, channelListReadable = {}, {}
   local rChan = {}
   local rChannelList 
   local iCount = 0
   trace(thisServer)
   for k, v in pairs(remoteServers) do

      rChan = net.http.get{
         url = v.url .. 'sc/view_local_channels',
         parameters = {
            ["for_export"] = true
         }, 
         headers = {['Cookie'] = v.LoginCookie},
         live       = true,
         timeout = conf.extended_timeout,
      }
      rChannelList = json.parse{data=rChan}

      trace(rChannelList.channel_list)
      for i, j in pairs(rChannelList.channel_list) do 
         trace(thisServer[j.name])
         --trace(type(thisServer.channel_configs[j.channel.name:nodeValue()]))
         if type(thisServer[j.name]) ~= 'table'  then
            -- if not table, then channel on remote worker does not exist on source. We must add to channelList.
            channelListGUID['Channel_'.. iCount] =  j.guid
            table.insert(channelListReadable, j.name)
            iCount = iCount + 1
         end
      end
      trace(channelListGUID)
      if next(channelListGUID) ~= nil then
         --If channel list has values, we pass it on to the delete function to remove them from the remote server.
         deleteRemoteChannel(channelListGUID, channelListReadable, v)
      end
   end
   collectgarbage("collect")

end

function deleteRemoteChannel (parameters, channelListReadable, remoteServer)

   parameters['action']  = 'stop'
   parameters['compact'] = true

   -- (1) Stop channel
   local Data, Code, Headers = net.http.post{ 
      url = remoteServer.url .. '/dashboard_action', 
      parameters = parameters,
      headers = {['Cookie'] = remoteServer.LoginCookie},
      live = true, 
      timeout    = conf.extended_timeout,
   }
   if Code == 200 then
      -- (2) Delete Channel
      parameters['action']  = 'delete'
      local Data, Code, Headers = net.http.post{
         url = remoteServer.url .. '/dashboard_action', 
         parameters = parameters,
         headers = {['Cookie'] = remoteServer.LoginCookie},
         live = false,
         timeout    = conf.extended_timeout,
      }

      if Code == 200 then
         for k, v in pairs(channelListReadable) do
            iguana.logDebug('Deleted [' .. v .. '] from [' ..remoteServer.config.iguana_config.web_config.server_label ..'] Server, with Address [' .. remoteServer.url .. '].')
         end
      end

   end
end
--
-- Some channels in the remote repo may not merit consideration for sync.
-- Filter the list to only include ones we have already decided to care about.
-- 
local function findImportables(Server, LocalServer)
   iguana.logDebug('Part 2: import')
   local Args = {
      url        = Server.url .. 'sc/view_remote_channels',
      auth = conf.auth,
      parameters = {
         ["remote_name"] = conf.main_repo
      },
      live       = true,
      cache_time = conf.cache_time,
      timeout    = conf.regular_timeout
   }
   local Status, ImportableChannelsJson = pcall(function() return Server:apiCall(net.http.get, Args) end)
   if not Status then 
      Args.timeout = conf.extended_timeout
      ImportableChannelsJson = Server:apiCall(net.http.post, Args)
   end

   -- NOTE: As of 6.1, you must specify the version of channels to import. Since this
   -- instance should be running 6.1, any channels it syncs will be using the key 6_1_0.
   -- If channels are imported from 6_0_0, simplest solution is to re-export the channels
   -- on an Iguana running 6.1 and abandon the 6.0 channels.
   local Importables = json.parse{data=ImportableChannelsJson}.channel_list["6_1_0"]
   local TrueImportables = {}

   for i=1, #Importables do 
      local EligibleForImport = Importables[i]
      trace(EligibleForImport.name)
      trace(LocalServer.eligible_for_syncing[EligibleForImport.name])

      if LocalServer.eligible_for_syncing[EligibleForImport.name] then
         trace(EligibleForImport.name .. " is truly importable.")
         TrueImportables[EligibleForImport.name] = EligibleForImport
      end
   end

   trace(TrueImportables)
   Server.importables = TrueImportables
end
--
-- This is triggered when there are shared file collisions in the channel we're pushing.
-- In this script we assume we want to overwrite.
--
local function approveFileCollisions(AttemptResult)
   local Approvals = AttemptResult

   for File, Details in pairs(AttemptResult) do
      Approvals[File].diff = nil
      Approvals[File].overwrite = true
   end
   trace(Approvals)
   trace(json.serialize{data = Approvals})
   return json.serialize{data = Approvals}
end
--
-- This is triggered when one or more channels already exists in the repository.
-- In this script we assume we want to overwrite.
--
local function approveCollisions(ChannelNames, Summary, Params)
   if Summary == json.NULL then 
      return
   end 
   iguana.logDebug('Collision Summary: \n' .. json.serialize{data=Summary})
   if Summary.channel_collision_confirmations ~= json.NULL then
      local ChanConfirmations = Summary.channel_collision_confirmations
      for i=1, #ChannelNames do 
         local ChannelName = ChannelNames[i]
         if ChanConfirmations[ChannelName] then 
            ChanConfirmations[ChannelName].update_everything = true
         else
            iguana.logDebug("Collision confirmation not required for " .. ChannelName)
         end
      end
      Params.channel_collision_confirmations = json.serialize{data = ChanConfirmations}
   end
   if Summary.overwrite_shared_file_confirmations ~= json.NULL then
      Params.overwrite_shared_file_confirmations = approveFileCollisions(Summary.overwrite_shared_file_confirmations)
   end
end

local function doIxport(Server, Params, Uri)
   local Args = {
      url        = Server.url .. Uri,
      parameters = Params,
      live       = true,
      cache_time = conf.cache_time,
      timeout    = conf.regular_timeout
   }
   iguana.logDebug('Calling [' .. Uri ..'] Api with [' .. Args.timeout .. '] second timeout...')
   local Status, Result = pcall(function() return Server:apiCall(net.http.post, Args) end)
   --iguana.logDebug('Status1:  [' .. Status ..']. ')
   local Status1
   if not Status then 
      Args.timeout = conf.extended_timeout
      iguana.logDebug('Api timed out. changing to [' .. Args.timeout .. '] seconds...') -- remove later
      -- If this fails we want an error to stop the channel, so no pcall
      Status1, Result = pcall(function() return Server:apiCall(net.http.post, Args) end)
      
      if not Status1 then
         iguana.logDebug('Final timeout. throwing error.')
         error()
      end
      
   end
   iguana.logDebug('[' .. Uri .. '] Ixport Result: \n' .. Result)
   return json.parse{data=Result}
end

local function doExport(Server, Params)
   return doIxport(Server, Params, 'export_channels')
end

local function doImport(Server, Params)
   return doIxport(Server, Params, 'import_channels')
end

--
-----------------------------
-- Public module functions --
-----------------------------
--
function prepareChannels(LocalServer, RemoteServer)
   -- Add the Sync Manager to the ignored channels.
   conf.do_not_sync[iguana.channelName()] = true
   findExportables(LocalServer, RemoteServer)
end

--
-- Export from the local Iguana to the shared repo and tell the others to import the exports.
--
function syncChannels(ChannelNames, Servers)
   trace(Servers.this.exportables[ChannelName])
   local Channels = {}
   for i=1, #ChannelNames do 
      table.insert(Channels, Servers.this.exportables[ChannelNames[i]])
   end
   local ChannelString = table.concat(ChannelNames, ', ')
   local Params = {
      ['sc_task']             = 'export_channels',
      ['commit_message']      = 'Export for automated sync of ' .. ChannelString,
      ['remote_name']         = conf.main_repo,
      ['channel_export_list'] = json.serialize{data = Channels},
   }

   local Result = doExport(Servers.this, Params)
   if not Result.success and Result.error then
      -- This is a serious error.
      error(Result.error)
   elseif Result.export_summary.confirmations_required then
      local Summary = Result.export_summary
      approveCollisions(ChannelNames, Summary, Params)
      local RealRes = doExport(Servers.this, Params)
   end

   --
   -- Export is done. Now signal remote Iguanas to import.
   --
   --do return end

   Params.channel_export_list                 = nil
   Params.channel_collision_confirmations     = nil
   Params.overwrite_shared_file_confirmations = nil
   Params.sc_task = 'import_channels'
   Params.commit_message = 'Import for automated sync of ' .. ChannelString
   Params.channels_version = "6_1_0"
   -- Skip local machine.
   for Url, Server in pairs(Servers.remotes) do
      Channels = {}
      findImportables(Server, Servers.this)
      for i=1, #ChannelNames do 
         table.insert(Channels, Server.importables[ChannelNames[i]])
      end
      Params.channel_import_list = json.serialize{data = Channels}
      iguana.logDebug('Import Channels Message: \n' .. json.serialize{data=Params})
      local ImportResult = doImport(Server, Params)

      if not ImportResult.success then
         local Summary = ImportResult.import_summary
         approveCollisions(ChannelNames, Summary, Params)
         local RealImportResult = doImport(Server, Params)
      end
   end
end
