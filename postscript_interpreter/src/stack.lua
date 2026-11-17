-- src/stack.lua
-- Stack manipulation operations

local StackOps = {}

-- Create a new stack operations handler
function StackOps.new(interpreter)
    -- interpreter reference allows stack ops to access the stack
    return setmetatable({ interpreter = interpreter }, { __index = StackOps })
end

-- Pop top value from stack
function StackOps:pop()
    local stack = self.interpreter.stack
    if #stack == 0 then
        error("Stack underflow")
    end
    return table.remove(stack)
end

-- Push value onto stack
function StackOps:push(value)
    table.insert(self.interpreter.stack, value)
end

-- Duplicate top element
function StackOps:dup()
    local top = self:pop()
    self:push(top)
    self:push(top)
end

-- Exchange top two elements
function StackOps:exch()
    local a = self:pop()
    local b = self:pop()
    self:push(a)
    self:push(b)
end

-- Clear entire stack
function StackOps:clear()
    self.interpreter.stack = {}
end

-- Push stack count onto stack
function StackOps:count()
    self:push(#self.interpreter.stack)
end

-- Copy top n elements
function StackOps:copy()
    local n = self:pop()
    local stack = self.interpreter.stack
    
    if n > #stack then
        error("Not enough elements to copy")
    end
    
    -- Copy top n elements
    local elements = {}
    for i = 1, n do
        elements[i] = stack[#stack - n + i]
    end
    
    -- Push copies
    for i = 1, n do
        self:push(elements[i])
    end
end

-- Pop top element (for operators that just discard)
function StackOps:pop_discard()
    self:pop()
end

return StackOps