<?php

class SecondDiplomeController extends Controller
{
    public function index()
    {
        $page = 'SecondDiplome.php';

        include __DIR__ . "/../views/Main.php";
    }

    public function city()
    {
        $page = 'FormSity.php';

        include __DIR__ . "/../views/Main.php";
    }

    public function day()
    {
        $page = 'FormDay.php';

        include __DIR__ . "/../views/Main.php";
    }
}