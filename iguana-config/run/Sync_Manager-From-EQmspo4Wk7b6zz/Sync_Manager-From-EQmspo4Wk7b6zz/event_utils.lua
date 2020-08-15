local UpdateDb     = store.connect(iguana.project.guid() .. '_updates.db')
local MarkerString = "Channel configuration modified: "
local DateFormat   = '%Y-%m-%d %X'
--
-------------------------------
-- Private helpers functions --
-------------------------------
local function splitMsgId(MsgId)
   local Parts = MsgId:split('-')
   return {tonumber(Parts[1], 10), tonumber(Parts[2], 10)}   
end

local function msgIsNewer(MsgId, RefMsgId)
   if not RefMsgId then 
      return true 
   end
	local Parts    = splitMsgId(MsgId)
   local RefParts = splitMsgId(RefMsgId)
   if Parts[1] == RefParts[1] then
      return Parts[2] > RefParts[2]
   end
	return Parts[1] > RefParts[1]
end

--
-- Inspect the individual channel update event and sync if this channel is
-- new or requires an update.
--
local function inspectChannelUpdateAndMarkForSyncIfRequired(UpdateEvent, Servers, ChannelsToSync)
   local ChannelName = UpdateEvent.data:S():gsub(MarkerString, '')

   trace(Servers.this.exportables)
   if not Servers.this.exportables[ChannelName] then
      trace("Channel " .. ChannelName .. " is not in the exportables list.")
      return
   end

   local MsgId        = UpdateEvent.message_id:S()
   local RecentMsgId  = UpdateDb:get(ChannelName)
   if not msgIsNewer(MsgId, RecentMsgId) then 
      return
   end

   local LogString = "Detected config update for channel: " 
     .. ChannelName .. " at message ID: " .. MsgId .. "."
   
   if RecentMsgId then 
      LogString = LogString .. " Previous sync was at message ID:"
      .. RecentMsgId .. "."
   end
   
   Logstring = LogString .. " Beginning sync."
   iguana.logInfo(LogString)
	ChannelsToSync[ChannelName] = MsgId
   iguana.logDebug(json.serialize{data=ChannelsToSync})
   trace(ChannelsToSync)
end
--
-- Helper for inspectChannelUpdateAndSyncIfRequired(). Only inspect 'message' types.
--
local function inspectChannelUpdates(Events, Servers)
   local ChannelsToSync = {}
   for i=1, Events.export:childCount() do
      if Events.export[i]:nodeName() == 'message' then
         inspectChannelUpdateAndMarkForSyncIfRequired(Events.export[i], Servers, ChannelsToSync)
      end
   end
   local ChannelNames = {}
   for ChannelName, MsgId in pairs(ChannelsToSync) do 
      table.insert(ChannelNames, ChannelName)
   end
   if #ChannelNames > 0 then
      syncChannels(ChannelNames, Servers)
   end
   for i=1, #ChannelsToSync do 
      local ChannelName = ChannelsToSync[i]
   end
   for ChannelName, MsgId in pairs(ChannelsToSync) do 
      iguana.logInfo("Sync of " .. ChannelName .. " for message ID: " .. MsgId .. " completed.")
      UpdateDb:put(ChannelName, MsgId)   
   end
end
--
-- When a channel is modified it will be logged. This log message is what
-- we hook into to know when to tell the other Iguanas to pull in changes.
-- This works for new channels and updated channels.
--
local function fetchChannelUpdatesAndInspect(Servers)
   --UpdateDb:reset() -- Reset DB for testing.
   local LocalServer = Servers.this
   local Now = os.ts.time()
   
   local ABitBeforeNow = Now - conf.poll_time - conf.poll_fuzz
   local Nowish = os.ts.date('%Y/%m/%d %X', ABitBeforeNow)

   local Logs = LocalServer:apiCall(net.http.get, {
      url  = LocalServer.url .. 'api_query',
      parameters={
         type    = 'message',
         reverse = 'true',
         filter  = MarkerString,
         after   = Nowish,
      },
      live  = true,
      cache_time = conf.cache_time,
   })

   local UpdateEvents = xml.parse{data=Logs}
   inspectChannelUpdates(UpdateEvents, Servers)
end
--
-----------------------------
-- Public module functions --
-----------------------------
--
function synchronizeIfRequired(Servers)
   prepareChannels(Servers.this, Servers.remotes)
   fetchChannelUpdatesAndInspect(Servers)
   
end

-- Call this to force sync of all channels.
function synchronizeAll(Servers)
   prepareChannels(Servers.this)

   local ChannelNames = {}
   for Name,_ in pairs(Servers.this.channel_configs) do
      if Servers.this.exportables[Name] then
         table.insert(ChannelNames, Name)
      end
   end
   syncChannels(ChannelNames, Servers)
end

