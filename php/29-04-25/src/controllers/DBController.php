<?php

class DBController extends Controller
{
    public function index()
    {
        // Учетные данные для подключения к базе данных
        $host = 'mysql';
        $username = 'catshredia';
        $password = 'password';
        $database = 'php_db';

        // Создаем объект mysqli
        $mysqli = new mysqli($host, $username, $password, $database);

        // Проверяем соединение
        if ($mysqli->connect_errno) {
            echo "Не удалось подключиться к MySQL: " . $mysqli->connect_error;
            exit();
        } else {
            echo "Успешное подключение к MySQL!";
        }
    }
}
