import 'package:flutter/material.dart';
import 'package:salomon_bottom_bar/salomon_bottom_bar.dart';
import 'database/services/userservice.dart';
import 'pages/home_tab.dart';
import 'pages/search_tab.dart';
import 'pages/sell_tab.dart';
import 'pages/profile_tab.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _currentIndex = 0;
  final UserService _userService = UserService();

  final List<Widget> _screens = [
    const HomeTab(),
    const SearchTab(),
    const SellTab(),
    const ProfileTab(),
  ];

  @override
  void initState() {
    super.initState();
    _checkSession();
  }

  void _checkSession() {
    final user = _userService.getCurrentUser();
    if (user == null) {
      // Пользователь не авторизован
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) {
          Navigator.pushNamedAndRemoveUntil(context, '/', (route) => false);
        }
      });
    }
  }

  Future<void> _logout(BuildContext context) async {
    await _userService.signOut();
    if (context.mounted) {
      Navigator.pushNamedAndRemoveUntil(context, '/', (route) => false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ala Avito'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () => _logout(context),
            tooltip: 'Выйти',
          ),
        ],
      ),
      body: _screens[_currentIndex],
      bottomNavigationBar: SalomonBottomBar(
        selectedItemColor: Colors.amber,
        unselectedItemColor: Colors.grey,
        onTap: (index) {
          setState(() {
            _currentIndex = index;
          });
        },
        currentIndex: _currentIndex,
        items: [
          SalomonBottomBarItem(
            icon: const Icon(Icons.home),
            title: const Text('Главная'),
          ),
          SalomonBottomBarItem(
            icon: const Icon(Icons.search),
            title: const Text('Поиск'),
          ),
          SalomonBottomBarItem(
            icon: const Icon(Icons.shopping_basket),
            title: const Text('Продажа'),
          ),
          SalomonBottomBarItem(
            icon: const Icon(Icons.person),
            title: const Text('Профиль'),
          ),
        ],
      ),
    );
  }
}
