local ChanConf = xml.parse(iguana.channelConfig{name="Sync Manager"})
local PollTime = ChanConf.channel.from_mapper.poll_time:nodeValue() / 1000
-- The Sync Manager polls every PollTime seconds. 
-- At each poll it looks for log activity indicating channel changes.
-- The time range it looks through is between "NOW" and PollFuzz seconds earlier than the previous poll.
local PollFuzz = 600
--
-- Update the global conf objects with syncmanager specific values.
--
conf.poll_time        = PollTime
conf.poll_fuzz        = PollFuzz
conf.cache_time       = 60
conf.regular_timeout  = 15
conf.extended_timeout = 300
conf.main_repo        = 'SyncManager'
conf.do_not_sync      = {
   ["Unified HA Logs"] = true
}
