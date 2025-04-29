<?php

class HomePageController extends Controller
{
    public function index()
    {
        $page = 'HomePage.php';

        include __DIR__ . "/../views/Main.php";
    }
}
