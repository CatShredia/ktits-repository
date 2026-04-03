import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../database/models/product.dart';
import '../database/services/productservice.dart';
import '../database/services/userservice.dart';
import 'package:cached_network_image/cached_network_image.dart';

class SellTab extends StatefulWidget {
  const SellTab({super.key});

  @override
  State<SellTab> createState() => _SellTabState();
}

class _SellTabState extends State<SellTab> {
  final ProductService _productService = ProductService();
  final UserService _userService = UserService();
  late Future<List<Product>> _productsFuture;

  @override
  void initState() {
    super.initState();
    _productsFuture = _loadUserProducts();
  }

  Future<List<Product>> _loadUserProducts() async {
    final user = _userService.getCurrentUser();
    if (user == null) return [];
    return await _productService.getProductsByUser(user.id);
  }

  void _refreshProducts() {
    setState(() {
      _productsFuture = _loadUserProducts();
    });
  }

  String _formatPrice(int priceCent, String currency) {
    final formatter = NumberFormat('#,##0', 'ru_RU');
    return '${formatter.format(priceCent / 100)} $currency';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: RefreshIndicator(
        onRefresh: () async {
          _refreshProducts();
        },
        child: FutureBuilder<List<Product>>(
          future: _productsFuture,
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            }

            if (snapshot.hasError) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(Icons.error_outline, size: 64, color: Colors.red[300]),
                    const SizedBox(height: 16),
                    Text(
                      'Ошибка загрузки',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: Colors.red[700],
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      snapshot.error.toString(),
                      style: TextStyle(color: Colors.red[400]),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton.icon(
                      onPressed: _refreshProducts,
                      icon: const Icon(Icons.refresh),
                      label: const Text('Попробовать снова'),
                    ),
                  ],
                ),
              );
            }

            final products = snapshot.data ?? [];

            if (products.isEmpty) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.shopping_basket_outlined,
                      size: 64,
                      color: Colors.grey[400],
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'Нет товаров',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: Colors.grey[600],
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Здесь будут отображаться товары',
                      style: TextStyle(color: Colors.grey[500]),
                    ),
                  ],
                ),
              );
            }

            return GridView.builder(
              padding: const EdgeInsets.all(8),
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                childAspectRatio: 0.75,
                crossAxisSpacing: 8,
                mainAxisSpacing: 8,
              ),
              itemCount: products.length,
              itemBuilder: (context, index) {
                final product = products[index];
                return _buildProductCard(product);
              },
            );
          },
        ),
      ),
    );
  }

  Widget _buildProductCard(Product product) {
    return Card(
      clipBehavior: Clip.antiAlias,
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        onTap: () {
          Navigator.pushNamed(
            context,
            '/product',
            arguments: {'id': product.id},
          );
        },
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Изображение товара
            Expanded(
              child: Stack(
                fit: StackFit.expand,
                children: [
                  if (product.image != null && product.image!.isNotEmpty)
                    CachedNetworkImage(
                      imageUrl: product.image!,
                      fit: BoxFit.cover,
                      placeholder: (context, url) => Container(
                        color: Colors.grey[200],
                        child: const Center(
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                      ),
                      errorWidget: (context, url, error) => Container(
                        color: Colors.grey[200],
                        child: Icon(
                          Icons.image_not_supported_outlined,
                          color: Colors.grey[400],
                        ),
                      ),
                    )
                  else
                    Container(
                      color: Colors.grey[200],
                      child: Icon(
                        Icons.image_outlined,
                        size: 48,
                        color: Colors.grey[400],
                      ),
                    ),
                  // Бейдж активности
                  Positioned(
                    top: 8,
                    right: 8,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 4,
                      ),
                      decoration: BoxDecoration(
                        color: product.isActive ? Colors.green : Colors.red,
                        borderRadius: BorderRadius.circular(4),
                      ),
                      child: Text(
                        product.isActive ? 'Активен' : 'Неактивен',
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 10,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
            // Информация о товаре
            Padding(
              padding: const EdgeInsets.all(8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Название
                  Text(
                    product.name,
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 4),
                  // Цена
                  Text(
                    _formatPrice(product.priceCent, product.currency),
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: Colors.orange,
                    ),
                  ),
                  const SizedBox(height: 4),
                  // Остаток на складе
                  Row(
                    children: [
                      Icon(
                        Icons.inventory_2_outlined,
                        size: 14,
                        color: Colors.grey[600],
                      ),
                      const SizedBox(width: 4),
                      Text(
                        '${product.stock} шт.',
                        style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
