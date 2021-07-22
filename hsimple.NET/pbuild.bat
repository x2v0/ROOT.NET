:: $Id: pbuild.bat 4923 2017-01-18 11:52:04Z onuchin $
:: Author: Valeriy Onuchin   07.06.2011
::
:: To  build - pass debug or release at the command line, e.g. pbuild.bat debug


call defines.bat

set conf=release
if not "%~1" == ""  set conf=%1

%msbuild% .\hsimple.%vs%sln /t:Build /p:Configuration=%conf%  /p:Platform=x86

::if not "%~1" == ""  set conf="%1"

::%msbuild% hsimple.NET\hsimple.sln /t:Build /p:Platform=Win32 /p:Configuration=%conf%


:end

if %conf%==release (
del /s /q *.pdb
)

pause
