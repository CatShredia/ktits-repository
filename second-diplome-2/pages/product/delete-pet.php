<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Удалить питомца - Дом для хвостиков</title>
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
                        <img src="images/photo-1552053831-71594a27632d.jpeg" alt="Бобик" class="pet-image">
                        <div class="pet-details">
                            <h2 class="pet-name">Бобик</h2>
                            <p class="pet-description">Добрый и игривый лабрадор, который обожает детей и активные игры. Очень умный и легко обучается командам.</p>
                            <div class="pet-price">15 000 ₽</div>
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

                    <div class="delete-actions">
                        <a href="products.html" class="btn btn-secondary">Отмена</a>
                        <a href="products.html" class="btn btn-danger">Да, удалить питомца</a>
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