<?php

require __DIR__ . '/system_components/Logger.php';
require __DIR__ . '/controllers/Controller.php';
require __DIR__ . '/controllers/HomePageController.php';
require __DIR__ . '/controllers/FormPageController.php';
require __DIR__ . '/controllers/DBController.php';
require __DIR__ . '/controllers/SecondDiplomeController.php';

use SystemComponents\Logger;

Logger::init();

session_start();

$uri = $_SERVER['REQUEST_URI'];
$uri = strtok($uri, '?');
$uri = trim($uri, '/');

if (empty($uri)) {
    $uri = '/';
}

RedirectTo($uri);

function RedirectTo($uri)
{
    Logger::route($uri);
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
        case 'second-diplome':
            $secondDiplomeController = new SecondDiplomeController();

            $secondDiplomeController->index();
            break;
        case 'second-diplome/form-city':
            $secondDiplomeController = new SecondDiplomeController();
            $secondDiplomeController->city();

            break;
        case 'second-diplome/form-day':
            $secondDiplomeController = new SecondDiplomeController();
            $secondDiplomeController->day();

            break;
    }
}
