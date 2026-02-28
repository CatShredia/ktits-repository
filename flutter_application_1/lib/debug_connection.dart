// lib/debug_connection.dart
// ignore_for_file: avoid_print

import 'dart:io';
import 'dart:async' show TimeoutException;
import 'package:supabase_flutter/supabase_flutter.dart';

/// Запустите эту функцию из main() для проверки подключения
Future<void> debugSupabaseConnection() async {
  const String supabaseUrl = 'https://mjjncjvuexsneazjajph.supabase.co';
  const String anonKey = 'sb_publishable_G0-YAme8jkW4KAxteNz1PQ_MIDM-Ld0';

  print('=== Проверка подключения к Supabase ===\n');

  // 1. Проверка DNS
  print('1. Проверка DNS...');
  try {
    final uri = Uri.parse(supabaseUrl);
    final addresses = await InternetAddress.lookup(uri.host);
    print('   ✓ DNS разрешён: ${addresses.first.address}');
  } on SocketException catch (e) {
    print('   ✗ Ошибка DNS: ${e.message}');
    print('   → Проверьте интернет-соединение и DNS');
    return;
  } catch (e) {
    print('   ✗ Ошибка: $e');
    return;
  }

  // 2. Проверка HTTP соединения (упрощённая)
  print('\n2. Проверка HTTP соединения...');
  try {
    final client = HttpClient();
    final request = await client.getUrl(Uri.parse('$supabaseUrl/rest/v1/'));
    request.headers.add('apikey', anonKey);
    request.headers.add('Authorization', 'Bearer $anonKey');

    final response = await request.close().timeout(const Duration(seconds: 5));
    print('   Статус: ${response.statusCode}');
    if (response.statusCode == 200 || response.statusCode == 404) {
      print('   ✓ Сервер отвечает');
    } else {
      print('   ⚠ Статус: ${response.statusCode}');
    }
    client.close();
  } on SocketException catch (e) {
    print('   ✗ Ошибка сети: ${e.message}');
    print('   → Устройство не имеет доступа к интернету');
  } on TimeoutException {
    print('   ✗ Таймаут (5 сек)');
    print('   → Сервер не отвечает или блокирует соединение');
    print('   → Проверьте статус проекта в https://supabase.com/dashboard');
  } on HttpException catch (e) {
    print('   ✗ HTTP ошибка: ${e.message}');
  } catch (e) {
    print('   ✗ Ошибка: $e');
  }

  // 3. Проверка Supabase клиента
  print('\n3. Проверка Supabase клиента...');
  try {
    await Supabase.initialize(url: supabaseUrl, anonKey: anonKey);
    print('   ✓ Инициализация успешна');

    // Проверка авторизации
    final user = Supabase.instance.client.auth.currentUser;
    print('   Текущий пользователь: ${user?.email ?? "не авторизован"}');
  } on SocketException catch (e) {
    print('   ✗ Ошибка сети: ${e.message}');
  } catch (e) {
    print('   ✗ Ошибка инициализации: $e');
  }

  print('\n=== Проверка завершена ===\n');
}
