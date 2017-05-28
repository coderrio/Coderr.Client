@echo off
if "%1"=="" goto blank
if "%1"=="pack" goto pack

:push
rem Here is the publish section
if "%OTE_KEY%"=="" goto nokey
set nugetKey=%OTE_KEY%

if "%2"=="" goto blank
set packageName=.%2
if "%2"=="core" set packageName=

FOR /F "delims=|" %%I IN ('DIR "NugetPackages\OneTrueError.Client%packageName%*.nupkg" /B /O:D') DO SET packageName=%%I
client\.nuget\nuget push NugetPackages\%packageName% %nugetKey%
goto end

:pack
if "%2"=="" goto blank
set packageName=.%2
if "%2"=="core" set packageName=""

set msbuild.exe=
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuild.exe=%%D\MSBuild.exe
%msbuild.exe% client%packageName%\OneTrueError.Client%packageName%\OneTrueError.client%packageName%.csproj /t:Build /p:Configuration=Release /nologo
client\.nuget\nuget pack client%packageName%\OneTrueError.Client%packageName%\OneTrueError.client%packageName%.csproj -OutputDirectory NugetPackages -Prop Configuration=Release
client\.nuget\nuget pack client%packageName%\OneTrueError.Client%packageName%\OneTrueError.client%packageName%.csproj -OutputDirectory NugetPackages -Prop Configuration=Release


goto end

:blank
echo To pack package, write:
echo publish pack [clientType]
echo
echo  Where client type is for instance "winforms", "aspnet", "aspnet.mvc" or "core" for the core library.
echo
echo
echo To push package run
echo publish nugetpackages\yourPackage.nukpg
goto end

:nokey
echo You must set an environment variable named "OTE_KEY" which contains the nuget key used to publish the package.


:end