-- test_dictionary.lua
-- Tests for dictionary operations

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

-- Test dict creation
function testDict()
    local ps = PostScript.new()
    ps:eval("10 dict")
    local dict = ps:pop()
    luaunit.assertNotNil(dict)
    luaunit.assertEquals(dict.__type, "dict")
    luaunit.assertEquals(dict.maxlength, 10)
end

-- Test def and lookup
function testDef()
    local ps = PostScript.new()
    ps:eval("/x 10 def")
    ps:eval("x")
    luaunit.assertEquals(ps:pop(), 10)
end

-- Test multiple definitions
function testMultipleDefs()
    local ps = PostScript.new()
    ps:eval("/x 10 def /y 20 def /z 30 def")
    ps:eval("x")
    luaunit.assertEquals(ps:pop(), 10)
    ps:eval("y")
    luaunit.assertEquals(ps:pop(), 20)
    ps:eval("z")
    luaunit.assertEquals(ps:pop(), 30)
end

-- Test begin/end
function testBeginEnd()
    local ps = PostScript.new()
    ps:eval("/mydict 5 dict def")
    ps:eval("mydict begin")
    ps:eval("/value 100 def")
    ps:eval("value")
    luaunit.assertEquals(ps:pop(), 100)
    ps:eval("end")
end

-- Test dictionary scoping
function testScoping()
    local ps = PostScript.new()
    ps:eval("/global 5 def")
    ps:eval("/mydict 5 dict def")
    ps:eval("mydict begin")
    ps:eval("/global 10 def")
    ps:eval("end")
    ps:eval("global")
    luaunit.assertEquals(ps:pop(), 5)
end

-- Test dict length
function testDictLength()
    local ps = PostScript.new()
    ps:eval("/mydict 10 dict def")
    ps:eval("mydict begin")
    ps:eval("/a 1 def /b 2 def /c 3 def")
    ps:eval("end")
    ps:eval("mydict length")
    luaunit.assertEquals(ps:pop(), 3)
end

-- Test maxlength
function testMaxLength()
    local ps = PostScript.new()
    ps:eval("/mydict 20 dict def")
    ps:eval("mydict maxlength")
    luaunit.assertEquals(ps:pop(), 20)
end

-- Run tests
print("\n" .. string.rep("=", 60))
print("📚 TESTING DICTIONARY OPERATIONS")
print(string.rep("=", 60))
os.exit(luaunit.LuaUnit.run())