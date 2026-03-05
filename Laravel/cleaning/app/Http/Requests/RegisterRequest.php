<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class RegisterRequest extends FormRequest
{
    public function authorize(): bool
    {
        return true;
    }

    public function rules(): array
    {
        return [
            'email' => 'required|email',
            'password' => 'required|min:6',
            'password_confirmation' => 'required|same:password',
        ];
    }

    public function messages(): array
    {
        return [
            'email.required' => 'Поле Email обязательно для заполнения.',
            'email.email' => 'Поле Email должно быть действительным адресом электронной почты.',
            'password.required' => 'Поле Пароль обязательно для заполнения.',
            'password.min' => 'Поле Пароль должно содержать не менее 6 символов.',
            'password_confirmation.required' => 'Поле Подтверждение пароля обязательно для заполнения.',
            'password_confirmation.same' => 'Поле Подтверждение пароля должно совпадать с полем Пароль.',
        ];
    }

    public function attributes(): array
    {
        return [
            'email' => 'Email',
            'password' => 'Пароль',
            'password_confirmation' => 'Подтверждение пароля',
        ];
    }
}
