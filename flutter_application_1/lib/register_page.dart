// lib/register_page.dart
import 'package:flutter/material.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmController = TextEditingController();
  bool _isLoading = false;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _showMessage(String message) async {
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
  }

  Future<void> _register() async {
    final email = _emailController.text.trim();
    final password = _passwordController.text;
    final confirm = _confirmController.text;

    if (email.isEmpty || password.isEmpty || confirm.isEmpty) {
      _showMessage('Заполните все поля');
      return;
    }
    if (password != confirm) {
      _showMessage('Пароли не совпадают');
      return;
    }

    setState(() => _isLoading = true);
    try {
      final signUpRes = await Supabase.instance.client.auth.signUp(
        email: email,
        password: password,
      );

      final user = signUpRes?.user ?? Supabase.instance.client.auth.currentUser;
      if (user == null || user.id == null) {
        _showMessage('Не удалось зарегистрировать пользователя');
        if (mounted) setState(() => _isLoading = false);
        return;
      }

      final insertRes = await Supabase.instance.client.from('users').insert({
        'id': user.id,
        'email': email,
        'password': password,
      }).select();

      if (insertRes == null || (insertRes is List && insertRes.isEmpty)) {
        _showMessage('Не удалось сохранить профиль пользователя в базе');
      } else {
        _showMessage('Регистрация успешна');
        if (mounted) Navigator.pushNamedAndRemoveUntil(context, '/home', (route) => false);
      }
    } catch (e) {
      _showMessage('Ошибка: ${e.toString()}');
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
                obscureText: true,
                cursorColor: Colors.black,
                decoration: InputDecoration(
                  labelStyle: TextStyle(color: Colors.black),
                  labelText: 'Пароль',
                  hintText: 'Введите пароль',
                  prefixIcon: Icon(Icons.lock),
                  suffixIcon: IconButton(
                    onPressed: () {},
                    icon: Icon(Icons.visibility),
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
                obscureText: true,
                cursorColor: Colors.black,
                decoration: InputDecoration(
                  labelStyle: TextStyle(color: Colors.black),
                  labelText: 'Подтвердите пароль',
                  hintText: 'Повторите пароль',
                  prefixIcon: Icon(Icons.lock_clock),
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
                  shape: MaterialStatePropertyAll(
                    RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(15),
                    ),
                  ),
                  backgroundColor: MaterialStatePropertyAll(Colors.orange),
                ),
                onPressed: _isLoading ? null : _register,
                child: Text(
                  _isLoading ? 'Подождите...' : 'Зарегистрироваться',
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
                Text("Уже есть аккаунт?"),
                TextButton(
                  onPressed: () => Navigator.pushNamed(context, '/'),
                  child: Text("Войти", style: TextStyle(color: Colors.blue)),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
