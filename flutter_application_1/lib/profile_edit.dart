// lib/profile_edit.dart
import 'package:flutter/material.dart';
import 'database/services/userservice.dart';
import 'database/models/user.dart';

class ProfileEdit extends StatefulWidget {
  const ProfileEdit({super.key});

  @override
  State<ProfileEdit> createState() => _ProfileEditState();
}

class _ProfileEditState extends State<ProfileEdit> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmController = TextEditingController();
  final TextEditingController _full_nameController = TextEditingController();
  final UserService _userService = UserService();

  bool _isLoading = false;
  bool _isDataLoading = true;
  User? _currentUserProfile;
  bool _showPasswordVisibility = false;
  bool _showConfirmVisibility = false;

  @override
  void initState() {
    super.initState();
    _loadUserProfile();
  }

  Future<void> _loadUserProfile() async {
    final authUser = _userService.getCurrentUser();
    if (authUser == null) {
      if (mounted) {
        Navigator.pushNamedAndRemoveUntil(context, '/', (route) => false);
      }
      return;
    }

    try {
      final profile = await _userService.getUserProfile(authUser.id);
      if (mounted) {
        setState(() {
          _currentUserProfile = profile;
          _emailController.text = profile?.email ?? authUser.email ?? '';
          _full_nameController.text = profile?.name ?? '';
          _isDataLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() => _isDataLoading = false);
        _showMessage('Ошибка загрузки профиля: ${e.toString()}');
      }
    }
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    _full_nameController.dispose();
    super.dispose();
  }

  Future<void> _showMessage(String message) async {
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
  }

  Future<void> _updateProfile() async {
    final email = _emailController.text.trim();
    final password = _passwordController.text;
    final confirm = _confirmController.text;
    final name = _full_nameController.text.trim();

    if (email.isEmpty) {
      _showMessage('Введите email');
      return;
    }

    if (password.isNotEmpty || confirm.isNotEmpty) {
      if (password != confirm) {
        _showMessage('Пароли не совпадают');
        return;
      }
      if (password.length < 6) {
        _showMessage('Пароль должен быть не менее 6 символов');
        return;
      }
    }

    setState(() => _isLoading = true);

    try {
      final authUser = _userService.getCurrentUser();
      if (authUser == null) {
        _showMessage('Пользователь не авторизован');
        if (mounted) setState(() => _isLoading = false);
        return;
      }

      if (email != authUser.email) {
        await _userService.updateUserEmail(email);
      }

      final updatedProfile = await _userService.updateUserProfile(
        userId: authUser.id,
        email: email,
        password: password.isNotEmpty ? password : null,
        name: name.isNotEmpty ? name : null,
      );

      if (updatedProfile != null) {
        setState(() {
          _currentUserProfile = updatedProfile;
        });
        _showMessage('Профиль успешно обновлен');
        if (mounted) {
          Navigator.pop(context);
        }
      } else {
        _showMessage('Нет данных для обновления');
      }
    } catch (e) {
      _showMessage('Ошибка: ${e.toString()}');
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isDataLoading) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        title: const Text('Редактирование профиля'),
        backgroundColor: Colors.orange,
        foregroundColor: Colors.white,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: SingleChildScrollView(
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Image.asset(
                'assets/images/logo.png',
                fit: BoxFit.contain,
                height: MediaQuery.of(context).size.height * 0.2,
                width: MediaQuery.of(context).size.width * 0.45,
              ),
              SizedBox(
                width: MediaQuery.of(context).size.width * 0.9,
                height: 50,
                child: Text(
                  "Изменение профиля",
                  textAlign: TextAlign.center,
                  style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
                ),
              ),
              SizedBox(height: MediaQuery.of(context).size.height * 0.02),
              // Поле Email
              SizedBox(
                width: MediaQuery.of(context).size.width * 0.9,
                child: TextField(
                  controller: _emailController,
                  keyboardType: TextInputType.emailAddress,
                  cursorColor: Colors.black,
                  style: TextStyle(color: Colors.orange),
                  decoration: InputDecoration(
                    labelStyle: TextStyle(color: Colors.black),
                    prefixIcon: Icon(Icons.email),
                    labelText: 'Email',
                    hintText: 'Введите email',
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.blue),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.black26),
                    ),
                  ),
                ),
              ),
              SizedBox(height: MediaQuery.of(context).size.height * 0.02),
              SizedBox(
                width: MediaQuery.of(context).size.width * 0.9,
                child: TextField(
                  controller: _full_nameController,
                  keyboardType: TextInputType.text,
                  cursorColor: Colors.black,
                  style: TextStyle(color: Colors.orange),
                  decoration: InputDecoration(
                    labelStyle: TextStyle(color: Colors.black),
                    prefixIcon: Icon(Icons.person),
                    labelText: 'Имя',
                    hintText: 'Введите имя',
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.blue),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.black26),
                    ),
                  ),
                ),
              ),
              SizedBox(height: MediaQuery.of(context).size.height * 0.02),
              SizedBox(
                width: MediaQuery.of(context).size.width * 0.9,
                child: TextField(
                  controller: _passwordController,
                  obscureText: !_showPasswordVisibility,
                  cursorColor: Colors.black,
                  decoration: InputDecoration(
                    labelStyle: TextStyle(color: Colors.black),
                    labelText: 'Новый пароль',
                    hintText: 'Введите новый пароль',
                    prefixIcon: Icon(Icons.lock),
                    suffixIcon: IconButton(
                      onPressed: () {
                        setState(() {
                          _showPasswordVisibility = !_showPasswordVisibility;
                        });
                      },
                      icon: Icon(
                        _showPasswordVisibility
                            ? Icons.visibility_off
                            : Icons.visibility,
                      ),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.blue),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.black26),
                    ),
                  ),
                ),
              ),
              SizedBox(height: MediaQuery.of(context).size.height * 0.02),
              SizedBox(
                width: MediaQuery.of(context).size.width * 0.9,
                child: TextField(
                  controller: _confirmController,
                  obscureText: !_showConfirmVisibility,
                  cursorColor: Colors.black,
                  decoration: InputDecoration(
                    labelStyle: TextStyle(color: Colors.black),
                    labelText: 'Подтвердите пароль',
                    hintText: 'Повторите пароль',
                    prefixIcon: Icon(Icons.lock_clock),
                    suffixIcon: IconButton(
                      onPressed: () {
                        setState(() {
                          _showConfirmVisibility = !_showConfirmVisibility;
                        });
                      },
                      icon: Icon(
                        _showConfirmVisibility
                            ? Icons.visibility_off
                            : Icons.visibility,
                      ),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.blue),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(15),
                      borderSide: BorderSide(color: Colors.black26),
                    ),
                  ),
                ),
              ),
              SizedBox(height: MediaQuery.of(context).size.height * 0.04),
              SizedBox(
                height: MediaQuery.of(context).size.height * 0.055,
                width: MediaQuery.of(context).size.width * 0.8,
                child: ElevatedButton(
                  style: ButtonStyle(
                    shape: WidgetStatePropertyAll(
                      RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(15),
                      ),
                    ),
                    backgroundColor: WidgetStatePropertyAll(Colors.orange),
                  ),
                  onPressed: _isLoading ? null : _updateProfile,
                  child: Text(
                    _isLoading ? 'Сохранение...' : 'Сохранить изменения',
                    style: TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
              SizedBox(height: MediaQuery.of(context).size.height * 0.02),
              TextButton.icon(
                onPressed: () async {
                  await _userService.signOut();
                  if (mounted) {
                    Navigator.pushNamedAndRemoveUntil(
                      context,
                      '/',
                      (route) => false,
                    );
                  }
                },
                icon: Icon(Icons.logout, color: Colors.red),
                label: Text(
                  'Выйти',
                  style: TextStyle(color: Colors.red, fontSize: 16),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
