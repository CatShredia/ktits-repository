import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:supabase_flutter/supabase_flutter.dart';
import '../database/services/productservice.dart';
import '../database/services/categoryservice.dart';
import '../database/services/userservice.dart';
import '../database/models/category.dart';

class CreateListingPage extends StatefulWidget {
  const CreateListingPage({super.key});

  @override
  State<CreateListingPage> createState() => _CreateListingPageState();
}

class _CreateListingPageState extends State<CreateListingPage> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _priceController = TextEditingController();
  final _stockController = TextEditingController();

  final ProductService _productService = ProductService();
  final CategoryService _categoryService = CategoryService();
  final UserService _userService = UserService();
  final ImagePicker _imagePicker = ImagePicker();

  List<Category> _categories = [];
  Category? _selectedCategory;
  XFile? _pickedImage;
  bool _isLoading = false;
  bool _loadingCategories = true;

  @override
  void initState() {
    super.initState();
    _loadCategories();
    _stockController.text = '1';
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    _priceController.dispose();
    _stockController.dispose();
    super.dispose();
  }

  Future<void> _loadCategories() async {
    try {
      final categories = await _categoryService.getAllCategories();
      if (mounted) {
        setState(() {
          _categories = categories;
          _loadingCategories = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() => _loadingCategories = false);
      }
    }
  }

  Future<void> _pickImage() async {
    final source = await showDialog<ImageSource>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Выберите источник'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, ImageSource.camera),
            child: const Text('Камера'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, ImageSource.gallery),
            child: const Text('Галерея'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Отмена'),
          ),
        ],
      ),
    );

    if (source == null) return;

    final image = await _imagePicker.pickImage(
      source: source,
      maxWidth: 1200,
      maxHeight: 1200,
      imageQuality: 85,
    );

    if (image != null) {
      setState(() => _pickedImage = image);
    }
  }

  Future<String?> _uploadImage() async {
    if (_pickedImage == null) return null;

    try {
      final user = _userService.getCurrentUser();
      if (user == null) return null;

      final imageBytes = await _pickedImage!.readAsBytes();
      final fileName = 'product_${DateTime.now().millisecondsSinceEpoch}.jpg';
      final path = '${user.id}/$fileName';

      await _userService.client.storage
          .from('avatars')
          .uploadBinary(
            path,
            imageBytes,
            fileOptions: FileOptions(upsert: true),
          );

      return _userService.client.storage.from('avatars').getPublicUrl(path);
    } catch (e) {
      print('Ошибка загрузки изображения: $e');
      return null;
    }
  }

  Future<void> _submitListing() async {
    if (!_formKey.currentState!.validate()) return;

    if (_selectedCategory == null) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Выберите категорию')));
      return;
    }

    setState(() => _isLoading = true);

    try {
      final user = _userService.getCurrentUser();
      if (user == null) {
        throw Exception('Пользователь не авторизован');
      }

      // Загружаем изображение, если выбрано
      String? imageUrl;
      if (_pickedImage != null) {
        imageUrl = await _uploadImage();
      }

      final priceRubles = double.tryParse(_priceController.text) ?? 0;
      final priceCent = (priceRubles * 100).round();

      await _productService.createProduct(
        name: _nameController.text.trim(),
        priceCent: priceCent,
        currency: 'RUB',
        stock: int.tryParse(_stockController.text) ?? 1,
        userId: user.id,
        categoryId: _selectedCategory!.id,
        image: imageUrl,
        description: _descriptionController.text.trim().isEmpty
            ? null
            : _descriptionController.text.trim(),
        isActive: true,
      );

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Объявление успешно создано!')),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Ошибка при создании объявления: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Новое объявление'),
        backgroundColor: Colors.white,
        foregroundColor: Colors.black,
        elevation: 1,
      ),
      body: _loadingCategories
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Image picker area
                    GestureDetector(
                      onTap: _pickImage,
                      child: Container(
                        height: 200,
                        decoration: BoxDecoration(
                          color: Colors.grey[200],
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: Colors.grey[400]!,
                            style: BorderStyle.solid,
                          ),
                        ),
                        child: _pickedImage != null
                            ? ClipRRect(
                                borderRadius: BorderRadius.circular(12),
                                child: Image.file(
                                  File(_pickedImage!.path),
                                  fit: BoxFit.cover,
                                ),
                              )
                            : Column(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  Icon(
                                    Icons.add_photo_alternate,
                                    size: 48,
                                    color: Colors.grey[600],
                                  ),
                                  const SizedBox(height: 8),
                                  Text(
                                    'Добавить фото',
                                    style: TextStyle(
                                      color: Colors.grey[600],
                                      fontSize: 16,
                                    ),
                                  ),
                                ],
                              ),
                      ),
                    ),
                    const SizedBox(height: 20),

                    // Name field
                    TextFormField(
                      controller: _nameController,
                      decoration: InputDecoration(
                        labelText: 'Название *',
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        prefixIcon: const Icon(Icons.title),
                      ),
                      validator: (value) {
                        if (value == null || value.trim().isEmpty) {
                          return 'Введите название объявления';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 16),

                    // Category dropdown
                    DropdownButtonFormField<Category>(
                      value: _selectedCategory,
                      decoration: InputDecoration(
                        labelText: 'Категория *',
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        prefixIcon: const Icon(Icons.category),
                      ),
                      items: _categories.map((category) {
                        return DropdownMenuItem(
                          value: category,
                          child: Text(category.name),
                        );
                      }).toList(),
                      onChanged: (value) {
                        setState(() => _selectedCategory = value);
                      },
                      validator: (value) {
                        if (value == null) {
                          return 'Выберите категорию';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 16),

                    // Description field
                    TextFormField(
                      controller: _descriptionController,
                      decoration: InputDecoration(
                        labelText: 'Описание',
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        prefixIcon: const Icon(Icons.description),
                        alignLabelWithHint: true,
                      ),
                      maxLines: 4,
                    ),
                    const SizedBox(height: 16),

                    // Price and stock row
                    Row(
                      children: [
                        Expanded(
                          flex: 2,
                          child: TextFormField(
                            controller: _priceController,
                            decoration: InputDecoration(
                              labelText: 'Цена (₽) *',
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                              prefixIcon: const Icon(Icons.attach_money),
                            ),
                            keyboardType: const TextInputType.numberWithOptions(
                              decimal: true,
                            ),
                            validator: (value) {
                              if (value == null || value.trim().isEmpty) {
                                return 'Введите цену';
                              }
                              if (double.tryParse(value) == null ||
                                  double.parse(value) <= 0) {
                                return 'Введите корректную цену';
                              }
                              return null;
                            },
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          flex: 1,
                          child: TextFormField(
                            controller: _stockController,
                            decoration: InputDecoration(
                              labelText: 'Кол-во',
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                              prefixIcon: const Icon(Icons.inventory_2),
                            ),
                            keyboardType: TextInputType.number,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 24),

                    // Submit button
                    ElevatedButton(
                      onPressed: _isLoading ? null : _submitListing,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.orange,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                      child: _isLoading
                          ? const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : const Text(
                              'Разместить объявление',
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                color: Colors.white,
                              ),
                            ),
                    ),
                    const SizedBox(height: 16),
                  ],
                ),
              ),
            ),
    );
  }
}
