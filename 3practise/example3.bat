@REM 1 задание
@ECHO off

cd ..
setlocal
set "folder_path=%cd%\files\"
set "folder_path_rod=%cd%\"

cls
echo on
D:
cd\
dir

CALL "%folder_path_rod%delete_files.bat"
