<?php
// файл нужен, чтобы при запуске контейнера migrate проводились все миграции в папке migrations 
echo "Поиск файлов миграций...\n";
$migrations = glob('migrations/*.php');

if (empty($migrations)) {
    echo "Файлы миграций не найдены.\n";
} else {
    echo "Найдены следующие файлы миграций:\n";
    print_r($migrations);
}

foreach ($migrations as $migration) {
    echo "Выполнение миграции: $migration\n";
    include $migration;
    echo "\n";
}
