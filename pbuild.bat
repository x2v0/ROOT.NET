:: $Id: pbuild.bat 4923 2017-01-18 11:52:04Z onuchin $
:: Author: Valeriy Onuchin  11.10.2011
::
:: To  build - pass debug or release at the command line, e.g. pbuild.bat debug


:ROOTSYS
if not "%ROOTSYS%"=="" goto build

@echo off
:: define ROOTSYS enviromental variable
:input
set INPUT=
set /P INPUT=Enter ROOT path: 
if "%INPUT%"=="" goto input
if not exist "%INPUT%\bin\root.exe" (
echo %=% Wrong path to ROOT directory!
goto input
)

set ROOTSYS=%INPUT%

::reg add "HKCU\Environment" /v ROOTSYS /t REG_SZ /d "%ROOTSYS%"
::reg add "HKCU\Environment" /v Path /t REG_SZ /d "%PATH%;%ROOTSYS%\bin"

reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v ROOTSYS /t REG_SZ /d "%ROOTSYS%"
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v Path /t REG_SZ /d "%PATH%;%ROOTSYS%\bin"

echo %=% To activate ROOTSYS enviromental variable restart either explorer.exe or computer!

:: building ...
:build
@echo on

call defines.bat

set conf=release
if not "%~1" == ""  set conf=%1

%msbuild% .\ROOT.NET\ROOT.NET.%vs%sln /t:Build /p:Configuration=%conf%  /p:Platform=x86
%msbuild% .\hsimple.NET\hsimple.%vs%sln /t:Build /p:Configuration=%conf%  /p:Platform=x86

cmd /c xcopy /y "hsimple.NET\%conf%\*" "bin\hsimple\"


if %conf%=="release" (
del /s /q *.pdb
)

pause

