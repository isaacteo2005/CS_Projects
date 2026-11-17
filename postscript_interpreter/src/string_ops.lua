-- src/string_ops.lua
-- String operations for PostScript

local StringOps = {}

function StringOps.new(interpreter)
    return setmetatable({ interpreter = interpreter }, { __index = StringOps })
end

-- Get string length
function StringOps:length()
    local dict = self.interpreter:pop()
    if type(dict) ~= "table" or dict.__type ~= "dict" then
        error("Not a dictionary")
    end
    local count = 0
    for _ in pairs(dict.entries) do
        count = count + 1
    end
    self.interpreter:push(count)
end

-- Get character at index (PostScript uses 0-based indexing)
function StringOps:get()
    local index = self.interpreter:pop()
    local str = self.interpreter:pop()
    
    if type(str) ~= "string" then
        error("Not a string")
    end
    if type(index) ~= "number" then
        error("Index must be a number")
    end
    
    -- Convert PostScript's 0-based index to Lua's 1-based
    local lua_index = index + 1
    
    if lua_index < 1 or lua_index > #str then
        error("Index out of range")
    end
    
    -- Return ASCII code (PostScript returns integer code)
    local char = str:sub(lua_index, lua_index)
    self.interpreter:push(string.byte(char))
end

-- Extract substring (PostScript: getinterval)
function StringOps:getinterval()
    local count = self.interpreter:pop()
    local index = self.interpreter:pop()
    local str = self.interpreter:pop()
    
    if type(str) ~= "string" then
        error("Not a string")
    end
    if type(index) ~= "number" then
        error("Index must be a number")
    end
    if type(count) ~= "number" then
        error("Count must be a number")
    end
    
    -- Convert 0-based to 1-based
    local lua_index = index + 1
    
    if lua_index < 1 or lua_index > #str then
        error("Index out of range")
    end
    if count < 0 then
        error("Count cannot be negative")
    end
    if lua_index + count - 1 > #str then
        error("Count exceeds string length")
    end
    
    local result = str:sub(lua_index, lua_index + count - 1)
    self.interpreter:push(result)
end

-- Fixed putinterval (handles variable length replacement)
function StringOps:putinterval()
    local src = self.interpreter:pop()
    local index = self.interpreter:pop()
    local dest = self.interpreter:pop()
    
    if type(dest) ~= "string" then
        error("Destination is not a string")
    end
    if type(src) ~= "string" then
        error("Source is not a string")
    end
    if type(index) ~= "number" then
        error("Index must be a number")
    end
    
    -- Convert to 1-based
    local pos = index + 1
    
    -- Build result: before + src + after
    local before = dest:sub(1, pos - 1)
    local after = dest:sub(pos + #src)  -- Skip the replaced section
    local result = before .. src .. after
    
    self.interpreter:push(result)
end
return StringOps