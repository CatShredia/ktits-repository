<?php
$id = isset($_GET['id']) ? (int)$_GET['id'] : 0;
if ($id <= 0) {
    http_response_code(400);
    echo '<div class="container"><p>Неверный идентификатор.</p></div>';
    return;
}

if (($_SERVER['REQUEST_METHOD'] ?? 'GET') === 'POST') {
    $name = trim($_POST['name'] ?? '');
    $coust = trim($_POST['coust'] ?? '');
    $imageUrl = trim($_POST['image_url'] ?? '');
    if ($name !== '' && $coust !== '') {
        $stmt = $link->prepare('UPDATE product SET name = :name, coust = :coust, image_url = :image_url WHERE id_product = :id');
        $stmt->execute([
            ':name' => $name,
            ':coust' => $coust,
            ':image_url' => $imageUrl !== '' ? $imageUrl : null,
            ':id' => $id,
        ]);
        header('Location: /?page=product-detail&id=' . $id);
        exit;
    }
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
?>

<main class="main">
    <div class="container">
        <div class="form-page">
            <div class="form-container">
                <div class="form-header">
                    <h1 class="form-title">Редактировать питомца</h1>
                    <p class="form-subtitle">Измените информацию о питомце</p>
                </div>

                <form class="pet-form" method="post" action="/?page=edit-pet&id=<?php echo (int)$product['id_product']; ?>">
                    <div class="form-group">
                        <label for="petName" class="form-label">Имя питомца</label>
                        <input type="text" id="petName" name="name" class="form-input" value="<?php echo $name; ?>" required>
                    </div>

                    <div class="form-group">
                        <label for="petImageUrl" class="form-label">URL изображения</label>
                        <input type="url" id="petImageUrl" name="image_url" class="form-input" value="<?php echo $img; ?>">
                        <small class="form-hint">Вставьте ссылку на изображение питомца</small>
                    </div>

                    <div class="form-group">
                        <label for="petPrice" class="form-label">Цена (руб.)</label>
                        <input type="number" id="petPrice" name="coust" class="form-input" value="<?php echo $price; ?>" min="0" required>
                    </div>

                    <div class="form-actions">
                        <a href="/?page=product-detail&id=<?php echo (int)$product['id_product']; ?>" class="btn btn-secondary">Отмена</a>
                        <button type="submit" class="btn btn-primary">Сохранить изменения</button>
                    </div>
                </form>
            </div>

            <div class="form-image">
                <img src="<?php echo $img !== '' ? $img : 'images/photo-1552053831-71594a27632d.jpeg'; ?>" alt="<?php echo $name; ?>">
            </div>
        </div>
    </div>
    </main>