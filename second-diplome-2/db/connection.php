<?php
$host = 'localhost';
$dbname = 'example123';
$user = 'root';
$pass = '';

try {
    $link = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8", $user, $pass);
    $link->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    die('Ошибка подключения: ' . $e->getMessage());
}
