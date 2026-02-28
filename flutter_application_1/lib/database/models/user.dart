class User {
  final String id;
  final String email;
  final String? password;
  final DateTime? createdAt;

  User({
    required this.id,
    required this.email,
    this.password,
    this.createdAt,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] as String,
      email: json['email'] as String,
      password: json['password'] as String?,
      createdAt: json['created_at'] != null
          ? DateTime.parse(json['created_at'] as String)
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'email': email,
      'password': password,
      if (createdAt != null) 'created_at': createdAt!.toIso8601String(),
    };
  }
}
