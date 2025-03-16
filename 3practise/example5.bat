@ECHO OFF

cd ..
setlocal
set "folder_path=%cd%\files\"
set "folder_path_rod=%cd%\"

MD %folder_path%\RPK
MD %folder_path%\RPK\Students
MD %folder_path%\RPK\Students\2B

ECHO Это файл Иванова > %folder_path%\RPK\Students\2B\ivanov.txt

COPY %folder_path%\RPK\Students\2B\ivanov.txt %folder_path%\RPK\

REN %folder_path%\RPK\ivanov.txt petrov.txt

DEL %folder_path%\RPK\Students\2B\ivanov.txt

ECHO Командный файл создал студент Ефимов Александр Андреевич

PAUSE