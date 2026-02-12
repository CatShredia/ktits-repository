<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Service extends Model
{
    protected $fillable = ['title', 'description', 'price'];

    public function reviews()
    {
        return $this->hasMany(Review::class);
    }
}