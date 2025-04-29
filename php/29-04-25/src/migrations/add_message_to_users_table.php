<?php
$host = 'mysql';
$username = 'catshredia';
$password = 'password';
$database = 'php_db';

//  устанавливаем соединение с базой данных
$mysqli = new mysqli($host, $username, $password, $database);

// Проверка соединения
if ($mysqli->connect_errno) {
    echo "Не удалось подключиться к MySQL: " . $mysqli->connect_error;
    exit();
}

// Проверка наличия колонки `message` в таблице `users`
$checkColumnQuery = "SHOW COLUMNS FROM users LIKE 'message'";
$result = $mysqli->query($checkColumnQuery);

// если колонка существует выводим сообщение
if ($result->num_rows > 0) {
    echo "Колонка 'message' уже существует в таблице 'users'.\n";
} else {
    // SQL-запрос для добавления колонки `message` к таблице `users`
    $sql = "ALTER TABLE users ADD COLUMN message TEXT AFTER email;";

    // Выполнение запроса
    if ($mysqli->query($sql) === TRUE) {
        echo "Колонка 'message' успешно добавлена к таблице 'users'.\n";
    } else {
        echo "Ошибка при добавлении колонки: " . $mysqli->error;
    }
}

// Закрытие соединения
$mysqli->close();
