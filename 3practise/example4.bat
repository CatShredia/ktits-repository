@ ECHO OFF

cd ..
setlocal
set "folder_path=%cd%\files\"
set "folder_path_rod=%cd%\"

TYPE %folder_path_rod%example.txt
PAUSE file show

CALL "3practise\example2.bat"
