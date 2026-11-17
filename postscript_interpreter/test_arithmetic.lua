-- test_arithmetic.lua
-- Comprehensive tests for arithmetic operations

local luaunit = require("luaunit")
local PostScript = require("ps_interpreter")

-- Test suite for arithmetic operations
function testAdd()
    print("\n🔢 Testing: add")
    local ps = PostScript.new()
    ps:eval("5 3 add")
    luaunit.assertEquals(ps:pop(), 8)
end

function testAddNegative()
    print("🔢 Testing: add with negative numbers")
    local ps = PostScript.new()
    ps:eval("-5 3 add")
    luaunit.assertEquals(ps:pop(), -2)
end

function testAddZero()
    print("🔢 Testing: add with zero")
    local ps = PostScript.new()
    ps:eval("5 0 add")
    luaunit.assertEquals(ps:pop(), 5)
end

function testAddDecimal()
    print("🔢 Testing: add with decimals")
    local ps = PostScript.new()
    ps:eval("3.5 2.7 add")
    luaunit.assertEquals(ps:pop(), 6.2)
end

function testAddMultiple()
    print("🔢 Testing: multiple additions")
    local ps = PostScript.new()
    ps:eval("1 2 3 4 add add add")
    -- 1 + 2 + 3 + 4 = 10
    luaunit.assertEquals(ps:pop(), 10)
end

function testSub()
    print("🔢 Testing: sub")
    local ps = PostScript.new()
    ps:eval("10 3 sub")
    luaunit.assertEquals(ps:pop(), 7)
end

function testSubNegative()
    print("🔢 Testing: sub resulting in negative")
    local ps = PostScript.new()
    ps:eval("3 10 sub")
    luaunit.assertEquals(ps:pop(), -7)
end

function testSubDecimal()
    print("🔢 Testing: sub with decimals")
    local ps = PostScript.new()
    ps:eval("10.5 3.2 sub")
    luaunit.assertEquals(ps:pop(), 7.3)
end

function testMul()
    print("🔢 Testing: mul")
    local ps = PostScript.new()
    ps:eval("4 5 mul")
    luaunit.assertEquals(ps:pop(), 20)
end

function testMulNegative()
    print("🔢 Testing: mul with negative")
    local ps = PostScript.new()
    ps:eval("-4 5 mul")
    luaunit.assertEquals(ps:pop(), -20)
end

function testMulZero()
    print("🔢 Testing: mul by zero")
    local ps = PostScript.new()
    ps:eval("100 0 mul")
    luaunit.assertEquals(ps:pop(), 0)
end

function testMulDecimal()
    print("🔢 Testing: mul with decimals")
    local ps = PostScript.new()
    ps:eval("2.5 4 mul")
    luaunit.assertEquals(ps:pop(), 10)
end

function testDiv()
    print("🔢 Testing: div")
    local ps = PostScript.new()
    ps:eval("15 3 div")
    luaunit.assertEquals(ps:pop(), 5)
end

function testDivDecimal()
    print("🔢 Testing: div with decimal result")
    local ps = PostScript.new()
    ps:eval("10 4 div")
    luaunit.assertEquals(ps:pop(), 2.5)
end

function testDivNegative()
    print("🔢 Testing: div with negative")
    local ps = PostScript.new()
    ps:eval("-15 3 div")
    luaunit.assertEquals(ps:pop(), -5)
end

function testDivByZero()
    print("🔢 Testing: div by zero")
    local ps = PostScript.new()
    luaunit.assertErrorMsgContains("division by zero", 
        function() ps:eval("5 0 div") end)
end

function testIdiv()
    print("🔢 Testing: idiv (integer division)")
    local ps = PostScript.new()
    ps:eval("7 3 idiv")
    luaunit.assertEquals(ps:pop(), 2)
end

function testIdivNegative()
    print("🔢 Testing: idiv with negative")
    local ps = PostScript.new()
    ps:eval("-7 3 idiv")
    luaunit.assertEquals(ps:pop(), -3)  -- Floor division
end

function testMod()
    print("🔢 Testing: mod (modulo)")
    local ps = PostScript.new()
    ps:eval("17 5 mod")
    luaunit.assertEquals(ps:pop(), 2)
end

function testModNegative()
    print("🔢 Testing: mod with negative")
    local ps = PostScript.new()
    ps:eval("-17 5 mod")
    luaunit.assertEquals(ps:pop(), 3)  -- Lua's % behavior
end

function testAbs()
    print("🔢 Testing: abs")
    local ps = PostScript.new()
    ps:eval("-5 abs")
    luaunit.assertEquals(ps:pop(), 5)
end

function testAbsPositive()
    print("🔢 Testing: abs of positive")
    local ps = PostScript.new()
    ps:eval("5 abs")
    luaunit.assertEquals(ps:pop(), 5)
end

function testAbsZero()
    print("🔢 Testing: abs of zero")
    local ps = PostScript.new()
    ps:eval("0 abs")
    luaunit.assertEquals(ps:pop(), 0)
end

function testNeg()
    print("🔢 Testing: neg")
    local ps = PostScript.new()
    ps:eval("5 neg")
    luaunit.assertEquals(ps:pop(), -5)
end

function testNegNegative()
    print("🔢 Testing: neg of negative")
    local ps = PostScript.new()
    ps:eval("-5 neg")
    luaunit.assertEquals(ps:pop(), 5)
end

function testNegZero()
    print("🔢 Testing: neg of zero")
    local ps = PostScript.new()
    ps:eval("0 neg")
    luaunit.assertEquals(ps:pop(), 0)
end

function testCeiling()
    print("🔢 Testing: ceiling")
    local ps = PostScript.new()
    ps:eval("3.2 ceiling")
    luaunit.assertEquals(ps:pop(), 4)
end

function testCeilingInteger()
    print("🔢 Testing: ceiling of integer")
    local ps = PostScript.new()
    ps:eval("5 ceiling")
    luaunit.assertEquals(ps:pop(), 5)
end

function testCeilingNegative()
    print("🔢 Testing: ceiling of negative")
    local ps = PostScript.new()
    ps:eval("-3.7 ceiling")
    luaunit.assertEquals(ps:pop(), -3)
end

function testFloor()
    print("🔢 Testing: floor")
    local ps = PostScript.new()
    ps:eval("3.8 floor")
    luaunit.assertEquals(ps:pop(), 3)
end

function testFloorInteger()
    print("🔢 Testing: floor of integer")
    local ps = PostScript.new()
    ps:eval("5 floor")
    luaunit.assertEquals(ps:pop(), 5)
end

function testFloorNegative()
    print("🔢 Testing: floor of negative")
    local ps = PostScript.new()
    ps:eval("-3.2 floor")
    luaunit.assertEquals(ps:pop(), -4)
end

function testRound()
    print("🔢 Testing: round (up)")
    local ps = PostScript.new()
    ps:eval("3.6 round")
    luaunit.assertEquals(ps:pop(), 4)
end

function testRoundDown()
    print("🔢 Testing: round (down)")
    local ps = PostScript.new()
    ps:eval("3.4 round")
    luaunit.assertEquals(ps:pop(), 3)
end

function testRoundHalf()
    print("🔢 Testing: round (half)")
    local ps = PostScript.new()
    ps:eval("3.5 round")
    luaunit.assertEquals(ps:pop(), 4)
end

function testRoundNegative()
    print("🔢 Testing: round negative")
    local ps = PostScript.new()
    ps:eval("-3.6 round")
    luaunit.assertEquals(ps:pop(), -4)
end

function testSqrt()
    print("🔢 Testing: sqrt")
    local ps = PostScript.new()
    ps:eval("16 sqrt")
    luaunit.assertEquals(ps:pop(), 4)
end

function testSqrtDecimal()
    print("🔢 Testing: sqrt of decimal")
    local ps = PostScript.new()
    ps:eval("2 sqrt")
    luaunit.assertEquals(ps:pop(), math.sqrt(2))
end

function testSqrtZero()
    print("🔢 Testing: sqrt of zero")
    local ps = PostScript.new()
    ps:eval("0 sqrt")
    luaunit.assertEquals(ps:pop(), 0)
end

function testSqrtNegative()
    print("🔢 Testing: sqrt of negative (error)")
    local ps = PostScript.new()
    luaunit.assertErrorMsgContains("negative", 
        function() ps:eval("-16 sqrt") end)
end

function testArithmeticOrder()
    print("🔢 Testing: arithmetic order (LIFO)")
    local ps = PostScript.new()
    -- PostScript is postfix: (5 - 3) * 2
    ps:eval("5 3 sub 2 mul")
    luaunit.assertEquals(ps:pop(), 4)
end

function testComplexArithmetic()
    print("🔢 Testing: complex arithmetic expression")
    local ps = PostScript.new()
    -- (10 + 5) * (8 - 3) / 2 = 37.5
    ps:eval("10 5 add 8 3 sub mul 2 div")
    luaunit.assertEquals(ps:pop(), 37.5)
end

function testArithmeticWithStackOps()
    print("🔢 Testing: arithmetic combined with stack ops")
    local ps = PostScript.new()
    -- 5 dup mul = 5 * 5 = 25
    ps:eval("5 dup mul")
    luaunit.assertEquals(ps:pop(), 25)
    
    -- 5 3 exch add = 5 + 3 = 8 (exch swaps, but addition is commutative)
    ps:eval("5 3 exch add")
    luaunit.assertEquals(ps:pop(), 8)
end

function testArithmeticChain()
    print("🔢 Testing: long arithmetic chain")
    local ps = PostScript.new()
    ps:eval("1 2 add 3 add 4 add 5 add 6 add")
    luaunit.assertEquals(ps:pop(), 21)
end

-- Run all tests
print("\n" .. string.rep("=", 60)) 
print("🧮 TESTING ARITHMETIC OPERATIONS")
print(string.rep("=", 60))
os.exit(luaunit.LuaUnit.run())