@echo off
copy /y ReleaseItems\SignedFiles\AlmToolkit.exe obj\Release\AlmToolkit.exe
copy /y ReleaseItems\SignedFiles\BismNormalizer.dll ..\BismNormalizer\obj\Release\BismNormalizer.dll
copy /y ReleaseItems\SignedFiles\BismNormalizer.CommandLine.exe ..\BismNormalizer.CommandLine\obj\Release\BismNormalizer.CommandLine.exe
