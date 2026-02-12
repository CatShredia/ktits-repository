<?php

use App\Http\Controllers\ServicesMainController;
use Illuminate\Support\Facades\Route;

Route::get('/', [ServicesMainController::class, 'index']);