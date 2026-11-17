-- test_io.lua
-- Tests for I/O operations (focus on stack behavior, not output capture)

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

-- Test that print consumes the stack correctly
function testPrintConsumesStack()
    local ps = PostScript.new()
    ps:eval("(Hello) print")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test that print works with numbers
function testPrintNumberConsumesStack()
    local ps = PostScript.new()
    ps:eval("42 print")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test that = consumes the stack correctly
function testEqualsConsumesStack()
    local ps = PostScript.new()
    ps:eval("(World) =")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test that = works with numbers
function testEqualsNumberConsumesStack()
    local ps = PostScript.new()
    ps:eval("100 =")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test that == consumes the stack correctly
function testEqualsEqualsConsumesStack()
    local ps = PostScript.new()
    ps:eval("(Hello) ==")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test that == works with numbers
function testEqualsEqualsNumberConsumesStack()
    local ps = PostScript.new()
    ps:eval("42 ==")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test that == works with booleans
function testEqualsEqualsBooleanConsumesStack()
    local ps = PostScript.new()
    ps:eval("true ==")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Test multiple operations
function testMultipleIOOperations()
    local ps = PostScript.new()
    ps:eval("(A) print (B) print (C) print")
    luaunit.assertEquals(#ps.stack, 0)
end

-- Run tests
print("\n" .. string.rep("=", 60))
print("📤 TESTING I/O OPERATIONS (Stack Behavior)")
print(string.rep("=", 60))
print("\nNote: Visual verification required for actual output")
print("Expected output: Hello, World, 42, 100, etc.\n")
os.exit(luaunit.LuaUnit.run())