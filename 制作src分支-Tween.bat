@echo off

set b="version"
set version ="1"
set branchName="upm"
@REM REM 获取版本号
for /f "tokens=1,2* delims=:," %%a in (Assets/WooTween/package.json) do (
    echo %%a| findstr %b% >nul && (
       set version=  %%b
    ) || (
        @REM echo %%a nnn %b%
    )
)


set version=%version: =%
echo on
git subtree split --prefix=Assets/WooTween --branch %branchName%
git push origin %branchName%:%branchName%
git tag %branchName%_%version% %branchName%
git push origin %branchName% --tags
set cur=%~dp0


pause