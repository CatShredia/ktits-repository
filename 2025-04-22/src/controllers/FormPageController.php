<?php
class FormPageController extends Controller
{
    public function index()
    {
        $errors = $_SESSION['errors'] ?? []; // Получаем ошибки из сессии
        $successMessage = $_SESSION['success_message'] ?? ''; // Получаем сообщение об успехе из сессии
        unset($_SESSION['errors']); // Очищаем ошибки после использования
        unset($_SESSION['success_message']); // Очищаем сообщение об успехе после использования

        $page = 'FormPage.php';
        include __DIR__ . "/../views/Main.php";
    }

    public function createUser()
    {
        $errors = [];

        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $name = $_POST['name'] ?? '';
            $email = $_POST['email'] ?? '';
            $message = $_POST['message'] ?? '';

            // Валидация имени
            if (empty($name)) {
                $errors['name'] = 'Пожалуйста, введите ваше имя.';
            }

            // Валидация email
            if (empty($email)) {
                $errors['email'] = 'Пожалуйста, введите ваш email.';
            } elseif (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
                $errors['email'] = 'Пожалуйста, введите корректный email.';
            }

            // Валидация сообщения
            if (empty($message)) {
                $errors['message'] = 'Пожалуйста, введите ваше сообщение.';
            }

            // Если ошибок нет, создаем пользователя
            if (empty($errors)) {
                $host = 'mysql';
                $username = 'catshredia';
                $password = 'password';
                $database = 'php_db';

                $mysqli = new mysqli($host, $username, $password, $database);

                if ($mysqli->connect_errno) {
                    echo "Не удалось подключиться к MySQL: " . $mysqli->connect_error;
                    exit();
                }

                $stmt = $mysqli->prepare("INSERT INTO users (name, email, message, created_at, updated_at) VALUES (?, ?, ?, NOW(), NOW())");
                $stmt->bind_param("sss", $name, $email, $message);

                if ($stmt->execute()) {
                    $_SESSION['success_message'] = 'Пользователь успешно создан!'; // Сохраняем сообщение об успехе
                } else {
                    $_SESSION['errors']['database'] = 'Ошибка при создании пользователя: ' . $stmt->error; // Сохраняем ошибку базы данных
                }

                $stmt->close();
                $mysqli->close();
            } else {
                $_SESSION['errors'] = $errors; // Сохраняем ошибки в сессии
            }

            // Переадресация на страницу формы
            header('Location: /form');
            exit();
        }
    }
}
