<?php
$id = isset($_GET['id']) ? (int)$_GET['id'] : 0;
if ($id <= 0) {
    http_response_code(400);
    echo '<div class="container"><p>Неверный идентификатор.</p></div>';
    return;
}

if (($_SERVER['REQUEST_METHOD'] ?? 'GET') === 'POST') {
    $stmt = $link->prepare('DELETE FROM product WHERE id_product = :id');
    $stmt->execute([':id' => $id]);
    header('Location: /?page=products');
    exit;
}

$stmt = $link->prepare('SELECT id_product, name, coust, image_url FROM product WHERE id_product = :id');
$stmt->execute([':id' => $id]);
$product = $stmt->fetch(PDO::FETCH_ASSOC);
if (!$product) {
    http_response_code(404);
    echo '<div class="container"><p>Питомец не найден.</p></div>';
    return;
}
$name = htmlspecialchars($product['name'] ?? '', ENT_QUOTES, 'UTF-8');
$price = htmlspecialchars((string)($product['coust'] ?? ''), ENT_QUOTES, 'UTF-8');
$img = htmlspecialchars($product['image_url'] ?? '', ENT_QUOTES, 'UTF-8');
$imgSrc = $img !== '' ? $img : 'images/photo-1552053831-71594a27632d.jpeg';
?>

<main class="main">
    <div class="container">
        <div class="breadcrumb">
            <a href="/?page=start" class="breadcrumb-link">Главная</a>
            <span class="breadcrumb-separator">/</span>
            <a href="/?page=products" class="breadcrumb-link">Наши питомцы</a>
            <span class="breadcrumb-separator">/</span>
            <span class="breadcrumb-current">Удалить питомца</span>
        </div>

        <div class="delete-page">
            <div class="delete-container">
                <div class="delete-header">
                    <div class="delete-icon">⚠️</div>
                    <h1 class="delete-title">Подтверждение удаления</h1>
                    <p class="delete-subtitle">Вы собираетесь удалить питомца из базы данных</p>
                </div>

                <div class="pet-info">
                    <img src="<?php echo $imgSrc; ?>" alt="<?php echo $name; ?>" class="pet-image">
                    <div class="pet-details">
                        <h2 class="pet-name"><?php echo $name; ?></h2>
                        <div class="pet-price"><?php echo $price; ?> ₽</div>
                    </div>
                </div>

                <div class="delete-warning">
                    <h3>⚠️ Внимание!</h3>
                    <ul class="warning-list">
                        <li>Это действие нельзя отменить</li>
                        <li>Все данные о питомце будут удалены навсегда</li>
                        <li>Фотографии и описания будут потеряны</li>
                        <li>Убедитесь, что вы действительно хотите удалить этого питомца</li>
                    </ul>
                </div>

                <form method="post" action="/?page=delete-pet&id=<?php echo (int)$product['id_product']; ?>" class="delete-actions">
                    <a href="/?page=product-detail&id=<?php echo (int)$product['id_product']; ?>" class="btn btn-secondary">Отмена</a>
                    <button type="submit" class="btn btn-danger">Да, удалить питомца</button>
                </form>
            </div>
        </div>
    </div>
    </main>