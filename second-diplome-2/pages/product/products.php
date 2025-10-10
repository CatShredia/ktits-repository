<?php
// Expect $link (PDO) from db/connection.php via index.php
// Fetch all products
try {
    $stmt = $link->query('SELECT id_product, name, coust, image_url FROM product ORDER BY id_product DESC');
    $products = $stmt->fetchAll(PDO::FETCH_ASSOC);
} catch (Throwable $e) {
    $products = [];
}
?>

<main class="main">
    <div class="container">
        <div class="products-header">
            <div class="products-title-section">
                <h1 class="page-title">Наши питомцы</h1>
                <p class="page-subtitle">Познакомьтесь с нашими четвероногими друзьями, которые ищут любящую семью</p>
            </div>
            <div class="products-actions">
                <a href="/?page=add-pet" class="btn btn-primary">
                    <span>➕</span> Добавить питомца
                </a>
            </div>
        </div>

        <div class="products-grid">
            <?php if (empty($products)): ?>
                <p>Питомцев пока нет. Добавьте первого!</p>
            <?php else: ?>
                <?php foreach ($products as $product): ?>
                    <?php
                        $id = (int)$product['id_product'];
                        $name = htmlspecialchars($product['name'] ?? '', ENT_QUOTES, 'UTF-8');
                        $price = htmlspecialchars((string)($product['coust'] ?? ''), ENT_QUOTES, 'UTF-8');
                        $img = htmlspecialchars($product['image_url'] ?? '', ENT_QUOTES, 'UTF-8');
                        $imgSrc = $img !== '' ? $img : 'images/photo-1552053831-71594a27632d.jpeg';
                    ?>
                    <div class="product-card">
                        <img src="<?php echo $imgSrc; ?>" alt="<?php echo $name; ?>" class="product-image">
                        <div class="product-content">
                            <h3 class="product-name"><?php echo $name; ?></h3>
                            <div class="product-meta">
                                <span class="product-price"><?php echo $price; ?> ₽</span>
                            </div>
                            <div class="product-actions">
                                <a href="/?page=product-detail&id=<?php echo $id; ?>" class="btn btn-primary">Подробнее</a>
                                <a href="/?page=edit-pet&id=<?php echo $id; ?>" class="btn btn-secondary">✏️ Редактировать</a>
                                <a href="/?page=delete-pet&id=<?php echo $id; ?>" class="btn btn-danger">🗑️ Удалить</a>
                            </div>
                        </div>
                    </div>
                <?php endforeach; ?>
            <?php endif; ?>
        </div>
    </div>
    </main>