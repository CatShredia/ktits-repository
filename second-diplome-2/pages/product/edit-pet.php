<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Редактировать питомца - Дом для хвостиков</title>
    <link rel="stylesheet" href="styles.css">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
</head>
<body>
    <header class="header">
        <nav class="nav">
            <div class="nav-container">
                <div class="logo">
                    <img src="images/dog-house.png" alt="Логотип" class="logo-img">
                    <span class="logo-text">Дом для хвостиков</span>
                </div>
                <ul class="nav-menu">
                    <li><a href="index.html" class="nav-link">Главная</a></li>
                    <li><a href="products.html" class="nav-link">Наши питомцы</a></li>
                    <li><a href="login.html" class="nav-link">Войти</a></li>
                    <li><a href="register.html" class="nav-link">Регистрация</a></li>
                    <li><a href="login.html" class="nav-link logout">Выйти</a></li>
                </ul>
            </div>
        </nav>
    </header>

    <main class="main">
        <div class="container">
            <div class="breadcrumb">
                <a href="index.html" class="breadcrumb-link">Главная</a>
                <span class="breadcrumb-separator">/</span>
                <a href="products.html" class="breadcrumb-link">Наши питомцы</a>
                <span class="breadcrumb-separator">/</span>
                <span class="breadcrumb-current">Редактировать питомца</span>
            </div>

            <div class="form-page">
                <div class="form-container">
                    <div class="form-header">
                        <h1 class="form-title">Редактировать питомца</h1>
                        <p class="form-subtitle">Измените информацию о питомце</p>
                    </div>

                    <form class="pet-form">
                        <div class="form-group">
                            <label for="petName" class="form-label">Имя питомца</label>
                            <input type="text" id="petName" name="name" class="form-input" value="Бобик">
                        </div>

                        <div class="form-group">
                            <label for="petImage" class="form-label">Фото питомца</label>
                            <input type="file" id="petImage" name="image" class="form-input">
                            <small class="form-hint">Вставьте ссылку на изображение питомца</small>
                        </div>

                        <div class="form-group">
                            <label for="petDescription" class="form-label">Описание</label>
                            <textarea id="petDescription" name="description" class="form-input" rows="6">Добрый и игривый лабрадор, который обожает детей и активные игры. Очень умный и легко обучается командам. Бобик любит плавать и гулять на свежем воздухе. Он приучен к поводку и хорошо ладит с другими собаками.</textarea>
                        </div>

                        <div class="form-group">
                            <label for="petPrice" class="form-label">Цена (руб.)</label>
                            <input type="number" id="petPrice" name="price" class="form-input" value="15000" min="0">
                        </div>

                        <div class="form-actions">
                            <a href="products.html" class="btn btn-secondary">Отмена</a>
                            <button type="submit" class="btn btn-primary">Сохранить изменения</button>
                        </div>
                    </form>
                </div>

                <div class="form-image">
                    <img src="images/photo-1552053831-71594a27632d.jpeg" alt="Бобик">
                </div>
            </div>
        </div>
    </main>

    <footer class="footer">
        <div class="container">
            <div class="footer-content">
                <div class="footer-section">
                    <h3>Дом для хвостиков</h3>
                    <p>Дарим надежду и любовь каждому питомцу</p>
                </div>
                <div class="footer-section">
                    <h4>Контакты</h4>
                    <p>📞 +7 (999) 123-45-67</p>
                    <p>📧 info@doghome.ru</p>
                    <p>📍 г. Казань, ул. Собачья, 15</p>
                </div>
                <div class="footer-section">
                    <h4>Режим работы</h4>
                    <p>Пн-Пт: 9:00 - 18:00</p>
                    <p>Сб-Вс: 10:00 - 16:00</p>
                </div>
            </div>
            <div class="footer-bottom">
                <p>&copy; 2025 Дом для хвостиков. Все права защищены.</p>
            </div>
        </div>
    </footer>
</body>
</html> 