import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../database/models/product.dart';
import '../database/services/productservice.dart';

class ProductPage extends StatefulWidget {
  final String productId;

  const ProductPage({super.key, required this.productId});

  @override
  State<ProductPage> createState() => _ProductPageState();
}

class _ProductPageState extends State<ProductPage> {
  final ProductService _productService = ProductService();
  Product? _product;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadProduct();
  }

  Future<void> _loadProduct() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final product = await _productService.getProductById(widget.productId);
      if (product == null) {
        setState(() {
          _error = 'Товар не найден';
          _isLoading = false;
        });
      } else {
        setState(() {
          _product = product;
          _isLoading = false;
        });
      }
    } catch (e) {
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  String _formatPrice(int priceCent, String currency) {
    final formatter = NumberFormat('#,##0', 'ru_RU');
    return '${formatter.format(priceCent / 100)} $currency';
  }

  String _formatDate(DateTime date) {
    return DateFormat('dd.MM.yyyy HH:mm', 'ru_RU').format(date);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Товар'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadProduct,
            tooltip: 'Обновить',
          ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    }

    if (_error != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.error_outline,
              size: 64,
              color: Colors.red[300],
            ),
            const SizedBox(height: 16),
            Text(
              _error!,
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.red[700],
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: _loadProduct,
              icon: const Icon(Icons.refresh),
              label: const Text('Попробовать снова'),
            ),
          ],
        ),
      );
    }

    if (_product == null) {
      return const Center(
        child: Text('Товар не найден'),
      );
    }

    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Изображение товара
          _buildProductImage(),
          // Информация о товаре
          _buildProductInfo(),
        ],
      ),
    );
  }

  Widget _buildProductImage() {
    final hasImage = _product!.image != null && _product!.image!.isNotEmpty;

    return Container(
      width: double.infinity,
      height: 300,
      color: Colors.grey[200],
      child: hasImage
          ? CachedNetworkImage(
              imageUrl: _product!.image!,
              fit: BoxFit.cover,
              placeholder: (context, url) => const Center(
                child: CircularProgressIndicator(),
              ),
              errorWidget: (context, url, error) => Center(
                child: Icon(
                  Icons.image_not_supported_outlined,
                  size: 64,
                  color: Colors.grey[400],
                ),
              ),
            )
          : Center(
              child: Icon(
                Icons.image_outlined,
                size: 64,
                color: Colors.grey[400],
              ),
            ),
    );
  }

  Widget _buildProductInfo() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Название
          Text(
            _product!.name,
            style: const TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          // Цена
          Row(
            children: [
              Text(
                _formatPrice(_product!.priceCent, _product!.currency),
                style: const TextStyle(
                  fontSize: 28,
                  fontWeight: FontWeight.bold,
                  color: Colors.orange,
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          // Статус
          _buildStatusChip(),
          const SizedBox(height: 16),
          // Описание
          if (_product!.description != null && _product!.description!.isNotEmpty)
            _buildDescriptionSection(),
          const SizedBox(height: 16),
          // Информация о наличии
          _buildStockInfo(),
          const SizedBox(height: 16),
          // Дата создания
          _buildCreatedAtInfo(),
          const SizedBox(height: 16),
          // Дополнительная информация
          _buildAdditionalInfo(),
        ],
      ),
    );
  }

  Widget _buildStatusChip() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: _product!.isActive ? Colors.green[100] : Colors.red[100],
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: _product!.isActive ? Colors.green : Colors.red,
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            _product!.isActive ? Icons.check_circle : Icons.cancel,
            size: 16,
            color: _product!.isActive ? Colors.green[700] : Colors.red[700],
          ),
          const SizedBox(width: 6),
          Text(
            _product!.isActive ? 'Активен' : 'Неактивен',
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w600,
              color: _product!.isActive ? Colors.green[700] : Colors.red[700],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDescriptionSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Описание',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: Colors.grey[100],
            borderRadius: BorderRadius.circular(8),
          ),
          child: Text(
            _product!.description!,
            style: TextStyle(
              fontSize: 15,
              height: 1.5,
              color: Colors.grey[800],
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildStockInfo() {
    return Row(
      children: [
        Icon(
          Icons.inventory_2_outlined,
          size: 24,
          color: Colors.blue[600],
        ),
        const SizedBox(width: 8),
        Text(
          'В наличии: ${_product!.stock} шт.',
          style: const TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.w500,
          ),
        ),
      ],
    );
  }

  Widget _buildCreatedAtInfo() {
    return Row(
      children: [
        Icon(
          Icons.calendar_today_outlined,
          size: 20,
          color: Colors.grey[600],
        ),
        const SizedBox(width: 8),
        Text(
          'Добавлен: ${_formatDate(_product!.createdAt)}',
          style: TextStyle(
            fontSize: 14,
            color: Colors.grey[600],
          ),
        ),
      ],
    );
  }

  Widget _buildAdditionalInfo() {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.grey[100],
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: Colors.grey[300]!),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Дополнительная информация',
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          _buildInfoRow('ID товара', _product!.id),
          const SizedBox(height: 8),
          _buildInfoRow(
            'Продавец',
            _product!.userLink ?? 'Не указан',
          ),
          const SizedBox(height: 8),
          _buildInfoRow(
            'Категория',
            _product!.categoryLink ?? 'Не указана',
          ),
        ],
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 100,
          child: Text(
            '$label:',
            style: TextStyle(
              fontSize: 14,
              color: Colors.grey[600],
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
        Expanded(
          child: Text(
            value,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
      ],
    );
  }
}
