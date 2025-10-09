<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Бобик - Дом для хвостиков</title>
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
                <span class="breadcrumb-current">Бобик</span>
            </div>

            <div class="product-detail">
                <div class="product-gallery">
                    <div class="main-image">
                        <img src="images/photo-1587300003388-59208cc962cb.jpeg" alt="Бобик">
                    </div>
                </div>

                <div class="product-info">
                    <div class="product-header">
                        <h1 class="product-title">Бобик</h1>
                        <div class="product-price-large">15 000 ₽</div>
                    </div>

                    <div class="product-description">
                        <p>Добрый и игривый лабрадор, который обожает детей и активные игры. Очень умный и легко обучается командам. Бобик любит плавать и гулять на свежем воздухе. Он приучен к поводку и хорошо ладит с другими собаками.</p>
                    </div>

                    <div class="product-actions">
                        <a href="products.html" class="btn btn-secondary">← Назад к питомцам</a>
                        <a href="edit-pet.html" class="btn btn-primary">✏️ Редактировать</a>
                        <a href="delete-pet.html" class="btn btn-danger">🗑️ Удалить</a>
                    </div>
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