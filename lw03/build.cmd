if "%~1" == "" (
    goto IncorrectNumberOfArguments
)

cd src/Frontend
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../Backend
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../TextListener
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../..
if exist "%~1" (
    rd /s /q "%~1"
)

mkdir "%~1"\Frontend
mkdir "%~1"\Backend
mkdir "%~1"\TextListener
mkdir "%~1"\config

xcopy src\Frontend\bin\Release\netcoreapp2.2\publish "%~1"\Frontend\
xcopy src\Backend\bin\Release\netcoreapp2.2\publish "%~1"\Backend\
xcopy src\TextListener\bin\Release\netcoreapp2.2\publish "%~1"\TextListener\
xcopy config "%~1"\config
xcopy run.cmd "%~1"
xcopy stop.cmd "%~1"

echo BUILD SUCCESS
exit /b 0

:IncorrectNumberOfArguments
    echo Incorrect number of arguments.
	echo Example: build.cmd <SemVer>(MAJOR,MINOR,PATCH)
    exit /b 1

:BuildError
    echo Failed to build project.
    exit /b 1	
	