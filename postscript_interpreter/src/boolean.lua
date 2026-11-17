-- src/boolean.lua
-- Boolean and comparison operations

local Boolean = {}

function Boolean.new(interpreter)
    return setmetatable({ interpreter = interpreter }, { __index = Boolean }) -- __index is metatable lookup for method calls
end

-- Helper to pop two values
function Boolean:pop_two()
    local b = self.interpreter:pop()
    local a = self.interpreter:pop()
    return a, b
end

-- Push boolean values
function Boolean:true_op()
    self.interpreter:push(true)
end

function Boolean:false_op()
    self.interpreter:push(false)
end

-- Logical operators
function Boolean:and_op()
    local a, b = self:pop_two()
    -- PostScript: returns true only if both are true
    self.interpreter:push(a and b)
end

function Boolean:or_op()
    local a, b = self:pop_two()
    -- PostScript: returns true if at least one is true
    self.interpreter:push(a or b)
end

function Boolean:not_op()
    local a = self.interpreter:pop()
    self.interpreter:push(not a)
end

-- Comparison operators
function Boolean:eq()
    local a, b = self:pop_two()
    self.interpreter:push(a == b)
end

function Boolean:ne()
    local a, b = self:pop_two()
    self.interpreter:push(a ~= b)
end

function Boolean:gt()
    local a, b = self:pop_two()
    self.interpreter:push(a > b)
end

function Boolean:lt()
    local a, b = self:pop_two()
    self.interpreter:push(a < b)
end

function Boolean:ge()
    local a, b = self:pop_two()
    self.interpreter:push(a >= b)
end

function Boolean:le()
    local a, b = self:pop_two()
    self.interpreter:push(a <= b)
end

return Boolean