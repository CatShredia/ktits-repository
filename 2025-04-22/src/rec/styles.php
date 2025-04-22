<?php
// файл нужный для того. чтобы подсоединить стили к views
header("Content-type: text/css; charset: UTF-8"); // Определяем правильный MIME-тип
readfile("style.css"); // Читаем и отправляем содержимое CSS-файла
