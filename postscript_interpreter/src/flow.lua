-- src/flow.lua
-- Flow control operations (if, ifelse, for, repeat, quit, exec)

local Flow = {}

function Flow.new(interpreter)
    return setmetatable({ interpreter = interpreter }, { __index = Flow })
end

-- Execute a procedure (push its contents onto execution stack)
function Flow:exec()
    local proc = self.interpreter:pop()
    
    if type(proc) ~= "table" or proc.type ~= "procedure" then
        error("Not a procedure")
    end
    
      -- Create execution frame
    local frame = {
        proc = proc,
        saved_dict_stack = self.interpreter.dict_stack,
        saved_current_procedure = self.interpreter.current_procedure,
    }
    
    -- Set current procedure
    self.interpreter.current_procedure = proc
    
    -- Apply lexical scoping environment if needed
    if self.interpreter.scoping == "lexical" and proc.env then
        self.interpreter.dict_stack = proc.env
    end
    
    -- Push frame marker for restoration
    table.insert(self.interpreter.exec_stack, {type = "frame_end", frame = frame})
    
    -- Push procedure contents (reverse order)
    for i = #proc.value, 1, -1 do
        table.insert(self.interpreter.exec_stack, proc.value[i])
    end
end

-- 'if' operator: bool proc if
-- If bool is true, execute proc
function Flow:if_op()
    local proc = self.interpreter:pop()
    local condition = self.interpreter:pop()
    
    -- Check types
    if type(proc) ~= "table" or proc.type ~= "procedure" then
        error("'if' expects a procedure")
    end
    
    if type(condition) ~= "boolean" then
        error("'if' expects a boolean condition")
    end
    
    -- Execute procedure if condition is true
    if condition then
        for i = #proc.value, 1, -1 do
            table.insert(self.interpreter.exec_stack, proc.value[i])
        end
    end
end

-- 'ifelse' operator: bool proc1 proc2 ifelse
-- If bool is true, execute proc1; otherwise execute proc2
function Flow:ifelse_op()
    local proc2 = self.interpreter:pop()  -- false branch
    local proc1 = self.interpreter:pop()  -- true branch
    local condition = self.interpreter:pop()
    
    -- Check types
    if type(proc1) ~= "table" or proc1.type ~= "procedure" then
        error("'ifelse' expects a procedure for true branch")
    end
    if type(proc2) ~= "table" or proc2.type ~= "procedure" then
        error("'ifelse' expects a procedure for false branch")
    end
    if type(condition) ~= "boolean" then
        error("'ifelse' expects a boolean condition")
    end
    
    -- Execute appropriate procedure
    local proc_to_execute = condition and proc1 or proc2
    for i = #proc_to_execute.value, 1, -1 do
        table.insert(self.interpreter.exec_stack, proc_to_execute.value[i])
    end
end

-- 'repeat' operator: n proc repeat
-- Execute proc n times
function Flow:repeat_op()
    local proc = self.interpreter:pop()
    local n = self.interpreter:pop()
    
    -- Check types
    if type(proc) ~= "table" or proc.type ~= "procedure" then
        error("'repeat' expects a procedure")
    end
    if type(n) ~= "number" then
        error("'repeat' expects a number")
    end
    
    -- Execute proc n times
    for i = 1, n do
        for j = #proc.value, 1, -1 do
            table.insert(self.interpreter.exec_stack, proc.value[j])
        end
    end
end

-- 'for' operator: start inc limit proc for
-- 'for' operator: start inc limit proc for
function Flow:for_op()
    local proc = self.interpreter:pop()
    local limit = self.interpreter:pop()
    local inc = self.interpreter:pop()
    local start = self.interpreter:pop()
    
    -- Check types
    if type(proc) ~= "table" or proc.type ~= "procedure" then
        error("'for' expects a procedure")
    end
    if type(start) ~= "number" or type(inc) ~= "number" or type(limit) ~= "number" then
        error("'for' expects numeric arguments")
    end
    
    -- Handle direction (inc can be positive or negative)
    local condition
    if inc > 0 then
        condition = function(i) return i <= limit end
    else
        condition = function(i) return i >= limit end
    end
    
    -- Execute loop
    local i = start
    while condition(i) do
        -- Push current loop variable as a NUMBER
        self.interpreter:push(i)  -- Make sure this is a number
        
        -- Push procedure onto execution stack
        for j = #proc.value, 1, -1 do
            table.insert(self.interpreter.exec_stack, proc.value[j])
        end
        
        i = i + inc
    end
end

-- 'quit' operator: Exit interpreter
function Flow:quit()
    os.exit()
end

return Flow