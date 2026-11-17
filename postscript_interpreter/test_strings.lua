-- test_strings.lua
-- Tests for string operations

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

function testStringLength()
    local ps = PostScript.new()
    ps:eval("(Hello) length")
    luaunit.assertEquals(ps:pop(), 5)
end

function testEmptyStringLength()
    local ps = PostScript.new()
    ps:eval("() length")
    luaunit.assertEquals(ps:pop(), 0)
end

function testStringGet()
    local ps = PostScript.new()
    ps:eval("(Hello) 0 get")
    luaunit.assertEquals(ps:pop(), 72)  -- ASCII 'H'
end

function testStringGetLast()
    local ps = PostScript.new()
    ps:eval("(Hello) 4 get")
    luaunit.assertEquals(ps:pop(), 111)  -- ASCII 'o'
end

function testStringGetInterval()
    local ps = PostScript.new()
    ps:eval("(Hello) 1 3 getinterval")
    luaunit.assertEquals(ps:pop(), "ell")
end

function testStringGetIntervalStart()
    local ps = PostScript.new()
    ps:eval("(Hello) 0 2 getinterval")
    luaunit.assertEquals(ps:pop(), "He")
end

function testStringGetIntervalEnd()
    local ps = PostScript.new()
    ps:eval("(Hello) 3 2 getinterval")
    luaunit.assertEquals(ps:pop(), "lo")
end

function testStringPutInterval()
    local ps = PostScript.new()
    ps:eval("(He) 2 (llo) putinterval")
    luaunit.assertEquals(ps:pop(), "Hello")
end

function testStringPutIntervalMiddle()
    local ps = PostScript.new()
    ps:eval("(Hxlo) 1 (el) putinterval")
    luaunit.assertEquals(ps:pop(), "Helo")
end

function testStringPutIntervalEnd()
    local ps = PostScript.new()
    ps:eval("(Hel) 3 (lo) putinterval")
    luaunit.assertEquals(ps:pop(), "Hello")
end

function testStringPutIntervalOverwrite()
    local ps = PostScript.new()
    ps:eval("(abcde) 2 (xyz) putinterval")
    luaunit.assertEquals(ps:pop(), "abxyz")
end

-- Run tests
print("\n" .. string.rep("=", 60))
print("📝 TESTING STRING OPERATIONS")
print(string.rep("=", 60))
os.exit(luaunit.LuaUnit.run())