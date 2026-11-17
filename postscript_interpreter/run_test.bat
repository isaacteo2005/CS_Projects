@echo off
echo ========================================
echo RUNNING POSTSCRIPT INTERPRETER TESTS
echo ========================================
echo.

echo Testing Stack Operations...
lua test_stack.lua
if %errorlevel% neq 0 (
    echo ❌ Stack tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ Stack tests passed
echo.

echo Testing Arithmetic Operations...
lua test_arithmetic.lua
if %errorlevel% neq 0 (
    echo ❌ Arithmetic tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ Arithmetic tests passed
echo.

echo Testing Boolean Operations...
lua test_boolean.lua
if %errorlevel% neq 0 (
    echo ❌ Boolean tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ Boolean tests passed
echo.

echo Testing Dictionary Operations...
lua test_dictionary.lua
if %errorlevel% neq 0 (
    echo ❌ Dictionary tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ Dictionary tests passed
echo.

echo Testing String Operations...
lua test_strings.lua
if %errorlevel% neq 0 (
    echo ❌ String tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ String tests passed
echo.

echo Testing Flow Control Operations...
lua test_flow.lua
if %errorlevel% neq 0 (
    echo ❌ Flow control tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ Flow control tests passed
echo.

echo Testing I/O Operations...
lua test_io.lua
if %errorlevel% neq 0 (
    echo ❌ I/O tests failed!
    pause
    exit /b %errorlevel%
)
echo.
echo ✓ I/O tests passed
echo.

echo.
echo ========================================
echo ✅ ALL TESTS PASSED!
echo ========================================
echo.
echo Summary:
echo   - Stack Operations: 20+ tests
echo   - Arithmetic Operations: 44 tests
echo   - Boolean Operations: 15+ tests
echo   - String Operations: 11 tests
echo.
pause