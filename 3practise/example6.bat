@ECHO OFF
cd ..
setlocal
set "folder_path=%cd%\files\"
set "folder_path_rod=%cd%\"

MD %folder_path%new

ECHO ОСС крутой предмет! > %folder_path%new\new.txt

COPY %folder_path%new\new.txt %folder_path%new\new_copy.txt

ECHO Файл успешно создан и скопирован!

PAUSE