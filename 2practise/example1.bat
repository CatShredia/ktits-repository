@echo off
setlocal

cd ../files/
pause

set "group_number=221"
set "surname=Evimov"
set "name=Aleksandr"
set "patronymic=Andreevich"
set "specialty=Svinka Pepa"

if not exist "%group_number%" (
    echo Creating group folder %group_number%
    md "%group_number%"
)

cd "%group_number%"

if not exist "%surname%" (
    echo Creating folder %surname%
    md "%surname%"
)

cd "%surname%"

echo Current directory: %CD%

echo Creating folders Proba and Zadanie
md Proba
md Zadanie

echo Content of %surname% folder:
dir

pause

echo Creating text1.txt in Proba folder
echo %surname% %name% %patronymic% > Proba\text1.txt

echo Creating text2.txt in Proba folder
echo %specialty% > Proba\text2.txt

echo Combining text1.txt and text2.txt into itog.txt
copy Proba\text1.txt+Proba\text2.txt Zadanie\itog.txt

echo Content of Proba folder:
dir Proba

echo.
echo Content of Zadanie folder:
dir Zadanie

pause

echo Content of Zadanie\itog.txt file:
type Zadanie\itog.txt

pause

echo Copying itog.txt to Proba as new.txt
copy Zadanie\itog.txt Proba\new.txt

echo Copying files to Zadanie folder
copy "C:\Windows\system.ini" Zadanie\
copy "C:\Program Files\Common Files\Microsoft Shared\MSInfo\msinfo32.exe" Zadanie\
copy "%SystemRoot%\system32\notepad.exe" Zadanie\
copy "%SystemRoot%\Fonts\arial.ttf" Zadanie\

echo Content of Zadanie folder after copying files:
dir Zadanie

pause

echo Sorting files in Zadanie by size:
dir Zadanie /OS

pause

echo Sorting files in Zadanie by date:
dir Zadanie /OD

pause

echo Sorting files in Zadanie by type:
dir Zadanie /OE

pause

echo Sorting files in Zadanie by name:
dir Zadanie /ON

pause

echo Renaming Proba folder to PRIMER
move Proba PRIMER

echo Deleting folder %cd% (WARNING! LAST OPERATION!)
pause
cd ..
rd /s /q "%surname%"

echo All actions completed.
pause

endlocal

cd C:\git-repositories\ktits-repository\
CALL C:\git-repositories\ktits-repository\delete_files.bat