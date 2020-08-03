-----------------------------------------------------------------------------
-- Copyright (C) 1997-2013 iNTERFACEWARE Inc.  All Rights Reserved
--
-- Module: help lua extension
--
-- Description:
--
-- Used to implement help functionality for iguana translator
--
-- Author: Nasron Cheong
-- Date:   Oct 2011
-----------------------------------------------------------------------------

-- We make local copies of the built-in functions that we need here so that if
-- the user's script overwrites any of these it won't affect the help module.
local pairs = pairs
local type = type
local getmetatable = getmetatable
local io = {
   open = io.open,
   read = io.read,
   close = io.close
}
local pcall = pcall
local json = {
   parse = json.parse
}
local string = {
   format = string.format
}
local ipairs = ipairs
local error = error
local iguana = {
   appDir = iguana.appDir
}

local _func_to_file = {}
local _json_file_cache = {}
local _help_initialized = false
if (not help) then
   help = {}
end 

--[[
Begin private data/functions
--]]

local _base_subdir = '/translator_help'

local _modules = {
   'net',
   'chm',
   'hl7',
   'x12',
   'db',
   'dbs',
   'node',
   'xml',
   'json',
   'cache',
   'util',
   'ack',
   'help',
   'queue',
   'filter',
   'iguana',
   'iconv',
   'crypto',
   --begin lua base libs
   'string',
   'table',
   'math',
   'io',
   'os',
   'debug',
   'coroutine',
   'package'
}

-- we list out the global functions here
-- because we want only lua ones, not including functions 
-- we happen to mix in
local _global_functions = {
   'module',
   'xpcall',
   'rawset',
   'error',
   'tostring',
   'print',
   'pcall',
   'unpack',
   'ipairs',
   'gcinfo',
   'require',
   'getfenv',
   'setfenv',
   'rawget',
   'type',
   'setmetatable',
   'next',
   'select',
   'assert',
   'pairs',
   'tonumber',
   'loadstring',
   'newproxy',
   'rawequal',
   'dofile',
   'collectgarbage',
   'load',
   'getmetatable',
   'loadfile',
   -- Local Additions
   'unwind_protect',
   'trace'
}

local function _addentry(the_function, function_name, base_path, base_name, parent_name, opt_help_data, output)
   -- The user's script could overwrite the built-in global functions with nil,
   -- so we don't do anything here if that's the case.
   if not the_function then
      return
   end
   
   local this_func_path = function_name
   if (parent_name) then this_func_path = parent_name..'.'..function_name end   
   local this_entry = 
   {func=the_function;
      file_location=base_path,
      file=base_path..'/'..base_name..'.json';
      func_path=this_func_path;
      name=function_name;
      help_data = opt_help_data}
   --merge data if it exists
   if output[the_function] then
      for k,v in pairs(this_entry) do output[the_function][k] = v end
   else
      output[the_function] = this_entry
   end
end

local function _makefilelist( base, base_name, base_path, parent_name, output)
   --prevent inspection of the map,
   --and to prevent infinite recursion
   if output[base] or base == package.loaded then return end  
   
   -- If the directory of JSON help files doesn't exist, abort.
   if not os.fs.access(base_path) then
      return
   end
   
   if type(base) == 'table' then
      for i,v in pairs(base) do
         if type(v) == 'function' then
            _addentry(v,i,base_path,base_name,parent_name,nil,output)
         elseif type(v) == 'table' and i ~= '__index' then
            _makefilelist(v,i,base_path..'/'..base_name,parent_name..'.'..i, output)
         end      
      end
      
      --add help for any metatables found in _submeta
	  local base_meta = getmetatable(base)
	  if base_meta and base_meta._submeta and type(base_meta._submeta)=='table' then
	     for k,v in pairs(base_meta._submeta) do      
	        if v.__index then
	           _makefilelist(v.__index, base_name .. '/' .. k, base_path, parent_name, output)
	        end
	     end
	  end
   end
end

-- consider it an error if the Usage entry doesn't exist
-- or parameters+Parameter table are not provided 
local function _verify_help_data(entry)
   if not (entry.Usage or (entry.Parameters and entry.ParameterTable)) then
       return {['Error']='<pre>\'Usage\' or (\'Parameters\' and \'ParameterTable=true\') must be defined in help data.</pre>'},false
   end
   return entry,true
end

local function _json_help(input_func)
   local ent = _func_to_file[input_func]
   if ent then
      local output = nil
      if (ent.help_data) then
         output = _verify_help_data(ent.help_data)
      else
         -- check cache first
         local parsed_help = _json_file_cache[ent.file]
         if (not parsed_help) then
            --if we can't open file, we just ignore
            local help_file,err = io.open(ent.file,'r+')      
            if (help_file) then
               local file_contents = help_file:read('*a')
               local ok,res = pcall(json.parse,{data=file_contents})
               if (ok) then 
                  parsed_help = res
               else
                  output = {['Error']='<pre>'..res..'</pre>'}
               end
            end
         end   

         --note that we don't update the 
         --cache unless we find an entry
         if (parsed_help) then
            output = parsed_help[ent.name]
            if type(output) == 'string' then
               -- reference to another function (aka, an alias).
               output = parsed_help[output]
            end
            if (output) then
               local ok = nil
               output,ok = _verify_help_data(output)
               if (ok) then
                  _json_file_cache[ent.file] = parsed_help
                  ent.help_data = output
               end
            end
         end
      end
      return output
   end
   return nil
end

local function _html_render_json(parsed)
   local Out = ''
   
   if (parsed.Error) then
      Out = Out..'<pre>'..parsed.Error..'</pre>'
   else
      if (parsed.Title) then
         Out = Out..string.format([[<h1>%s</h1>]],parsed.Title)
      end
	  
      if (parsed.Usage) then
         Out = Out..'<h2>Usage:</h2> <div class="codeExample"><pre class="prettyprint">'..parsed.Usage..'</pre></div>'
      end
	  
      if (parsed.Desc) then
         Out = Out..string.format([[<p>%s</p>]],parsed.Desc)
      end
	  
      if (parsed.Parameters) then
	     Out = Out .. '<h2>Parameters:</h2>'
		 
         if (#parsed.Parameters > 0) then
            Out = Out..[[<table class="Basic"><tbody>
			<tr><th>Name</th><th>Description</th></tr>]]
			
            for ParamIndex = 1,#parsed.Parameters do
               local Param = parsed.Parameters[ParamIndex]
               for key,val in pairs(Param) do
                  local key_out
                  if (val.Opt) then
				     key_out = '<i>'..key..'</i>'
                  else
				     key_out = '<b>'..key..'</b>'
				  end
                  Out = Out..'<tr><td>'..key_out..'</td><td>'
				  
                  if (val.Opt) then 
                      Out = Out..'<i>optional</i>: '
                  end
					if (val.Desc) then
						Out = Out..val.Desc
					else 
						error('The parameter takes an object with a "Desc" property.  Please run help.example() to examine a valid help data table.')
					end
					Out = Out..'</td></tr>'
               end
            end
			
            Out = Out..'</tbody></table>'
         else         
            Out = Out..'<p>none.</p>'
         end
      end
	  
      if (parsed.Returns) then
	     Out = Out .. '<h2>Returns:</h2>'
		 
         if type(parsed.Returns) == 'string' then
            Out = Out..'<p>'..parsed.Returns..'</p>'
         elseif #parsed.Returns == 0 then
            Out = Out .. '<p>nothing.</p>'
         else  -- it must be an object
            Out = Out..[[<table class="Basic"><tbody>
            <tr><th>Number</th><th>Description</th></tr>]]
			
            for i,ReturnValue in ipairs(parsed.Returns) do
				if (ReturnValue.Desc) then
					Out = Out .. '<tr><td>' .. i .. '</td><td>'
						.. ReturnValue.Desc .. '</td></tr>'
				else
					error('If passing a table to "Returns", then it requires a Desc field. Please run help.example() to examine a valid help data table.') 
				end
            end
			
            Out = Out .. '</tbody></table>'
         end
      end
	  
      if (parsed.Examples and #parsed.Examples > 0) then
         Out = Out..'<h2>Examples:</h2>'
		 
         for Index = 1,#parsed.Examples do
            local Ex = parsed.Examples[Index]
            Out = Out..'<div class="codeExample"><pre class="prettyprint">'..Ex..'</pre></div>'
         end
      end
	  
      if (parsed.SeeAlso and #parsed.SeeAlso > 0) then
         Out = Out..'<h2>See Also:</h2><ol>'
		 
         for SeeIndex=1,#parsed.SeeAlso do
            local SeeAlso = parsed.SeeAlso[SeeIndex];
            Out = Out..string.format('<li><a target="_blank" href="%s">%s</a></li>\n',SeeAlso.Link,SeeAlso.Title)
         end
		 
         Out = Out..'</ol>'
      end
   end
   
   return Out
end

local function lazyInit()
   if (not _help_initialized) then
      help.reset()
   end   
end

--[[
Begin public functions
--]]
help.set = function(parameters)
   lazyInit()  
   if (type(parameters) ~= 'table') then
      error('First parameter should be a table', 2)      
   end
   local input_function = parameters['input_function']
   if (type(input_function) ~= 'function') then
      error('Expected a function as the \'input_function\' parameter', 2)
   end
   local help_data = parameters['help_data']
   if (type(help_data) ~= 'table') then
      error('Expected a table as the \'help_data\' parameter', 2)
   end
   -- update if exists, if not, add
   if (_func_to_file[input_function]) then
      _func_to_file[input_function].help_data = help_data
   else
      _addentry(input_function, '', '', '', '', help_data, _func_to_file)
   end
end

help.reset = function()
   _json_file_cache = {}
   _func_to_file = {}
   local base_path = iguana.appDir().._base_subdir;
   for i,v in pairs(_modules) do
      _makefilelist( _G[v], v, base_path, v, _func_to_file)
   end
   for i,v in pairs(_global_functions) do
      _addentry(_G[v],v,base_path,'global',nil,nil,_func_to_file)
   end

   _help_initialized = true
end

-- Also see NODLhelp.cpp
help.toHtml = function(help_data)
   lazyInit()
   if (type(help_data) == "table") then
      local verified_help_data = _verify_help_data(help_data)
      return _html_render_json(verified_help_data)
   end
   return ""
end

help.get = function(input_func)
   lazyInit()  
   local ent = _func_to_file[input_func]
   if (ent) then
      return _json_help(input_func)
   end
   return nil
end

help.example = function()
   local out = {
       ["Title"]   ="Typically the full path to the function, e.g. 'string.gsub'";
       ["Usage"]   ="A declaration of the function with the expected parameters, e.g. 'string.gsub (s, pattern, repl [, n])'";
       ["Desc"]    ="A description of the function and what it does. e.g. 'Replaces substrings in a string";
       ["Returns"] = {
          {["Desc"]="What the function returns, e.g. 'string'"},
          {["Desc"]="A second return value, e.g. 'number'"}
       };
       ["ParameterTable"] = "Set this to true or false depending on how the function takes parameters.",
       ["Parameters"] = {
           {["parameter_name_1"]= {["Desc"]="Description of the first parameter"}},
           {["parameter_name_2"]= {["Desc"]="Description of the optional second parameter"; ["Opt"]=true}},
       };
       ["Examples"]={
           "An example invocation, e.g. x = string.gsub(\"hello world\", \"(%w+)\", \"%1 %1\")"     
       };
       ["SeeAlso"]={
           {
               Title="Title of a webpage for more information",
               Link="http://mydomain/my_extra_help.html"
           }
       }
   }
   return out   
end

