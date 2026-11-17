-- src/arithmetic.lua
-- Arithmetic operations

local Arithmetic = {}

function Arithmetic.new(interpreter)

      -- When a method is called on 'interpreter', look in 'Arithmetic' table
    return setmetatable({ interpreter = interpreter }, { __index = Arithmetic })
end

-- Helper to get stack access
function Arithmetic:get_stack()
    return self.interpreter.stack
end

-- Helper to pop two values
function Arithmetic:pop_two()
    local b = self.interpreter:pop()
    local a = self.interpreter:pop()
    return a, b
end

-- Basic arithmetic
function Arithmetic:add()
    local a, b = self:pop_two()
    self.interpreter:push(a + b)
end

function Arithmetic:sub()
    local a, b = self:pop_two()
    self.interpreter:push(a - b)
end

function Arithmetic:mul()
    local a, b = self:pop_two()
    self.interpreter:push(a * b)
end


function Arithmetic:div()
    local a, b = self:pop_two()
    if b == 0 then
        error("division by zero")
    end
    self.interpreter:push(a / b)
end

-- Integer division (also needs division by zero check)
function Arithmetic:idiv()
    local a, b = self:pop_two()
    if b == 0 then
        error("division by zero")
    end
    self.interpreter:push(math.floor(a / b))
end

-- Modulo (also needs division by zero check)
function Arithmetic:mod()
    local a, b = self:pop_two()
    if b == 0 then
        error("division by zero")
    end
    self.interpreter:push(a % b)
end

-- Absolute value
function Arithmetic:abs()
    local a = self.interpreter:pop()
    self.interpreter:push(math.abs(a))
end

-- Negate
function Arithmetic:neg()
    local a = self.interpreter:pop()
    self.interpreter:push(-a)
end

-- Ceiling
function Arithmetic:ceiling()
    local a = self.interpreter:pop()
    self.interpreter:push(math.ceil(a))
end

-- Floor
function Arithmetic:floor()
    local a = self.interpreter:pop()
    self.interpreter:push(math.floor(a))
end

-- Round (PostScript rounds to nearest integer)
function Arithmetic:round()
    local a = self.interpreter:pop()
    self.interpreter:push(math.floor(a + 0.5))
end

-- Square root
function Arithmetic:sqrt()
    local a = self.interpreter:pop()
    if a < 0 then
        error("sqrt of negative number")
    end
    self.interpreter:push(math.sqrt(a))
end

return Arithmetic