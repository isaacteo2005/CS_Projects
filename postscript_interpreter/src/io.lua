-- src/io.lua
-- Input/Output operations for PostScript

local IO = {}

function IO.new(interpreter)
    return setmetatable({ interpreter = interpreter }, { __index = IO })
end

-- print: writes string to stdout (no newline)
function IO:print()
    local value = self.interpreter:pop()
    
    if type(value) == "string" then
        io.write(value)
    else
        io.write(tostring(value))
    end
end

-- = : prints object with newline (text representation)
function IO:equals()
    local value = self.interpreter:pop()
    io.write(tostring(value) .. "\n")
end

-- == : prints object without newline (PostScript representation)
function IO:equals_equals()
    local value = self.interpreter:pop()
    
    if type(value) == "string" then
        -- PostScript string representation: (hello)
        io.write("(" .. value .. ")")
    elseif type(value) == "boolean" then
        io.write(value and "true" or "false")
    elseif type(value) == "number" then
        io.write(tostring(value))
    elseif type(value) == "table" and value.__type == "dict" then
        io.write("--dict--")
    elseif type(value) == "table" and value.type == "procedure" then
        io.write("--proc--")
    else
        io.write(tostring(value))
    end
end

return IO