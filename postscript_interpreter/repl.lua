-- repl.lua - Fixed version
local PostScript = require("ps_interpreter")

local ps = PostScript.new()

print("\n" .. string.rep("=", 60))
print("PostScript Interpreter REPL")
print("Type 'quit' to exit")
print("Type 'stack' to see stack")
print("Type 'reset' to clear stack and dictionaries")
print("Type 'scope dynamic' or 'scope lexical' to change scoping")
print(string.rep("=", 60) .. "\n")

while true do
    io.write("ps> ")
    local input = io.read()
    
    if not input or input == "quit" then
        print("Goodbye!")
        break
    elseif input == "stack" then
        print("Stack: " .. ps:show_stack())
    elseif input == "dicts" then
        ps:show_dict_stack()
    elseif input == "reset" then
        ps = PostScript.new()  -- Create fresh interpreter
        print("Interpreter reset (stack and dictionaries cleared)")
    elseif input == "clear" then
        ps:eval("clear")  -- Use PostScript clear operator
        print("Stack cleared")
    elseif input == "scope dynamic" then
        ps:set_scoping("dynamic")
        print("Scoping set to DYNAMIC")
    elseif input == "scope lexical" then
        ps:set_scoping("lexical")
        print("Scoping set to LEXICAL")
    elseif input == "help" then
        print("Commands: stack, dicts, clear, reset, scope dynamic, scope lexical, quit")
        print("PostScript: numbers, operators, /names, {procedures}")
    else
        local success, err = pcall(function()
            ps:eval(input)
            print("Stack: " .. ps:show_stack())
        end)
        
        if not success then
            print("Error: " .. err)
        end
    end
end