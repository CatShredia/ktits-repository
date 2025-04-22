<?php
// файл нужен для проверки mysql подключения
$host = 'mysql';
$port = 3306;
$user = 'catshredia';
$password = 'password';
$database = 'php_db';

try {
    $pdo = new PDO("mysql:host=$host;port=$port;dbname=$database", $user, $password);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    echo "Connected successfully to MySQL!\n";

    // Example query (optional)
    $sql = "SELECT VERSION()";
    $stmt = $pdo->query($sql);
    $version = $stmt->fetchColumn();
    echo "MySQL version: " . $version . "\n";
} catch (PDOException $e) {
    echo "Connection failed: " . $e->getMessage() . "\n";
}
