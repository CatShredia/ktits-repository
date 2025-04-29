<!DOCTYPE html>
<html lang="ru">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Простая Страница</title>
    <link rel="stylesheet" href="/rec/styles.php">
</head>

<body>
    <header>
        <h1>Добро пожаловать на мою страницу!</h1>
    </header>
    <nav>
        <ul>
            <li><a href="<?php echo htmlspecialchars('/') ?>">Главная</a></li>
            <li><a href="<?php echo htmlspecialchars('/form') ?>">Валидация</a></li>
            <li><a href="<?php echo htmlspecialchars('/db') ?>">DB</a></li>
            <li><a href="<?php echo htmlspecialchars('/second-diplome') ?>">Второй диплом</a></li>
        </ul>
    </nav>
    <?php
    include($page);
    ?>
    <footer>
        &copy; 2023 Моя Простая Страница
    </footer>
</body>

</html>