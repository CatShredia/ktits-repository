# Ala Avito - Flutter Marketplace Application

A mobile marketplace application built with Flutter and Supabase backend, similar to Avito classifieds platform.

## 📱 Application Overview

**Ala Avito** is a cross-platform marketplace app that allows users to:

- Browse and search products by category
- Manage user profiles with avatar uploads
- Secure authentication and session management
- View product details with images and pricing

---

## 🏗️ Project Structure

```
flutter_application_1/
├── lib/
│   ├── main.dart                    # App entry point & routing
│   ├── auth_page.dart               # Login screen
│   ├── register_page.dart           # User registration
│   ├── forgot_password.dart         # Password recovery
│   ├── home.dart                    # Main navigation container
│   ├── profile_edit.dart            # Profile editing
│   ├── notifications_page.dart      # Notifications (stub)
│   ├── debug_connection.dart        # Supabase connection diagnostics
│   ├── database/
│   │   ├── models/
│   │   │   ├── user.dart            # User data model
│   │   │   ├── product.dart         # Product data model
│   │   │   └── category.dart        # Category data model
│   │   └── services/
│   │       ├── userservice.dart     # User auth & profile operations
│   │       ├── productservice.dart  # Product CRUD operations
│   │       └── categoryservice.dart # Category fetch operations
│   └── pages/
│       ├── home_tab.dart            # Home feed with categories
│       ├── search_tab.dart          # Product search
│       ├── sell_tab.dart            # Products listing view
│       ├── profile_tab.dart         # User profile with avatar
│       └── product_page.dart        # Product detail page
├── assets/
│   └── images/
│       └── logo.png                 # Application logo
├── test/
│   └── widget_test.dart             # Widget tests
├── test_sql_scripts/
│   └── database_insert.sql          # Database seeding script
├── android/                         # Android platform config
├── ios/                             # iOS platform config
├── web/                             # Web platform config
├── windows/                         # Windows platform config
├── linux/                           # Linux platform config
└── macos/                           # macOS platform config
```

---

## 🔑 Key Features

### Authentication

- **Login** - Email/password authentication via Supabase
- **Registration** - New user signup with profile creation
- **Password Recovery** - Email-based password reset
- **Session Persistence** - Automatic session restoration

### Main Navigation (4 Tabs)

| Tab                   | Description                         |
| --------------------- | ----------------------------------- |
| **Главная** (Home)    | Category-filtered product feed      |
| **Поиск** (Search)    | Real-time product search            |
| **Продажа** (Sell)    | Product listing view                |
| **Профиль** (Profile) | User profile with avatar management |

### Product Features

- Browse products by category
- Search products by name
- View product details (price, description, stock, status)
- Cached product images
- Active/inactive product status

### User Profile

- Avatar upload (camera or gallery)
- Edit profile (email, name, password)
- Settings navigation
- Logout functionality

---

## 📦 Dependencies

### Production

| Package                | Purpose                                        |
| ---------------------- | ---------------------------------------------- |
| `supabase_flutter`     | Backend-as-a-Service (auth, database, storage) |
| `salomon_bottom_bar`   | Custom bottom navigation bar                   |
| `image_picker`         | Camera/gallery access for avatar upload        |
| `cached_network_image` | Image caching for product images               |
| `intl`                 | Internationalization (date/number formatting)  |
| `crypto`               | Cryptographic functions                        |
| `http`                 | HTTP client                                    |
| `cupertino_icons`      | iOS-style icons                                |

### Development

- `flutter_test` - Testing framework
- `flutter_lints` - Code linting rules

---

## 🗄️ Database Schema (Supabase)

### Tables

**users**

- `id`, `email`, `password`, `full_name`, `avatar`, `created_at`

**products**

- `id`, `user_id`, `category_id`, `name`, `image`, `description`, `price_cents`, `currency`, `stock`, `is_active`, `created_at`

**product_categories**

- `id`, `name`, `image`, `created_at`

**notifications**

- `id`, `user_id`, `title`, `body`, `status`, `read_at`

### Supabase Configuration

- **URL:** `https://mjjncjvuexsneazjajph.supabase.co`
- **Storage Bucket:** `avatars` (user profile pictures)

---

## 🚀 Getting Started

### Prerequisites

- Flutter SDK ^3.10.8
- Dart SDK
- Supabase account (backend)

### Installation

```bash
# Navigate to project directory
cd flutter_application_1

# Install dependencies
flutter pub get

# Run the application
flutter run
```

### Platform-Specific Setup

#### Android

Permissions required in `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"/>
<uses-permission android:name="android.permission.CAMERA"/>
```

#### iOS

Privacy descriptions in `Info.plist`:

- `NSPhotoLibraryUsageDescription` - Photo library access for avatar
- `NSCameraUsageDescription` - Camera access for avatar

---

## 🧪 Testing

```bash
# Run all tests
flutter test

# Run with coverage
flutter test --coverage
```

> **Note:** Current test coverage is minimal. The default widget test does not cover actual app functionality.

---

## 📱 Supported Platforms

- ✅ Android
- ✅ iOS
- ✅ Web
- ✅ Windows
- ✅ Linux
- ✅ macOS

---

## 🏛️ Architecture

**Pattern:** Layered Architecture

```
┌─────────────────────────────────────┐
│         UI Layer                    │
│  (pages/*.dart, *.dart in lib/)     │
├─────────────────────────────────────┤
│      Service Layer                  │
│  (database/services/*.dart)         │
├─────────────────────────────────────┤
│      Model Layer                    │
│  (database/models/*.dart)           │
├─────────────────────────────────────┤
│      Supabase Backend               │
│  (Auth, Database, Storage)          │
└─────────────────────────────────────┘
```

**State Management:** `StatefulWidget` with `setState()` (no external state management library)

---

## 🌐 Localization

- **UI Language:** Russian
- All user-facing text is in Russian

---

## ⚠️ Known Issues & Considerations

1. **Security:** Passwords appear to be stored in plaintext in the database (should use hashing)
2. **Notifications:** `notifications_page.dart` is a stub with no implementation
3. **Test Coverage:** Minimal test coverage for core functionality
4. **State Management:** Basic `setState()` approach may not scale for larger apps

---

## 🛠️ Development Tools

- **Debug Connection:** `debug_connection.dart` provides Supabase connectivity diagnostics
- **SQL Scripts:** `test_sql_scripts/database_insert.sql` for test data seeding

---

## 📄 License

This project is versioned as **1.0.0+1**.

---

## 📞 Support

For issues or questions, please refer to the Supabase dashboard and connection diagnostics.
