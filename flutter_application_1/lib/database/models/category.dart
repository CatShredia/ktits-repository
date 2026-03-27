class Category {
  final int id;
  final String name;
  final String? image;
  final DateTime createdAt;

  Category({
    required this.id,
    required this.name,
    this.image,
    required this.createdAt,
  });

  factory Category.fromJson(Map<String, Object?> json) {
    final Object? createdAtValue = json['created_at'];
    return Category(
      id: json['id'] as int,
      name: json['name'] as String,
      image: json['image'] as String?,
      createdAt: createdAtValue != null
          ? (createdAtValue is String
                ? DateTime.parse(createdAtValue)
                : DateTime.now())
          : DateTime.now(),
    );
  }

  Map<String, Object?> toJson() {
    return {
      'id': id,
      'name': name,
      'image': image,
      'created_at': createdAt.toIso8601String(),
    };
  }
}
