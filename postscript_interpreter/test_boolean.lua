-- test_boolean.lua
-- Tests for boolean and comparison operations

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

-- Boolean value tests
function testTrue()
    local ps = PostScript.new()
    ps:eval("true")
    luaunit.assertEquals(ps:pop(), true)
end

function testFalse()
    local ps = PostScript.new()
    ps:eval("false")
    luaunit.assertEquals(ps:pop(), false)
end

-- Logical operator tests
function testAnd()
    local ps = PostScript.new()
    ps:eval("true true and")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("true false and")
    luaunit.assertEquals(ps:pop(), false)
    
    ps:eval("false true and")
    luaunit.assertEquals(ps:pop(), false)
    
    ps:eval("false false and")
    luaunit.assertEquals(ps:pop(), false)
end

function testOr()
    local ps = PostScript.new()
    ps:eval("true true or")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("true false or")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("false true or")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("false false or")
    luaunit.assertEquals(ps:pop(), false)
end

function testNot()
    local ps = PostScript.new()
    ps:eval("true not")
    luaunit.assertEquals(ps:pop(), false)
    
    ps:eval("false not")
    luaunit.assertEquals(ps:pop(), true)
end

-- Comparison tests
function testEq()
    local ps = PostScript.new()
    ps:eval("5 5 eq")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("5 3 eq")
    luaunit.assertEquals(ps:pop(), false)
end

function testNe()
    local ps = PostScript.new()
    ps:eval("5 3 ne")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("5 5 ne")
    luaunit.assertEquals(ps:pop(), false)
end

function testGt()
    local ps = PostScript.new()
    ps:eval("5 3 gt")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("3 5 gt")
    luaunit.assertEquals(ps:pop(), false)
end

function testLt()
    local ps = PostScript.new()
    ps:eval("3 5 lt")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("5 3 lt")
    luaunit.assertEquals(ps:pop(), false)
end

function testGe()
    local ps = PostScript.new()
    ps:eval("5 5 ge")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("5 3 ge")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("3 5 ge")
    luaunit.assertEquals(ps:pop(), false)
end

function testLe()
    local ps = PostScript.new()
    ps:eval("5 5 le")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("3 5 le")
    luaunit.assertEquals(ps:pop(), true)
    
    ps:eval("5 3 le")
    luaunit.assertEquals(ps:pop(), false)
end

-- Combined tests
function testComplexBoolean()
    local ps = PostScript.new()
    -- (5 > 3) and (10 < 20)
    ps:eval("5 3 gt 10 20 lt and")
    luaunit.assertEquals(ps:pop(), true)
    
    -- (5 > 10) or (3 < 4)
    ps:eval("5 10 gt 3 4 lt or")
    luaunit.assertEquals(ps:pop(), true)
end

-- Run tests
print("\n" .. string.rep("=", 60))
print("🎯 TESTING BOOLEAN & COMPARISON OPERATIONS")
print(string.rep("=", 60))
os.exit(luaunit.LuaUnit.run())