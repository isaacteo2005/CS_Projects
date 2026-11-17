-- src/dictionary.lua
-- Dictionary operations for variables and scope

local Dictionary = {}

function Dictionary.new(interpreter) -- references to the ps interpreter for stack and dict stack access
    return setmetatable({ interpreter = interpreter }, { __index = Dictionary })
end

-- Create a new dictionary
function Dictionary:dict()
    local capacity = self.interpreter:pop()
    local new_dict = {
        __type = "dict",
        entries = {},
        maxlength = capacity or 10
    }
    self.interpreter:push(new_dict)
end

-- Get dictionary length (number of entries)
function Dictionary:get_length(dict)
    local count = 0
    for _ in pairs(dict.entries) do
        count = count + 1
    end
    return count
end

-- Get dictionary maxlength
function Dictionary:maxlength()
    local dict = self.interpreter:pop()
    if type(dict) ~= "table" or dict.__type ~= "dict" then
        error("Not a dictionary")
    end
    self.interpreter:push(dict.maxlength or 0)
end

-- Define a key-value pair in current dictionary
function Dictionary:def()
    local value = self.interpreter:pop()
    local key_obj = self.interpreter:pop()
    
    
    -- Determine where to store based on scoping mode
    local target_dict
    
    if self.interpreter.scoping == "lexical" and self.interpreter.current_procedure then
        -- Lexical scoping: store in captured environment (captures all outside variables)
        local proc = self.interpreter.current_procedure
        if proc.env and #proc.env > 0 then
            target_dict = proc.env[#proc.env]
        else
            target_dict = self.interpreter.dict_stack[#self.interpreter.dict_stack]
        end
    else
        -- Dynamic scoping: store in current dictionary
        target_dict = self.interpreter.dict_stack[#self.interpreter.dict_stack]
    end
    
    -- Extract key name
    local key
    if type(key_obj) == "table" and key_obj.__type == "name" then
        key = key_obj.name
    else
        key = key_obj
    end
    
    if not target_dict.entries then
        target_dict.entries = {}
    end
    
    target_dict.entries[key] = value
end

-- Begin: push dictionary onto dictionary stack
function Dictionary:begin()
    local dict = self.interpreter:pop()
    if type(dict) ~= "table" or dict.__type ~= "dict" then
        error("Not a dictionary")
    end
    if not dict.entries then
        dict.entries = {}
    end
    table.insert(self.interpreter.dict_stack, dict)
end

-- End: pop dictionary from dictionary stack
function Dictionary:end_dict()
    if #self.interpreter.dict_stack <= 1 then
        error("Cannot pop system dictionary")
    end
    table.remove(self.interpreter.dict_stack)
end

-- Lookup a name in the dictionary stack
function Dictionary:lookup(name)
    for i = #self.interpreter.dict_stack, 1, -1 do
        local dict = self.interpreter.dict_stack[i]
        if dict.entries and dict.entries[name] ~= nil then
            return dict.entries[name]
        end
    end
    return nil
end

-- Create a name object (for /name syntax)
function Dictionary:make_name(name_string)
    return {
        __type = "name",
        name = name_string
    }
end

return Dictionary