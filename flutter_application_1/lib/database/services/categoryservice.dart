import 'package:supabase_flutter/supabase_flutter.dart';
import '../models/category.dart';

class CategoryService {
  final SupabaseClient _client = Supabase.instance.client;

  /// Получить все категории
  Future<List<Category>> getAllCategories() async {
    final response = await _client
        .from('product_categories')
        .select()
        .order('created_at', ascending: false);

    return (response as List).map((item) => Category.fromJson(item)).toList();
  }

  /// Получить категорию по ID
  Future<Category?> getCategoryById(int id) async {
    try {
      final response = await _client
          .from('product_categories')
          .select()
          .eq('id', id)
          .maybeSingle();

      if (response == null) return null;

      return Category.fromJson(response);
    } catch (e) {
      print('Ошибка при получении категории: $e');
      return null;
    }
  }

  /// Создать новую категорию
  Future<Category> createCategory({
    required String name,
    String? image,
  }) async {
    final response = await _client
        .from('product_categories')
        .insert({
          'name': name,
          'image': image,
        })
        .select();

    return Category.fromJson(response.first);
  }
}
