<?php
// Load product by id
$id = isset($_GET['id']) ? (int)$_GET['id'] : 0;
$product = null;
if ($id > 0) {
    $stmt = $link->prepare('SELECT id_product, name, coust, image_url FROM product WHERE id_product = :id');
    $stmt->execute([':id' => $id]);
    $product = $stmt->fetch(PDO::FETCH_ASSOC) ?: null;
}
if (!$product) {
    http_response_code(404);
    echo '<div class="container"><p>Питомец не найден.</p><p><a class="btn btn-secondary" href="/?page=products">← Назад</a></p></div>';
    return;
}
$name = htmlspecialchars($product['name'] ?? '', ENT_QUOTES, 'UTF-8');
$price = htmlspecialchars((string)($product['coust'] ?? ''), ENT_QUOTES, 'UTF-8');
$img = htmlspecialchars($product['image_url'] ?? '', ENT_QUOTES, 'UTF-8');
$imgSrc = $img !== '' ? $img : 'images/photo-1587300003388-59208cc962cb.jpeg';
?>

<main class="main">
    <div class="container">
        <div class="breadcrumb">
            <a href="/?page=start" class="breadcrumb-link">Главная</a>
            <span class="breadcrumb-separator">/</span>
            <a href="/?page=products" class="breadcrumb-link">Наши питомцы</a>
            <span class="breadcrumb-separator">/</span>
            <span class="breadcrumb-current"><?php echo $name; ?></span>
        </div>

        <div class="product-detail">
            <div class="product-gallery">
                <div class="main-image">
                    <img src="<?php echo $imgSrc; ?>" alt="<?php echo $name; ?>">
                </div>
            </div>

            <div class="product-info">
                <div class="product-header">
                    <h1 class="product-title"><?php echo $name; ?></h1>
                    <div class="product-price-large"><?php echo $price; ?> ₽</div>
                </div>

                <div class="product-actions">
                    <a href="/?page=products" class="btn btn-secondary">← Назад к питомцам</a>
                    <a href="/?page=edit-pet&id=<?php echo (int)$product['id_product']; ?>" class="btn btn-primary">✏️ Редактировать</a>
                    <a href="/?page=delete-pet&id=<?php echo (int)$product['id_product']; ?>" class="btn btn-danger">🗑️ Удалить</a>
                </div>
            </div>
        </div>
    </div>
    </main>