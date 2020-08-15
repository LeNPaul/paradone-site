----------------------------------------
-- Iguana Tools Sync Manager 2.0
-- Copyright © 2016 iNTERFACEWARE, Inc.
-- All Rights Reserved
----------------------------------------

require 'html_entities'
require 'net.http.cache'
store = require 'store2'
conf  = require 'ha.conf'
require "conf"
local Server = require 'ha.server_class'
require 'channel_utils'
require 'event_utils'

--Set this value to true to force all channels to be synced on each run
local SYNC_ALL = false

--
-- Filter out servers which do not contain conf.main_repo
-- Returing true means to filter it out.
--
local function filterServerFunc(Url, ServerConfig)
   local Repos = ServerConfig.iguana_config.repositories
   if not Repos then 
      return true
   end
   for i=1, Repos:childCount() do
      if Repos[i].name:S() == conf.main_repo then
         -- We have found the shared repo, so do not filter.
         return false
      end
   end
   return true
end

local Servers = {}

function main()
   iguana.setTimeout(1200)
   trace(Servers)
   Servers = Server.fillServerList(Servers, filterServerFunc)
   if SYNC_ALL then
      synchronizeAll(Servers)
   else
      synchronizeIfRequired(Servers)
   end
end
