-- test_flow.lua
-- Tests for flow control operations

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

function testExec()
    local ps = PostScript.new()
    ps:eval("{ 42 } exec")
    luaunit.assertEquals(ps:pop(), 42)
end

function testIfTrue()
    local ps = PostScript.new()
    ps:eval("true { 42 } if")
    luaunit.assertEquals(ps:pop(), 42)
end

function testIfFalse()
    local ps = PostScript.new()
    ps:eval("false { 42 } if")
    luaunit.assertEquals(#ps.stack, 0)
end

function testIfelseTrue()
    local ps = PostScript.new()
    ps:eval("true { 10 } { 20 } ifelse")
    luaunit.assertEquals(ps:pop(), 10)
end

function testIfelseFalse()
    local ps = PostScript.new()
    ps:eval("false { 10 } { 20 } ifelse")
    luaunit.assertEquals(ps:pop(), 20)
end

function testRepeat()
    local ps = PostScript.new()
    ps:eval("/count 0 def 5 { /count count 1 add def } repeat count")
    luaunit.assertEquals(ps:pop(), 5)
end

function testForLoop()
     local ps = PostScript.new()
    -- This should sum numbers 1 through 5
     ps:eval("0 1 1 5 { add } for")
    luaunit.assertEquals(ps:pop(), 15)
end

function testNestedProcedures()
    local ps = PostScript.new()
    ps:eval("/double { 2 mul } def 5 double")
    luaunit.assertEquals(ps:pop(), 10)
end

-- Run tests
print("\n" .. string.rep("=", 60))
print("🔄 TESTING FLOW CONTROL OPERATORS")
print(string.rep("=", 60))
os.exit(luaunit.LuaUnit.run())