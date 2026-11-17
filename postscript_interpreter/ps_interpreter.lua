-- ps_interpreter.lua

-- Load modules
local StackOps = require("src.stack")
local Arithmetic = require("src.arithmetic")
local Boolean = require("src.boolean")
local Dictionary = require("src.dictionary")
local StringOps = require("src.string_ops")
local Flow = require("src.flow")
local IO = require("src.io")

local PostScript = {}
PostScript.__index = PostScript

-- Configuration
function PostScript.new(config)
    config = config or {}
    
    local self = {
        stack = {},           -- Operand stack
        exec_stack = {},      -- Execution stack (for procedures)
        dict_stack = {{ entries = {} }},  -- Dictionary stack
        scoping = config.scoping or "dynamic",  -- "dynamic" or "lexical"
        env_stack = {},  -- Stack for lexical scoping environments
        current_procedure = nil,  -- Track current executing procedure
    }
    
    -- Initialize operation modules
    self.stack_ops = StackOps.new(self)
    self.math = Arithmetic.new(self)
    self.boolean = Boolean.new(self)
    self.dictionary = Dictionary.new(self)
    self.string_ops = StringOps.new(self)
    self.flow = Flow.new(self)
     self.io = IO.new(self)
    
    return setmetatable(self, PostScript)
end

-- Convenience methods
function PostScript:push(value)
    self.stack_ops:push(value)
end

function PostScript:pop()
    return self.stack_ops:pop()
end

-- Deep copy a single table
local function deep_copy_table(original)
    if type(original) ~= "table" then
        return original
    end
    
    local copy = {}
    for k, v in pairs(original) do
        if type(v) == "table" then
            copy[k] = deep_copy_table(v)
        else
            copy[k] = v
        end
    end
    return copy
end

-- Deep copy the dictionary stack
local function deep_copy_dict_stack(stack)
    if type(stack) ~= "table" then
        return stack
    end
    
    local copy = {}
    for i = 1, #stack do
        copy[i] = { entries = {} }
        if stack[i] and stack[i].entries then
            for k, v in pairs(stack[i].entries) do
                if type(v) == "table" then
                    copy[i].entries[k] = deep_copy_table(v)
                else
                    copy[i].entries[k] = v
                end
            end
        end
    end
    return copy
end


-- Tokenizer (handles numbers, strings, names, operators, and procedures)
function PostScript:tokenize(code)
    local tokens = {}
    local i = 1
    local len = #code
    
    while i <= len do
        local char = code:sub(i, i)
        
        -- Skip whitespace
        if char:match("%s") then
            i = i + 1
        
        -- Numbers
        elseif char:match("%d") or (char == "-" and code:sub(i+1, i+1):match("%d")) then
            local num = ""
            while i <= len and code:sub(i, i):match("[%d%.%-]") do
                num = num .. code:sub(i, i)
                i = i + 1
            end
            table.insert(tokens, {type = "number", value = tonumber(num)})
        
        -- Name objects (/name)
        elseif char == "/" then
            local name = ""
            i = i + 1
            while i <= len and code:sub(i, i):match("[%a%d_]") do
                name = name .. code:sub(i, i)
                i = i + 1
            end
            table.insert(tokens, {type = "name", value = name})
        
        -- String literals
        elseif char == "(" then
            local str = ""
            i = i + 1
            local depth = 1
            while i <= len and depth > 0 do
                if code:sub(i, i) == "(" then
                    depth = depth + 1
                elseif code:sub(i, i) == ")" then
                    depth = depth - 1
                    if depth == 0 then
                        i = i + 1
                        break
                    end
                end
                str = str .. code:sub(i, i)
                i = i + 1
            end
            table.insert(tokens, {type = "string", value = str})
        
        -- Procedure start
        elseif char == "{" then
            table.insert(tokens, {type = "procedure_start"})
            i = i + 1
        
        -- Procedure end
        elseif char == "}" then
            table.insert(tokens, {type = "procedure_end"})
            i = i + 1
        
        -- = and == operators
        elseif char == "=" then
            if code:sub(i+1, i+1) == "=" then
                table.insert(tokens, {type = "operator", value = "=="})
                i = i + 2
            else
                table.insert(tokens, {type = "operator", value = "="})
                i = i + 1
            end
        
        -- Regular operators and variable names
        else
            local word = ""
            while i <= len and not code:sub(i, i):match("%s") and 
                  code:sub(i, i) ~= "{" and code:sub(i, i) ~= "}" and
                  code:sub(i, i) ~= "=" do
                word = word .. code:sub(i, i)
                i = i + 1
            end
            table.insert(tokens, {type = "operator", value = word})
        end
    end
    
    return tokens
end
-- Parse procedures from tokens
function PostScript:parse(tokens)
    local result = {}
    local i = 1
    
    while i <= #tokens do
        local token = tokens[i]
        
        if token.type == "procedure_start" then
            -- Parse procedure { ... }
            -- CAPTURE CURRENT ENVIRONMENT FOR LEXICAL SCOPING
            local proc = {type = "procedure", value = {}, executable = true, env = self.dict_stack, created_in_scope = #self.dict_stack} -- Stored captured enviroment
            i = i + 1
            local depth = 1
            
            while i <= #tokens and depth > 0 do
                if tokens[i].type == "procedure_start" then
                    depth = depth + 1
                    table.insert(proc.value, tokens[i])
                elseif tokens[i].type == "procedure_end" then
                    depth = depth - 1
                    if depth == 0 then
                        i = i + 1
                        break
                    end
                    table.insert(proc.value, tokens[i])
                else
                    table.insert(proc.value, tokens[i])
                end
                i = i + 1
            end
            table.insert(result, proc)
        
        else
            table.insert(result, token)
            i = i + 1
        end
    end
    
    return result
end

-- Execute a procedure
function PostScript:exec()
    local proc = self:pop()
    
    if type(proc) ~= "table" or proc.type ~= "procedure" then
        error("Not a procedure")
    end
    -- Create a new execution frame for restoration
    local frame = {
        proc = proc,
        saved_dict_stack = self.interpreter.dict_stack,
        saved_current_procedure = self.interpreter.current_procedure,
    }
    
    if not proc.executable then
        error("Cannot execute non-executable object")
    end

     -- Set current procedure for def to use
    self.interpreter.current_procedure = proc

     -- If lexical scoping and procedure has captured environment, restore it
    if self.interpreter.scoping == "lexical" and proc.env then
        self.interpreter.dict_stack = proc.env
    end

    -- Push a marker to restore after procedure completes
    table.insert(self.interpreter.exec_stack, {type = "frame_end", frame = frame})
    
    -- Push procedure contents onto execution stack (reverse order)
    for j = #proc.value, 1, -1 do
        table.insert(self.exec_stack, proc.value[j])
    end
end

-- Look up a variable name (handles dynamic/lexical scoping)
function PostScript:lookup(name)
    if self.scoping == "dynamic" then
        -- Dynamic scoping: search from top of dict stack (top to bottom)
        for i = #self.dict_stack, 1, -1 do
            local dict = self.dict_stack[i]
            if dict.entries and dict.entries[name] ~= nil then
                return dict.entries[name]
            end
        end
    else
        -- Lexical: search in this order:
        -- 1. Current procedure's captured environment (if any)
        -- 2. Then current dict_stack bottom to top
        
        -- Step 1: Check current procedure's captured environment
        if self.current_procedure and self.current_procedure.env then
            -- Search captured environment (top to bottom? bottom to top?)
            for i = #self.current_procedure.env, 1, -1 do
                local dict = self.current_procedure.env[i]
                if dict.entries and dict.entries[name] ~= nil then
                    return dict.entries[name]
                end
            end
        end
        
        -- Step 2: Search current dict_stack bottom to top
        for i = 1, #self.dict_stack do
            local dict = self.dict_stack[i]
            if dict.entries and dict.entries[name] ~= nil then
                return dict.entries[name]
            end
        end
    end
    return nil
end

-- Operator dispatch table
local operators = {
    -- Stack operations
    pop = function(s) s.stack_ops:pop_discard() end,
    dup = function(s) s.stack_ops:dup() end,
    exch = function(s) s.stack_ops:exch() end,
    clear = function(s) s.stack_ops:clear() end,
    count = function(s) s.stack_ops:count() end,
    copy = function(s) s.stack_ops:copy() end,
    
    -- Arithmetic operations
    add = function(s) s.math:add() end,
    sub = function(s) s.math:sub() end,
    mul = function(s) s.math:mul() end,
    div = function(s) s.math:div() end,
    idiv = function(s) s.math:idiv() end,
    mod = function(s) s.math:mod() end,
    abs = function(s) s.math:abs() end,
    neg = function(s) s.math:neg() end,
    ceiling = function(s) s.math:ceiling() end,
    floor = function(s) s.math:floor() end,
    round = function(s) s.math:round() end,
    sqrt = function(s) s.math:sqrt() end,
    
    -- Boolean operations
    ["true"] = function(s) s.boolean:true_op() end,
    ["false"] = function(s) s.boolean:false_op() end,
    ["and"] = function(s) s.boolean:and_op() end,
    ["or"] = function(s) s.boolean:or_op() end,
    ["not"] = function(s) s.boolean:not_op() end,
    
    -- Comparison operations
    eq = function(s) s.boolean:eq() end,
    ne = function(s) s.boolean:ne() end,
    gt = function(s) s.boolean:gt() end,
    lt = function(s) s.boolean:lt() end,
    ge = function(s) s.boolean:ge() end,
    le = function(s) s.boolean:le() end,
    
    -- Dictionary operations
    dict = function(s) s.dictionary:dict() end,
    def = function(s) s.dictionary:def() end,
    begin = function(s) s.dictionary:begin() end,
    ["end"] = function(s) s.dictionary:end_dict() end,
    maxlength = function(s) s.dictionary:maxlength() end,
    
    -- String operations
    get = function(s) s.string_ops:get() end,
    getinterval = function(s) s.string_ops:getinterval() end,
    putinterval = function(s) s.string_ops:putinterval() end,
    -- Type-aware length (handles both strings and dictionaries)
    length = function(s)
    local value = s:pop()
    
    if type(value) == "string" then
        -- String length
        s:push(#value)
    elseif type(value) == "table" and value.__type == "dict" then
        -- Dictionary length - use the helper function
        local count = s.dictionary:get_length(value)
        s:push(count)
    else
        error("Length not defined for this type")
    end
    end,
    
    
    -- Flow control
     exec = function(s) s.flow:exec() end,
    ["if"] = function(s) s.flow:if_op() end,
    ifelse = function(s) s.flow:ifelse_op() end,
    ["repeat"] = function(s) s.flow:repeat_op() end,
    ["for"] = function(s) s.flow:for_op() end,
    quit = function(s) s.flow:quit() end,

    -- I/O operations
    print = function(s) s.io:print() end,
    ["="] = function(s) s.io:equals() end,
    ["=="] = function(s) s.io:equals_equals() end,
}

-- Execute built-in operators
function PostScript:execute(op)
    local func = operators[op]
    if func then
        func(self)
    else
        error("Unknown operator: " .. op)
    end
end

-- Main evaluation loop
function PostScript:eval(code)
    local tokens = self:tokenize(code)
    local parsed = self:parse(tokens)
    
    -- Push parsed tokens onto execution stack
    for i = #parsed, 1, -1 do
        table.insert(self.exec_stack, parsed[i])
    end
    
    -- Process execution stack
    while #self.exec_stack > 0 do
        local token = table.remove(self.exec_stack)
        
        if token.type == "number" then
            self:push(token.value)
        elseif token.type == "string" then
            self:push(token.value)
        elseif token.type == "name" then
            self:push(self.dictionary:make_name(token.value))
        elseif token.type == "procedure" then
            -- Push procedure onto stack
            self:push(token)
        elseif token.type == "frame_end" then
            -- Restore the saved dictionary stack
            self.dict_stack = token.frame.saved_dict_stack
            self.current_procedure = token.frame.saved_current_procedure
        elseif token.type == "operator" then
    -- Check if it's a variable name
    local value = self:lookup(token.value)
    if value ~= nil then
        if type(value) == "table" and value.type == "procedure" then
             -- Save current environment
            local saved_stack = self.dict_stack
            local saved_proc = self.current_procedure
            self.current_procedure = value
            
            -- Switch to captured environment if in lexical mode
            if self.scoping == "lexical" and value.env then
                self.dict_stack = value.env --dict_stack becomes outer enviroment of the function (what did dict_stack look like when the function was defined?)
            end
            
            -- Set current procedure
            self.current_procedure = value
            
            -- Push frame marker to restore later
            table.insert(self.exec_stack, {
                type = "frame_end",
                frame = {
                    saved_dict_stack = saved_stack,
                    saved_current_procedure = saved_proc
                }
            })
            for i = #value.value, 1, -1 do
                table.insert(self.exec_stack, value.value[i])
            end
        else
            self:push(value)
        end
    else
        self:execute(token.value)
    end
end
    end
end 
-- Toggle scoping mode
function PostScript:set_scoping(mode)
    if mode == "dynamic" or mode == "lexical" then
        self.scoping = mode
    else
        error("Scoping must be 'dynamic' or 'lexical'")
    end
end

-- Debug helpers
function PostScript:show_stack()
    local parts = {}
    for i, v in ipairs(self.stack) do
        table.insert(parts, tostring(v))
    end
    return "[" .. table.concat(parts, ", ") .. "]"
end

function PostScript:show_dict_stack()
    print("Dictionary stack (" .. self.scoping .. " scoping):")
    for i, dict in ipairs(self.dict_stack) do
        print(string.format("  [%d]:", i))
        if dict.entries then
            for k, v in pairs(dict.entries) do
                print(string.format("      %s = %s", k, tostring(v)))
            end
        end
    end
end

return PostScript