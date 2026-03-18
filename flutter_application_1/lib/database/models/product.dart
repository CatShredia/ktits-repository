class Product {
  final String id;
  final String? userId;
  final int? categoryId;
  final String name;
  final String? image;
  final String? description;
  final int priceCent;
  final String currency;
  final int stock;
  final bool isActive;
  final DateTime createdAt;

  Product({
    required this.id,
    this.userId,
    this.categoryId,
    required this.name,
    this.image,
    this.description,
    required this.priceCent,
    required this.currency,
    required this.stock,
    required this.isActive,
    required this.createdAt,
  });

  factory Product.fromJson(Map<String, dynamic> json) {
    final idValue = json['id'];
    final createdAtValue = json['created_at'];
    return Product(
      id: idValue is int ? idValue.toString() : (idValue as String?) ?? '',
      userId: json['user_id'] as String?,
      categoryId: json['category_id'] as int?,
      name: (json['name'] as String?) ?? '',
      image: json['image'] as String?,
      description: json['description'] as String?,
      priceCent: (json['price_cents'] as int?) ?? 0,
      currency: (json['currency'] as String?) ?? 'RUB',
      stock: (json['stock'] as int?) ?? 0,
      isActive: (json['is_active'] as bool?) ?? false,
      createdAt: createdAtValue != null
          ? (createdAtValue is String
              ? DateTime.parse(createdAtValue)
              : DateTime.now())
          : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'user_id': userId,
      'category_id': categoryId,
      'name': name,
      'image': image,
      'description': description,
      'price_cents': priceCent,
      'currency': currency,
      'stock': stock,
      'is_active': isActive,
      'created_at': createdAt.toIso8601String(),
    };
  }
}
