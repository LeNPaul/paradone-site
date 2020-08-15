function main ()
   local myfile = io.open("mymain.txt", "w")
   local data = "Test new.\n"
   myfile:write (data)
   myfile:close ()
end

function test ()
   local myfile = io.open("mytest.txt", "w")
   local data = "Test new.\n"
   myfile:write (data)
   myfile:close ()
end
