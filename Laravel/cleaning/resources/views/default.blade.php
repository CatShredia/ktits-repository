<!DOCTYPE html>
<html lang="ru">

<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>CleanSpace — Профессиональный клининг</title>

    @vite(['resources/css/app.css', 'resources/js/app.js'])

</head>

<body class="font-sans bg-gray-50 text-gray-800">
    @include('parts.header')

    @yield('content')

    @include('parts.footer')
</body>

</html>