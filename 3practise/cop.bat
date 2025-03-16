@REM @ECHO OFF

cd ..
setlocal
set "folder_path=%cd%\files\"

COPY %folder_path%NAME1\%1 %folder_path%NAME2\

CALL "delete_files.bat"