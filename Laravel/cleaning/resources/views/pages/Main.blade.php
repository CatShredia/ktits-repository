@extends('default')

@section('content')
    <!-- Hero Section -->
    <section id="home" class="relative h-[70vh] flex flex-col justify-center items-center text-center text-white" style="background-image: url('{{ asset('storage/images/i.png') }}');                 background-size: cover;
                        background-position: center;
                        background-repeat: no-repeat;" class=" bg-cover bg-center bg-no-repeat">
        <div class="relative z-10 px-4 max-w-3xl">
            <h1 class="text-4xl md:text-5xl font-bold mb-4">Профессиональный клининг зданий</h1>
            <p class="text-lg md:text-xl mb-8">Чистота, надежность и внимание к деталям — наш приоритет.</p>
            <a href="#contacts"
                class="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-6 rounded inline-block transition">
                Заказать услугу
            </a>
        </div>
    </section>

    <!-- Services Section -->
    <section id="services" class="py-16 px-4 max-w-6xl mx-auto">
        <h2 class="text-3xl font-bold text-center mb-12">Наши услуги</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <div class="bg-white p-6 rounded-lg shadow-md text-center">
                <h3 class="text-xl font-semibold mb-3">Ежедневная уборка</h3>
                <p>Поддержание чистоты офисов и помещений каждый день.</p>
            </div>
            <div class="bg-white p-6 rounded-lg shadow-md text-center">
                <h3 class="text-xl font-semibold mb-3">Генеральная уборка</h3>
                <p>Глубокая очистка всех поверхностей после ремонта или перед сдачей.</p>
            </div>
            <div class="bg-white p-6 rounded-lg shadow-md text-center">
                <h3 class="text-xl font-semibold mb-3">Мытье окон</h3>
                <p>Без разводов и пыли — идеальная чистота стекол любой сложности.</p>
            </div>
        </div>
    </section>
@endsection
