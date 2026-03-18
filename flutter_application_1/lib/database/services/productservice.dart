import 'package:supabase_flutter/supabase_flutter.dart';
import '../models/product.dart';

class ProductService {
  final SupabaseClient _client = Supabase.instance.client;

  /// Получить все продукты
  Future<List<Product>> getAllProducts() async {
    final response = await _client
        .from('products')
        .select()
        .order('created_at', ascending: false);

    return (response as List).map((item) => Product.fromJson(item)).toList();
  }

  /// Получить активные продукты
  Future<List<Product>> getActiveProducts() async {
    final response = await _client
        .from('products')
        .select()
        .eq('is_active', true)
        .order('created_at', ascending: false);

    return (response as List).map((item) => Product.fromJson(item)).toList();
  }

  /// Получить продукт по ID
  Future<Product?> getProductById(String id) async {
    try {
      // Парсим ID в int, так как в БД поле id имеет тип bigint
      final intId = int.tryParse(id);
      if (intId == null) {
        print('Ошибка: не удалось преобразовать ID "$id" в int');
        return null;
      }

      final response = await _client
          .from('products')
          .select()
          .eq('id', intId)
          .maybeSingle();

      if (response == null) {
        return null;
      }

      return Product.fromJson(response);
    } catch (e, stackTrace) {
      print('Ошибка при получении продукта: $e');
      print('Stack trace: $stackTrace');
      return null;
    }
  }

  /// Получить продукты пользователя
  Future<List<Product>> getProductsByUser(String userId) async {
    final response = await _client
        .from('products')
        .select()
        .eq('user_id', userId)
        .order('created_at', ascending: false);

    return (response as List).map((item) => Product.fromJson(item)).toList();
  }

  /// Получить продукты по категории
  Future<List<Product>> getProductsByCategory(int categoryId) async {
    final response = await _client
        .from('products')
        .select()
        .eq('category_id', categoryId)
        .order('created_at', ascending: false);

    return (response as List).map((item) => Product.fromJson(item)).toList();
  }

  /// Создать новый продукт
  Future<Product> createProduct({
    required String name,
    required int priceCent,
    required String currency,
    required int stock,
    String? userId,
    int? categoryId,
    String? image,
    String? description,
    bool isActive = true,
  }) async {
    final response = await _client.from('products').insert({
      'name': name,
      'price_cents': priceCent,
      'currency': currency,
      'stock': stock,
      'user_id': userId,
      'category_id': categoryId,
      'image': image,
      'description': description,
      'is_active': isActive,
    }).select();

    return Product.fromJson(response.first);
  }

  /// Обновить продукт
  Future<Product?> updateProduct({
    required String id,
    String? name,
    int? priceCent,
    String? currency,
    int? stock,
    String? image,
    String? description,
    bool? isActive,
  }) async {
    final Map<String, dynamic> updateData = {};
    if (name != null) updateData['name'] = name;
    if (priceCent != null) updateData['price_cent'] = priceCent;
    if (currency != null) updateData['currency'] = currency;
    if (stock != null) updateData['stock'] = stock;
    if (image != null) updateData['image'] = image;
    if (description != null) updateData['description'] = description;
    if (isActive != null) updateData['is_active'] = isActive;

    if (updateData.isEmpty) return null;

    final response = await _client
        .from('products')
        .update(updateData)
        .eq('id', int.parse(id))
        .select()
        .single();

    return Product.fromJson(response);
  }

  /// Удалить продукт
  Future<void> deleteProduct(String id) async {
    await _client.from('products').delete().eq('id', int.parse(id));
  }

  /// Поиск продуктов по названию
  Future<List<Product>> searchProducts(String query) async {
    final response = await _client
        .from('products')
        .select()
        .ilike('name', '%$query%')
        .eq('is_active', true)
        .order('created_at', ascending: false);

    return (response as List).map((item) => Product.fromJson(item)).toList();
  }
}
