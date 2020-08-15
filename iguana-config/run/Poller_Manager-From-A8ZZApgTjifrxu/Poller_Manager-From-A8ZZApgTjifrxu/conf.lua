-- Primary
local server1 = "http://<PRIMARY IGUANA HOSTNAME>:6543/"
-- Failover
local server2 = "http://<FAILOVER IGUANA HOSTNAME>:6543/"

local poller_manager = {
   weights = {
      [server1] = 100,
      [server2] = 75,
   },
   -- Probably don't need to touch.
   ignored_channels = {
      "Sync Manager",
   },
   channel_name = "Poller Manager",
   -- Don't touch
   remotes_count = 0,
}

-- Count the remotes. This is required so that Poller Manager
-- will re-initialize if it needs to.
for k,v in pairs(poller_manager.weights) do
   poller_manager.remotes_count = poller_manager.remotes_count + 1
end
-- Remove the count for this server. Just interested in remotes.
poller_manager.remotes_count = poller_manager.remotes_count - 1

return poller_manager