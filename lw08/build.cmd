if "%~1" == "" (
    goto IncorrectNumberOfArguments
)

cd src/Core/Core
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../Frontend/Frontend
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../Backend/Backend
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../TextListener/TextListener
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../TextRankCalc/TextRankCalc
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../VowelConsCounter/VowelConsCounter
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../VowelConsRater/VowelConsRater
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../TextStatistics/TextStatistics
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../TextProcessingLimiter/TextProcessingLimiter
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../../..
if exist "%~1" (
    rd /s /q "%~1"
)

mkdir "%~1"\Frontend
mkdir "%~1"\Backend
mkdir "%~1"\TextListener
mkdir "%~1"\TextRankCalc
mkdir "%~1"\VowelConsCounter
mkdir "%~1"\VowelConsRater
mkdir "%~1"\TextStatistics
mkdir "%~1"\TextProcessingLimiter
mkdir "%~1"\config

xcopy src\Frontend\Frontend\bin\Release\netcoreapp2.2\publish "%~1"\Frontend\
xcopy src\Backend\Backend\bin\Release\netcoreapp2.2\publish "%~1"\Backend\
xcopy src\TextListener\TextListener\bin\Release\netcoreapp2.2\publish "%~1"\TextListener\
xcopy src\TextRankCalc\TextRankCalc\bin\Release\netcoreapp2.2\publish "%~1"\TextRankCalc\
xcopy src\VowelConsCounter\VowelConsCounter\bin\Release\netcoreapp2.2\publish  "%~1"\VowelConsCounter\
xcopy src\VowelConsRater\VowelConsRater\bin\Release\netcoreapp2.2\publish  "%~1"\VowelConsRater\
xcopy src\TextStatistics\TextStatistics\bin\Release\netcoreapp2.2\publish  "%~1"\TextStatistics\
xcopy src\TextProcessingLimiter\TextProcessingLimiter\bin\Release\netcoreapp2.2\publish  "%~1"\TextProcessingLimiter\

xcopy config\application.properties "%~1"\Frontend\
xcopy config\application.properties "%~1"\Backend\
xcopy config\application.properties "%~1"\TextListener\
xcopy config\application.properties "%~1"\TextRankCalc\
xcopy config\application.properties  "%~1"\VowelConsCounter\
xcopy config\application.properties  "%~1"\VowelConsRater\
xcopy config\application.properties  "%~1"\TextStatistics\
xcopy config\application.properties  "%~1"\TextProcessingLimiter\
xcopy config  "%~1"\config\
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
	