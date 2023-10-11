Powershell.exe -Command Set-ExecutionPolicy Unrestricted

Powershell.exe .\TestPrep.ps1

echo About to run Test1103.bsmn ...
.\..\..\..\BismNormalizer.CommandLine\bin\Release\BismNormalizer.exe Test1103.bsmn

if %ERRORLEVEL% gtr 0 exit /b %ERRORLEVEL%

echo About to run Test1200.bsmn ...
.\..\..\..\BismNormalizer.CommandLine\bin\Release\BismNormalizer.exe Test1200.bsmn

exit /b %ERRORLEVEL%
