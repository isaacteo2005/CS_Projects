-- test_stack.lua
-- Comprehensive tests for stack manipulation operations

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

-- Test suite for stack operations
function testPush()
    print("\n📚 Testing: push")
    local ps = PostScript.new()
    ps:push(42)
    luaunit.assertEquals(#ps.stack, 1)
    luaunit.assertEquals(ps.stack[1], 42)
end

function testPushMultiple()
    print("📚 Testing: push multiple values")
    local ps = PostScript.new()
    ps:push(10)
    ps:push(20)
    ps:push(30)
    luaunit.assertEquals(#ps.stack, 3)
    luaunit.assertEquals(ps.stack[1], 10)
    luaunit.assertEquals(ps.stack[2], 20)
    luaunit.assertEquals(ps.stack[3], 30)
end

function testPop()
    print("📚 Testing: pop")
    local ps = PostScript.new()
    ps:push(100)
    local value = ps:pop()
    luaunit.assertEquals(value, 100)
    luaunit.assertEquals(#ps.stack, 0)
end

function testPopOrder()
    print("📚 Testing: pop order (LIFO)")
    local ps = PostScript.new()
    ps:push(1)
    ps:push(2)
    ps:push(3)
    luaunit.assertEquals(ps:pop(), 3)
    luaunit.assertEquals(ps:pop(), 2)
    luaunit.assertEquals(ps:pop(), 1)
end

function testPopEmpty()
    print("📚 Testing: pop from empty stack (error)")
    local ps = PostScript.new()
    luaunit.assertErrorMsgContains("Stack underflow", 
        function() ps:pop() end)
end

function testDup()
    print("📚 Testing: dup")
    local ps = PostScript.new()
    ps:eval("5 dup")
    luaunit.assertEquals(#ps.stack, 2)
    luaunit.assertEquals(ps.stack[1], 5)
    luaunit.assertEquals(ps.stack[2], 5)
end

function testDupMultiple()
    print("📚 Testing: dup on larger stack")
    local ps = PostScript.new()
    ps:eval("1 2 3 dup")
    luaunit.assertEquals(#ps.stack, 4)
    luaunit.assertEquals(ps.stack[1], 1)
    luaunit.assertEquals(ps.stack[2], 2)
    luaunit.assertEquals(ps.stack[3], 3)
    luaunit.assertEquals(ps.stack[4], 3)
end

function testDupEmpty()
    print("📚 Testing: dup on empty stack (error)")
    local ps = PostScript.new()
    luaunit.assertErrorMsgContains("Stack underflow", 
        function() ps:eval("dup") end)
end

function testExch()
    print("📚 Testing: exch")
    local ps = PostScript.new()
    ps:eval("1 2 exch")
    luaunit.assertEquals(#ps.stack, 2)
    luaunit.assertEquals(ps:pop(), 1)
    luaunit.assertEquals(ps:pop(), 2)
end

function testExchWithMoreElements()
    print("📚 Testing: exch with deeper stack")
    local ps = PostScript.new()
    ps:eval("10 20 30 exch")
    -- Stack should be: 10 30 20
    luaunit.assertEquals(#ps.stack, 3)
    luaunit.assertEquals(ps:pop(), 20)
    luaunit.assertEquals(ps:pop(), 30)
    luaunit.assertEquals(ps:pop(), 10)
end

function testExchInsufficient()
    print("📚 Testing: exch with insufficient stack (error)")
    local ps = PostScript.new()
    ps:push(1)
    luaunit.assertErrorMsgContains("Stack underflow", 
        function() ps:eval("exch") end)
end

function testClear()
    print("📚 Testing: clear")
    local ps = PostScript.new()
    ps:eval("1 2 3 clear")
    luaunit.assertEquals(#ps.stack, 0)
end

function testClearEmpty()
    print("📚 Testing: clear on empty stack")
    local ps = PostScript.new()
    ps:eval("clear")
    luaunit.assertEquals(#ps.stack, 0)
end

function testCount()
    print("📚 Testing: count")
    local ps = PostScript.new()
    ps:eval("1 2 3 count")
    luaunit.assertEquals(#ps.stack, 4)  -- 3 numbers + count result
    luaunit.assertEquals(ps:pop(), 3)   -- count returns 3
end

function testCountEmpty()
    print("📚 Testing: count on empty stack")
    local ps = PostScript.new()
    ps:eval("count")
    luaunit.assertEquals(#ps.stack, 1)
    luaunit.assertEquals(ps:pop(), 0)
end

function testCopy()
    print("📚 Testing: copy")
    local ps = PostScript.new()
    ps:eval("1 2 3 2 copy")
    -- Should copy top 2 elements: 2 and 3
    -- Result: 1 2 3 2 3
    luaunit.assertEquals(#ps.stack, 5)
    luaunit.assertEquals(ps:pop(), 3)
    luaunit.assertEquals(ps:pop(), 2)
    luaunit.assertEquals(ps:pop(), 3)
    luaunit.assertEquals(ps:pop(), 2)
    luaunit.assertEquals(ps:pop(), 1)
end

function testCopyAll()
    print("📚 Testing: copy all elements")
    local ps = PostScript.new()
    ps:eval("10 20 30 3 copy")
    -- Copy all 3 elements: 10,20,30,10,20,30
    luaunit.assertEquals(#ps.stack, 6)
end

function testCopyInvalid()
    print("📚 Testing: copy with invalid count (error)")
    local ps = PostScript.new()
    ps:eval("1 2 3")
    luaunit.assertErrorMsgContains("Not enough elements", 
        function() ps:eval("5 copy") end)
end

function testStackManipulationChain()
    print("📚 Testing: chain of stack operations")
    local ps = PostScript.new()
    ps:eval("5 dup mul")  -- 5 * 5 = 25
    luaunit.assertEquals(ps:pop(), 25)
end

function testComplexStackOperations()
    print("📚 Testing: complex stack operations")
    local ps = PostScript.new()
    -- 1 2 3 exch pop dup
    -- Step by step:
    -- 1 2 3 exch -> 1 3 2
    -- pop -> 1 3
    -- dup -> 1 3 3
    ps:eval("1 2 3 exch pop dup")
    luaunit.assertEquals(#ps.stack, 3)
    luaunit.assertEquals(ps:pop(), 3)
    luaunit.assertEquals(ps:pop(), 3)
    luaunit.assertEquals(ps:pop(), 1)
end

-- Run all tests
print("\n" .. string.rep("=", 60))
print("🧪 TESTING STACK OPERATIONS")
print(string.rep("=", 60))
os.exit(luaunit.LuaUnit.run())