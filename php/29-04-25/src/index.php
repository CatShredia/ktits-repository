<?php

require __DIR__ . '/system_components/Logger.php';
require __DIR__ . '/controllers/Controller.php';
require __DIR__ . '/controllers/HomePageController.php';
require __DIR__ . '/controllers/FormPageController.php';
require __DIR__ . '/controllers/DBController.php';
require __DIR__ . '/controllers/SecondDiplomeController.php';
require __DIR__ . '/route.php';

use SystemComponents\Logger;

Logger::init();

session_start();

handleRequest();
