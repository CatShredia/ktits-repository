@extends('default')

@section('content')
    <section class="py-16 px-4 max-w-md mx-auto">
        <h2 class="text-3xl font-bold text-center mb-8">Регистрация</h2>

        @if(session('success'))
            <div class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-6">
                {{ session('success') }}
            </div>
        @endif

        <form method="POST" action="{{ route('register') }}" class="bg-white p-6 rounded-lg shadow-md">
            @csrf

            <!-- Email -->
            <div class="mb-4">
                <label for="email" class="block text-gray-700 font-bold mb-2">Email</label>
                <input type="text" id="email" name="email" value="{{ old('email') }}"
                    class="w-full px-3 py-2 border @error('email') border-red-500 @enderror rounded focus:outline-none focus:border-blue-500">
                @error('email')
                    <p class="text-red-500 text-sm mt-1">{{ $message }}</p>
                @enderror
            </div>

            <!-- Password -->
            <div class="mb-4">
                <label for="password" class="block text-gray-700 font-bold mb-2">Пароль</label>
                <input type="password" id="password" name="password"
                    class="w-full px-3 py-2 border @error('password') border-red-500 @enderror rounded focus:outline-none focus:border-blue-500">
                @error('password')
                    <p class="text-red-500 text-sm mt-1">{{ $message }}</p>
                @enderror
            </div>

            <!-- Confirmation -->
            <div class="mb-4">
                <label for="password_confirmation" class="block text-gray-700 font-bold mb-2">Подтверждение пароля</label>
                <input type="password" id="password_confirmation" name="password_confirmation"
                    class="w-full px-3 py-2 border @error('password_confirmation') border-red-500 @enderror rounded focus:outline-none focus:border-blue-500">
                @error('password_confirmation')
                    <p class="text-red-500 text-sm mt-1">{{ $message }}</p>
                @enderror
            </div>

            <div class="mb-4">
                <button type="submit"
                    class="w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded transition">
                    Зарегистрироваться
                </button>
            </div>
        </form>
    </section>
@endsection
