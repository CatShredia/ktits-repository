import 'dart:typed_data';
import 'package:supabase_flutter/supabase_flutter.dart' as supabase_flutter;
import 'package:supabase_flutter/supabase_flutter.dart';
import '../models/user.dart' as app_models;

class UserService {
  final SupabaseClient _client = Supabase.instance.client;
  static const String _avatarBucket = 'avatars';

  // Авторизация
  Future<supabase_flutter.AuthResponse> signIn(
    String email,
    String password,
  ) async {
    return await _client.auth.signInWithPassword(
      email: email,
      password: password,
    );
  }

  // Регистрация
  Future<supabase_flutter.AuthResponse> signUp(
    String email,
    String password,
  ) async {
    return await _client.auth.signUp(email: email, password: password);
  }

  // Сохранение данных пользователя в БД
  Future<app_models.User> createUserProfile({
    required String id,
    required String email,
    required String password,
  }) async {
    final response = await _client.from('users').insert({
      'id': id,
      'email': email,
      'password': password,
    }).select();

    final data = response.first;
    return app_models.User.fromJson(data);
  }

  // Выход
  Future<void> signOut() async {
    await _client.auth.signOut();
  }

  // Текущий пользователь
  supabase_flutter.User? getCurrentUser() {
    return _client.auth.currentUser;
  }

  // Получение профиля пользователя из БД
  Future<app_models.User?> getUserProfile(String userId) async {
    final response = await _client
        .from('users')
        .select()
        .eq('id', userId)
        .single();

    return app_models.User.fromJson(response);
  }

  // Сброс пароля
  Future<void> resetPassword(String email) async {
    await _client.auth.resetPasswordForEmail(email);
  }

  // Загрузка аватара в Supabase Storage
  Future<String> uploadAvatar(
    String userId,
    Uint8List imageBytes,
    String fileName,
  ) async {
    await _client.storage
        .from('avatars')
        .uploadBinary(
          '$userId/$fileName',
          imageBytes,
          fileOptions: const FileOptions(upsert: true),
        );

    // Получаем публичный URL
    final publicUrl = _client.storage
        .from('avatars')
        .getPublicUrl('$userId/$fileName');

    return publicUrl;
  }

  // Обновление URL аватара в базе данных
  Future<void> updateUserAvatar(String userId, String avatarUrl) async {
    await _client.from('users').update({'avatar': avatarUrl}).eq('id', userId);
  }
}
