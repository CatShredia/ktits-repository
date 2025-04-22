FROM php:8.2-fpm-alpine

WORKDIR /var/www/laravel

# pdo
RUN docker-php-ext-install pdo pdo_mysql

# mysqli
RUN docker-php-ext-install pdo pdo_mysql mysqli