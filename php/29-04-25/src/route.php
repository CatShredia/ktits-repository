<?php

use SystemComponents\Logger;

function handleRequest()
{
    $uri = $_SERVER['REQUEST_URI'];
    $uri = strtok($uri, '?');
    $uri = trim($uri, '/');

    if (empty($uri)) {
        $uri = '/';
    }

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

        default:
            Logger::route($uri, Logger::LEVEL_ERROR);

            include __DIR__ . "/views/404error.php";
    }
}