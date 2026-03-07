import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:supabase_flutter/supabase_flutter.dart' as supabase_flutter;
import '../database/services/userservice.dart';
import '../database/models/user.dart' as app_models;

class ProfileTab extends StatefulWidget {
  const ProfileTab({super.key});

  @override
  State<ProfileTab> createState() => _ProfileTabState();
}

class _ProfileTabState extends State<ProfileTab> {
  final UserService _userService = UserService();
  final ImagePicker _imagePicker = ImagePicker();
  app_models.User? _profileUser;
  supabase_flutter.User? _authUser;
  bool _isLoading = true;
  bool _isUploading = false;

  @override
  void initState() {
    super.initState();
    _loadUserProfile();
  }

  Future<void> _loadUserProfile() async {
    setState(() => _isLoading = true);

    try {
      _authUser = _userService.getCurrentUser();
      if (_authUser != null) {
        _profileUser = await _userService.getUserProfile(_authUser!.id);
      }
    } catch (e) {
      print('Ошибка загрузки профиля: $e');
    }

    if (mounted) setState(() => _isLoading = false);
  }

  Future<void> _logout() async {
    await _userService.signOut();
    if (mounted) {
      Navigator.pushNamedAndRemoveUntil(context, '/', (route) => false);
    }
  }

  Future<void> _pickAndUploadImage() async {
    // модальное окно для выбора источника изображения (камера/галерея/отмена)
    final source = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Выберите источник'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, 'camera'),
            child: const Text('Камера'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, 'gallery'),
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

    final XFile? pickedFile = source == 'camera'
        ? await _imagePicker.pickImage(source: ImageSource.camera)
        : await _imagePicker.pickImage(source: ImageSource.gallery);

    if (pickedFile == null || _authUser == null) return;

    setState(() => _isUploading = true);

    try {
      final imageBytes = await pickedFile.readAsBytes();
      final fileName = 'avatar_${DateTime.now().millisecondsSinceEpoch}.jpg';

      print('Загрузка аватара для пользователя: ${_authUser!.id}');
      print('Имя файла: $fileName');

      final avatarUrl = await _userService.uploadAvatar(
        _authUser!.id,
        imageBytes,
        fileName,
      );

      print('Получен URL: $avatarUrl');

      await _userService.updateUserAvatar(_authUser!.id, avatarUrl);
      print('Avatar URL обновлён в БД');

      // Перезагружаем профиль из БД для корректного обновления
      final updatedProfile = await _userService.getUserProfile(_authUser!.id);

      if (mounted) {
        setState(() {
          _profileUser = updatedProfile;
        });

        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Аватар успешно загружен')),
        );
      }
    } catch (e) {
      print('Ошибка загрузки изображения: $e');
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Ошибка при загрузке аватара: $e')),
        );
      }
    } finally {
      if (mounted) setState(() => _isUploading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    final displayName =
        _profileUser?.name ?? _authUser?.email ?? 'Пользователь';
    final email = _authUser?.email ?? '';
    final avatarUrl =
        _profileUser?.avatarUrl ??
        'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSS8AHKl3Z8TjEYX07ul7F8Q2Cx2OJZRj7Z5Q&s';

    return Scaffold(
      body: SingleChildScrollView(
        child: Column(
          children: [
            Stack(
              alignment: Alignment.center,
              children: [
                SizedBox(
                  height: MediaQuery.of(context).size.height * 0.3,
                  width: MediaQuery.of(context).size.width * 0.4,
                  child: CircleAvatar(
                    backgroundImage: NetworkImage(avatarUrl),
                    radius: 60,
                  ),
                ),
                if (_isUploading)
                  const CircleAvatar(
                    radius: 30,
                    child: CircularProgressIndicator(),
                  )
                else
                  Positioned(
                    bottom: 0,
                    right: 0,
                    child: InkWell(
                      onTap: _pickAndUploadImage,
                      child: Container(
                        padding: const EdgeInsets.all(8),
                        decoration: const BoxDecoration(
                          color: Colors.blue,
                          shape: BoxShape.circle,
                        ),
                        child: const Icon(
                          Icons.camera_alt,
                          color: Colors.white,
                          size: 20,
                        ),
                      ),
                    ),
                  ),
              ],
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.02),
            Text(
              displayName,
              style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.01),
            Text(
              email,
              style: TextStyle(fontSize: 14, color: Colors.grey[600]),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.01),
            InkWell(
              onTap: () {
                Navigator.pushNamed(context, '/profile-edit');
              },
              child: const Text(
                'Редактирование',
                style: TextStyle(color: Colors.blue),
              ),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.04),
            Container(
              alignment: Alignment.topLeft,
              padding: const EdgeInsets.fromLTRB(35, 10, 10, 10),
              child: const Text(
                'Настройки',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
            ),
            SizedBox(
              width: MediaQuery.of(context).size.width * 0.9,
              child: Card(
                child: Column(
                  children: [
                    ListTile(
                      leading: const Icon(Icons.security),
                      title: const Text('Безопасность'),
                      onTap: () {
                        // Переход к настройкам безопасности
                      },
                    ),
                    ListTile(
                      leading: const Icon(Icons.notifications),
                      title: const Text('Уведомления'),
                      onTap: () {
                        // Переход к настройкам уведомлений
                      },
                    ),
                    ListTile(
                      leading: const Icon(Icons.logout, color: Colors.red),
                      title: const Text(
                        'Выйти',
                        style: TextStyle(color: Colors.red),
                      ),
                      onTap: _logout,
                    ),
                  ],
                ),
              ),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.03),
            SizedBox(
              width: MediaQuery.of(context).size.width * 0.8,
              child: ElevatedButton(
                onPressed: () {
                  // Разместить объявление
                },
                style: ButtonStyle(
                  shape: WidgetStatePropertyAll(
                    RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(25),
                    ),
                  ),
                  backgroundColor: const WidgetStatePropertyAll(Colors.orange),
                ),
                child: const Text(
                  'Разместить объявление',
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.02),
          ],
        ),
      ),
    );
  }
}
