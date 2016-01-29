@Echo OFF
@setlocal

IF %1.==. GOTO Usage

set NUGET_EXE=%~dp0..\.nuget\nuget.exe

set BASE_PATH=%1
set VERSION=%2
set SRC_DIR=%3
IF %2 == "" set VERSION=%~dp0..\Build\Version.txt

@echo CreateOrleansNugetPackages running in directory = %1
@cd
@echo CreateOrleansNugetPackages version file = %VERSION% from base dir = %BASE_PATH% using nuget location = %NUGET_EXE%

if "%BASE_PATH%" == "." (
	if EXIST "Release" (
		set BASE_PATH=Release
	) else if EXIST "Debug" (
		set BASE_PATH=Debug
	)
)
@echo Using binary drop location directory %BASE_PATH%

if EXIST "%VERSION%" (
      @Echo Using version number from file %VERSION%
      FOR /F "usebackq tokens=1,2,3,4 delims=." %%i in (`type "%VERSION%"`) do (

	   set VERSION=%%i.%%j.%%k
	   set VERSION_BETA=%%l
	  )
	  
) else (
    @Echo ERROR: Unable to read version number from file %VERSION%
    GOTO Usage
)

set VERSION=%PRODUCT_VERSION%



if not "%VERSION_BETA%" == "" ( set VERSION=%VERSION%-%VERSION_BETA% )

@echo VERSION_BETA=%VERSION_BETA%
@echo VERSION=%VERSION%

@echo CreateOrleansNugetPackages: Version = %VERSION% -- Drop location = %BASE_PATH% -- SRC_DIR=%SRC_DIR%

@set NUGET_PACK_OPTS= -Version %VERSION%
@set NUGET_PACK_OPTS=%NUGET_PACK_OPTS% -NoPackageAnalysis -Symbols
@set NUGET_PACK_OPTS=%NUGET_PACK_OPTS% -BasePath "%BASE_PATH%" -Properties SRC_DIR=%SRC_DIR%
@REM @set NUGET_PACK_OPTS=%NUGET_PACK_OPTS% -Verbosity detailed

FOR %%G IN ("%~dp0*.nuspec") DO (
  "%NUGET_EXE%" pack "%%G" %NUGET_PACK_OPTS%
  if ERRORLEVEL 1 EXIT /B 1
)

GOTO EOF

:Usage
@ECHO Usage:
@ECHO    CreateOrleansPackages ^<Path to Orleans SDK folder^> ^<VersionFile^>
EXIT /B -1

:EOF
