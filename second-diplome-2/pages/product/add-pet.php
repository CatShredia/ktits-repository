<?php
// Handle create product (POST)
if (($_SERVER['REQUEST_METHOD'] ?? 'GET') === 'POST') {
    $name = trim($_POST['name'] ?? '');
    $coust = trim($_POST['coust'] ?? '');
    $imageUrl = trim($_POST['image_url'] ?? '');

    if ($name !== '' && $coust !== '') {
        $stmt = $link->prepare('INSERT INTO product (name, coust, image_url) VALUES (:name, :coust, :image_url)');
        $stmt->execute([
            ':name' => $name,
            ':coust' => $coust,
            ':image_url' => $imageUrl !== '' ? $imageUrl : null,
        ]);
        header('Location: /?page=products');
        exit;
    }
}
?>

<main class="main">
    <div class="container">
        <div class="breadcrumb">
            <a href="/?page=start" class="breadcrumb-link">Главная</a>
            <span class="breadcrumb-separator">/</span>
            <a href="/?page=products" class="breadcrumb-link">Наши питомцы</a>
            <span class="breadcrumb-separator">/</span>
            <span class="breadcrumb-current">Добавить питомца</span>
        </div>

        <div class="form-page">
            <div class="form-container">
                <div class="form-header">
                    <h1 class="form-title">Добавить нового питомца</h1>
                    <p class="form-subtitle">Заполните информацию о новом питомце</p>
                </div>

                <form class="pet-form" method="post" action="/?page=add-pet">
                    <div class="form-group">
                        <label for="petName" class="form-label">Имя питомца</label>
                        <input type="text" id="petName" name="name" class="form-input" placeholder="Введите имя питомца" required>
                    </div>

                    <div class="form-group">
                        <label for="petImageUrl" class="form-label">URL изображения</label>
                        <input type="url" id="petImageUrl" name="image_url" class="form-input" placeholder="https://...">
                        <small class="form-hint">Вставьте ссылку на изображение питомца</small>
                    </div>

                    <div class="form-group">
                        <label for="petPrice" class="form-label">Цена (руб.)</label>
                        <input type="number" id="petPrice" name="coust" class="form-input" min="0" placeholder="0" required>
                    </div>

                    <div class="form-actions">
                        <a href="/?page=products" class="btn btn-secondary">Отмена</a>
                        <button type="submit" class="btn btn-primary">Добавить питомца</button>
                    </div>
                </form>
            </div>

            <div class="form-image">
                <img src="images/photo-1552053831-71594a27632d.jpeg" alt="Новый питомец">
            </div>
        </div>
    </div>
    </main>