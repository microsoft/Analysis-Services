@echo off
copy /y bin\Release\AlmToolkit.exe ReleaseItems\ForSigning\AlmToolkit.exe
copy /y ..\BismNormalizer\bin\Release\BismNormalizer.dll ReleaseItems\ForSigning\BismNormalizer.dll
copy /y ..\BismNormalizer.CommandLine\bin\Release\BismNormalizer.CommandLine.exe ReleaseItems\ForSigning\BismNormalizer.CommandLine.exe
