<?php

namespace App\Http\Controllers;

use App\Http\Requests\RegisterRequest;

class RegisterController extends Controller
{
    public function show()
    {
        return view('pages.register');
    }

    public function register(RegisterRequest $request)
    {
        return redirect()
            ->route('register')
            ->with('success', 'Регистрация прошла успешно');
    }
}
