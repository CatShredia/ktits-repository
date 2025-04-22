<?php
session_start(); // Добавляем старт сессии
require __DIR__ . '/controllers/Controller.php';
require __DIR__ . '/controllers/HomePageController.php';
require __DIR__ . '/controllers/FormPageController.php';
require __DIR__ . '/controllers/DBController.php';

$uri = $_SERVER['REQUEST_URI'];
$uri = strtok($uri, '?');
$uri = trim($uri, '/');

if (empty($uri)) {
    $uri = '/';
}

RedirectTo($uri);

function RedirectTo($uri)
{
    switch ($uri) {
        case '/':
            $homeController = new HomePageController();
            $homeController->index();
            break;
        case 'form':
            $formController = new FormPageController();
            $formController->index();
            break;
        case 'form/create':
            $formController = new FormPageController();
            $formController->createUser();
            break;
        case 'db':
            $dbController = new DBController();
            $dbController->index();
            break;
    }
}
