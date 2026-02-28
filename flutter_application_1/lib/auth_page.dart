import 'package:flutter/material.dart';
import 'database/services/userservice.dart';

class AuthPage extends StatefulWidget {
  const AuthPage({super.key});

  @override
  State<AuthPage> createState() => _AuthPageState();
}

class _AuthPageState extends State<AuthPage> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final UserService _userService = UserService();
  bool _isLoading = false;
  bool _obscurePassword = true;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _showMessage(String message) async {
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
  }

  Future<void> _login() async {
    final email = _emailController.text.trim();
    final password = _passwordController.text;

    if (email.isEmpty || password.isEmpty) {
      _showMessage('Заполните email и пароль');
      return;
    }

    setState(() => _isLoading = true);
    try {
      final response = await _userService.signIn(email, password);

      final user = response.user;
      if (user == null) {
        _showMessage('Ошибка входа: пользователь не найден или пароль неверен');
        if (mounted) setState(() => _isLoading = false);
        return;
      }

      _showMessage('Вход успешен');
      if (mounted) {
        Navigator.pushNamedAndRemoveUntil(context, '/home', (route) => false);
      }
    } catch (e) {
      String errorMsg = 'Ошибка входа';
      if (e.toString().contains('Invalid login')) {
        errorMsg = 'Неверный email или пароль';
      } else if (e.toString().contains('User not found')) {
        errorMsg = 'Пользователь не найден';
      } else if (e.toString().contains('Email not confirmed')) {
        errorMsg = 'Email не подтверждён. Проверьте ваш почтовый ящик.';
      } else {
        errorMsg = 'Ошибка: ${e.toString()}';
      }
      _showMessage(errorMsg);
    }
    if (mounted) setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Image.asset(
              'assets/images/logo.png',
              fit: BoxFit.contain,
              height: MediaQuery.of(context).size.height * 0.3,
              width: MediaQuery.of(context).size.width * 0.45,
            ),
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
                controller: _passwordController,
                obscureText: _obscurePassword,
                cursorColor: Colors.black,
                decoration: InputDecoration(
                  labelStyle: TextStyle(color: Colors.black),
                  labelText: 'Пароль',
                  hintText: 'Введите пароль',
                  prefixIcon: Icon(Icons.lock),
                  suffixIcon: IconButton(
                    onPressed: () =>
                        setState(() => _obscurePassword = !_obscurePassword),
                    icon: Icon(
                      _obscurePassword
                          ? Icons.visibility
                          : Icons.visibility_off,
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

            Container(
              width: MediaQuery.of(context).size.width * 0.9,
              alignment: Alignment.centerRight,
              child: InkWell(
                child: Text(
                  'Забыли пароль?',
                  style: TextStyle(color: Colors.blue),
                ),
                onTap: () => Navigator.pushNamed(context, '/forgot-password'),
              ),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.02),

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
                onPressed: _isLoading ? null : _login,
                child: Text(
                  _isLoading ? 'Подождите...' : 'Войти',
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
            SizedBox(height: MediaQuery.of(context).size.height * 0.02),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text("Нет аккаунта?"),
                TextButton(
                  onPressed: () => Navigator.pushNamed(context, '/register'),
                  child: Text(
                    "Зарегистрироваться",
                    style: TextStyle(color: Colors.blue),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
