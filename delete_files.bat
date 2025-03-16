@ECHO OFF
setlocal

set "folder_path=%~dp0files"

REM Проверка, существует ли папка
if not exist "%folder_path%" (
  echo Папка "%folder_path%" не существует.
  pause
  exit /b 1
)

echo Do you really want to delete the files? "%folder_path%"?
choice /M "(Y/N): "
if errorlevel 2 goto :eof

REM Удаление файлов (ОСТОРОЖНО!)
echo Deleting files...
del /f /q "%folder_path%\*"

for /d %%a in ("%folder_path%\*") do (
  rd /s /q "%%a"
)

echo All files and directories have been deleted.

cd "files"
mkdir "NAME1"
mkdir "NAME2"
echo Бан > "%folder_path%\NAME1\text.txt"

pause
endlocal