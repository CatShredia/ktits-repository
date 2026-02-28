// lib/main.dart
import 'package:flutter/material.dart';
import 'auth_page.dart';
import 'register_page.dart';
import 'forgot_password.dart';
import 'home.dart';
import 'debug_connection.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Запуск проверки подключения
  await debugSupabaseConnection();

  await Supabase.initialize(
    url: 'https://mjjncjvuexsneazjajph.supabase.co',
    anonKey: 'sb_publishable_G0-YAme8jkW4KAxteNz1PQ_MIDM-Ld0',
  );
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Ala Avito',
      theme: ThemeData.light(),
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      routes: {
        '/': (context) => AuthPage(),
        '/register': (context) => RegisterPage(),
        '/forgot-password': (context) => RecoveryPage(),
        '/home': (context) => const HomePage(),
      },
    );
  }
}
