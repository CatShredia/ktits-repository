@REM 1 задание
@ECHO off

cd ..
setlocal
set "folder_path=%cd%\files\"
set "folder_path_rod=%cd%\"

MD %folder_path%DOCUMENT
MD %folder_path%TEXT
COPY %folder_path_rod%*.docx %folder_path%DOCUMENT
COPY %folder_path_rod%*.txt %folder_path%TEXT

CALL "delete_files.bat"