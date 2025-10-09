<!DOCTYPE html>
<html lang="ru">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Дом для хвостиков - Главная</title>
    <link rel="stylesheet" href="styles.css">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
</head>

<body>

    <?php
    require_once 'db/connection.php';

    // Теперь можно использовать $link
    $stmt = $link->query('SELECT * FROM product');
    $pets = $stmt->fetchAll(PDO::FETCH_ASSOC);

    print_r($pets);

    $page = $_GET['page'] ?? 'start';

    $pages = [
        'start' => 'pages/start.php',
        'products' => 'pages/product/products.php',
        'product-detail' => 'pages/product/product-detail.php',
        'add-pet' => 'pages/product/add-pet.php',
        'edit-pet' => 'pages/product/edit-pet.php',
        'delete-pet' => 'pages/product/delete-pet.php',
    ];

    if (!isset($pages[$page])) {
        http_response_code(404);
        die('Страница не найдена');
    }

    include('components/header.php');
    include($pages[$page]);
    include('components/footer.php');
    ?>


</body>

</html>