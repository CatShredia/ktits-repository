# Flutter UI Widgets Reference Guide
## Ala Avito Application

This document describes all UI widgets used in the application with their properties and examples.

---

## Table of Contents

1. [Layout Widgets](#layout-widgets)
2. [Material Widgets](#material-widgets)
3. [Input Widgets](#input-widgets)
4. [Display Widgets](#display-widgets)
5. [Navigation Widgets](#navigation-widgets)
6. [Scrolling Widgets](#scrolling-widgets)
7. [Common Properties Reference](#common-properties-reference)

---

## Layout Widgets

### 1. Column

Arranges children vertically in a line.

**File:** `auth_page.dart`, `product_page.dart`, `profile_tab.dart`

```dart
Column(
  // Main properties
  mainAxisAlignment: MainAxisAlignment.center,    // Vertical alignment
  crossAxisAlignment: CrossAxisAlignment.start, // Horizontal alignment
  children: [
    Text('First widget'),
    Text('Second widget'),
  ],
)
```

| Property | Type | Description | Example Values |
|----------|------|-------------|----------------|
| `mainAxisAlignment` | `MainAxisAlignment` | Vertical alignment | `center`, `start`, `end`, `spaceBetween`, `spaceAround` |
| `crossAxisAlignment` | `CrossAxisAlignment` | Horizontal alignment | `start`, `center`, `end`, `stretch` |
| `children` | `List<Widget>` | Child widgets | `[Text(), SizedBox()]` |

---

### 2. Row

Arranges children horizontally in a line.

**File:** `product_page.dart`, `sell_tab.dart`

```dart
Row(
  // Main properties
  mainAxisAlignment: MainAxisAlignment.start,   // Horizontal alignment
  crossAxisAlignment: CrossAxisAlignment.center, // Vertical alignment
  children: [
    Icon(Icons.inventory_2_outlined),
    SizedBox(width: 8),
    Text('В наличии: 5 шт.'),
  ],
)
```

| Property | Type | Description | Example Values |
|----------|------|-------------|----------------|
| `mainAxisAlignment` | `MainAxisAlignment` | Horizontal alignment | `start`, `center`, `end`, `spaceBetween` |
| `crossAxisAlignment` | `CrossAxisAlignment` | Vertical alignment | `start`, `center`, `end`, `stretch` |
| `children` | `List<Widget>` | Child widgets | `[Icon(), Text()]` |

---

### 3. Stack

Overlaps children on top of each other (z-axis layering).

**File:** `profile_tab.dart`, `home_tab.dart`, `sell_tab.dart`

```dart
Stack(
  // Main properties
  alignment: Alignment.center,        // Default alignment for all children
  fit: StackFit.expand,               // How to size non-positioned children
  children: [
    // Background image
    CachedNetworkImage(imageUrl: url),
    
    // Badge on top
    Positioned(
      top: 8,
      right: 8,
      child: Text('Активен'),
    ),
  ],
)
```

| Property | Type | Description | Example Values |
|----------|------|-------------|----------------|
| `alignment` | `AlignmentGeometry` | Default alignment | `center`, `topLeft`, `bottomRight` |
| `fit` | `StackFit` | Size constraint for children | `expand`, `loose`, `passthrough` |
| `children` | `List<Widget>` | Child widgets | `[Image(), Positioned()]` |

---

### 4. Positioned

Positions a child within a Stack at specific coordinates.

**File:** `profile_tab.dart`, `sell_tab.dart`

```dart
Positioned(
  // Position properties
  top: 0,      // Distance from top
  bottom: 0,   // Distance from bottom
  left: 0,     // Distance from left
  right: 0,    // Distance from right
  width: 100,  // Fixed width
  height: 100, // Fixed height
  child: Icon(Icons.camera_alt),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `top` | `double?` | Distance from top edge |
| `bottom` | `double?` | Distance from bottom edge |
| `left` | `double?` | Distance from left edge |
| `right` | `double?` | Distance from right edge |
| `width` | `double?` | Fixed width |
| `height` | `double?` | Fixed height |
| `child` | `Widget` | Child widget |

---

### 5. Container

A box with decoration, constraints, and transformations.

**File:** `product_page.dart`, `home_tab.dart`

```dart
Container(
  // Size properties
  width: double.infinity,
  height: 300,
  
  // Padding inside container
  padding: const EdgeInsets.all(16),
  
  // Margin outside container
  margin: const EdgeInsets.symmetric(horizontal: 8),
  
  // Decoration (colors, borders, etc.)
  decoration: BoxDecoration(
    color: Colors.grey[200],
    borderRadius: BorderRadius.circular(12),
    border: Border.all(color: Colors.grey[300]!),
  ),
  
  // Child widget
  child: Text('Content'),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `width` | `double?` | Container width |
| `height` | `double?` | Container height |
| `padding` | `EdgeInsetsGeometry` | Inner spacing |
| `margin` | `EdgeInsetsGeometry` | Outer spacing |
| `decoration` | `Decoration` | Visual styling |
| `child` | `Widget` | Single child widget |
| `alignment` | `AlignmentGeometry` | Child alignment |

---

### 6. Padding

Adds padding around a child widget.

**File:** `search_tab.dart`, `product_page.dart`

```dart
Padding(
  padding: const EdgeInsets.all(16),           // All sides
  // padding: EdgeInsets.symmetric(horizontal: 12, vertical: 8)
  // padding: EdgeInsets.only(left: 10, right: 10, top: 5, bottom: 5)
  child: TextField(),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `padding` | `EdgeInsetsGeometry` | Padding values |
| `child` | `Widget` | Child widget |

---

### 7. SizedBox

A box with a specified size.

**File:** All pages (spacing between widgets)

```dart
// Fixed spacing
SizedBox(height: 16),
SizedBox(width: 8),

// Fixed size box
SizedBox(
  height: 50,
  width: 200,
  child: Text('Fixed size'),
)

// Expand to fill parent
SizedBox.expand(child: Image()),
```

| Property | Type | Description |
|----------|------|-------------|
| `width` | `double?` | Fixed width |
| `height` | `double?` | Fixed height |
| `child` | `Widget` | Child widget |

---

### 8. Expanded

Expands a child to fill available space in a Row or Column.

**File:** `home_tab.dart`, `product_page.dart`

```dart
Column(
  children: [
    // Fixed height widget
    Text('Header'),
    
    // Expands to fill remaining space
    Expanded(
      flex: 1,  // Flex factor (default: 1)
      child: GridView.builder(...),
    ),
  ],
)
```

| Property | Type | Description |
|----------|------|-------------|
| `flex` | `int` | Flex factor for space distribution |
| `child` | `Widget` | Child widget to expand |

---

### 9. Center

Centers its child within itself.

**File:** All pages

```dart
Center(
  // Alignment properties
  widthFactor: 1.0,   // Width multiplier
  heightFactor: 1.0,  // Height multiplier
  child: CircularProgressIndicator(),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `child` | `Widget` | Child widget to center |
| `widthFactor` | `double?` | Width constraint |
| `heightFactor` | `double?` | Height constraint |

---

## Material Widgets

### 10. Scaffold

Provides the basic material design visual structure.

**File:** All pages

```dart
Scaffold(
  // Main properties
  backgroundColor: Colors.white,
  appBar: AppBar(title: Text('Title')),
  body: SingleChildScrollView(child: ...),
  floatingActionButton: FloatingActionButton(...),
  bottomNavigationBar: BottomNavigationBar(...),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `appBar` | `AppBar?` | Top app bar |
| `body` | `Widget?` | Main content |
| `backgroundColor` | `Color?` | Background color |
| `floatingActionButton` | `Widget?` | Floating action button |
| `bottomNavigationBar` | `Widget?` | Bottom navigation |

---

### 11. AppBar

A material design app bar.

**File:** `product_page.dart`, `profile_edit.dart`

```dart
AppBar(
  // Main properties
  title: const Text('Товар'),
  backgroundColor: Colors.orange,
  foregroundColor: Colors.white,
  
  // Leading icon (usually back button)
  leading: IconButton(
    icon: Icon(Icons.arrow_back),
    onPressed: () => Navigator.pop(context),
  ),
  
  // Actions on the right
  actions: [
    IconButton(
      icon: Icon(Icons.refresh),
      onPressed: _loadProduct,
      tooltip: 'Обновить',
    ),
  ],
)
```

| Property | Type | Description |
|----------|------|-------------|
| `title` | `Widget?` | Title widget |
| `backgroundColor` | `Color?` | Background color |
| `foregroundColor` | `Color?` | Icon/text color |
| `leading` | `Widget?` | Left-side widget |
| `actions` | `List<Widget>` | Right-side widgets |
| `elevation` | `double?` | Shadow elevation |

---

### 12. Card

A material design card with rounded corners and shadow.

**File:** `home_tab.dart`, `search_tab.dart`, `profile_tab.dart`

```dart
Card(
  // Main properties
  elevation: 2,                    // Shadow elevation
  clipBehavior: Clip.antiAlias,    // Clip content to rounded corners
  shape: RoundedRectangleBorder(   // Border shape
    borderRadius: BorderRadius.circular(12),
  ),
  color: Colors.white,             // Card color
  child: Column(...),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `elevation` | `double` | Shadow depth |
| `clipBehavior` | `Clip` | Content clipping |
| `shape` | `ShapeBorder` | Border shape |
| `color` | `Color?` | Background color |
| `child` | `Widget` | Child widget |

---

### 13. ElevatedButton

A filled button with elevation.

**File:** `auth_page.dart`, `register_page.dart`, `profile_edit.dart`

```dart
ElevatedButton(
  // Main properties
  onPressed: _isLoading ? null : _login,
  style: ButtonStyle(
    // Button shape
    shape: WidgetStatePropertyAll(
      RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(15),
      ),
    ),
    // Background color
    backgroundColor: WidgetStatePropertyAll(Colors.orange),
    // Text color
    foregroundColor: WidgetStatePropertyAll(Colors.white),
  ),
  child: Text(
    _isLoading ? 'Подождите...' : 'Войти',
    style: TextStyle(fontWeight: FontWeight.bold),
  ),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `onPressed` | `VoidCallback?` | Tap handler (null = disabled) |
| `style` | `ButtonStyle` | Button styling |
| `child` | `Widget` | Button content |

---

### 14. TextButton

A simple text button without elevation.

**File:** `auth_page.dart`, `register_page.dart`

```dart
TextButton(
  onPressed: () => Navigator.pushNamed(context, '/register'),
  child: Text(
    "Зарегистрироваться",
    style: TextStyle(color: Colors.blue),
  ),
)

// With icon
TextButton.icon(
  onPressed: _logout,
  icon: Icon(Icons.logout, color: Colors.red),
  label: Text('Выйти', style: TextStyle(color: Colors.red)),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `onPressed` | `VoidCallback?` | Tap handler |
| `child` | `Widget` | Button content |
| `icon` | `Widget` | Icon (for `.icon` constructor) |
| `label` | `Widget` | Label (for `.icon` constructor) |

---

### 15. IconButton

A button with an icon.

**File:** `product_page.dart`, `search_tab.dart`

```dart
IconButton(
  // Main properties
  icon: Icon(Icons.refresh),
  onPressed: _loadProduct,
  tooltip: 'Обновить',
  color: Colors.blue,
  iconSize: 24,
)
```

| Property | Type | Description |
|----------|------|-------------|
| `icon` | `Widget` | Icon widget |
| `onPressed` | `VoidCallback?` | Tap handler |
| `tooltip` | `String?` | Long-press hint |
| `color` | `Color?` | Icon color |
| `iconSize` | `double` | Icon size |

---

### 16. AlertDialog

A material design alert dialog.

**File:** `profile_tab.dart`

```dart
AlertDialog(
  // Main properties
  title: const Text('Выберите источник'),
  content: Text('Content'),  // Optional
  actions: [
    TextButton(
      onPressed: () => Navigator.pop(context, 'camera'),
      child: const Text('Камера'),
    ),
    TextButton(
      onPressed: () => Navigator.pop(context),
      child: const Text('Отмена'),
    ),
  ],
)
```

| Property | Type | Description |
|----------|------|-------------|
| `title` | `Widget?` | Dialog title |
| `content` | `Widget?` | Dialog content |
| `actions` | `List<Widget>` | Action buttons |

---

### 17. SnackBar

A lightweight message bar.

**File:** `profile_tab.dart`, `auth_page.dart`

```dart
ScaffoldMessenger.of(context).showSnackBar(
  SnackBar(
    content: Text('Аватар успешно загружен'),
    duration: Duration(seconds: 2),
    backgroundColor: Colors.green,
  ),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `content` | `Widget` | Message content |
| `duration` | `Duration` | Display duration |
| `backgroundColor` | `Color?` | Background color |

---

### 18. CircleAvatar

A circular avatar widget.

**File:** `profile_tab.dart`

```dart
CircleAvatar(
  // Main properties
  backgroundImage: NetworkImage(avatarUrl),
  radius: 60,
  backgroundColor: Colors.grey[200],
  child: Icon(Icons.person),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `backgroundImage` | `ImageProvider?` | Avatar image |
| `radius` | `double` | Avatar radius |
| `child` | `Widget` | Fallback content |

---

### 19. FilterChip

A chip that can be selected/deselected.

**File:** `home_tab.dart`

```dart
FilterChip(
  // Main properties
  label: Text(category?.name ?? 'Все'),
  selected: isSelected,
  onSelected: (selected) {
    _onCategorySelected(selected ? category : null);
  },
  backgroundColor: Colors.grey[200],
  selectedColor: Colors.orange[100],
  checkmarkColor: Colors.orange,
  labelStyle: TextStyle(
    color: isSelected ? Colors.orange[800] : Colors.black87,
    fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
  ),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `label` | `Widget` | Chip text |
| `selected` | `bool` | Selection state |
| `onSelected` | `Function(bool)` | Selection handler |
| `backgroundColor` | `Color?` | Unselected color |
| `selectedColor` | `Color?` | Selected color |
| `checkmarkColor` | `Color?` | Checkmark color |

---

### 20. ListTile

A single fixed-height row with icons and text.

**File:** `profile_tab.dart`

```dart
ListTile(
  // Main properties
  leading: Icon(Icons.security),
  title: Text('Безопасность'),
  subtitle: Text('Subtitle'),  // Optional
  trailing: Icon(Icons.chevron_right),  // Optional
  onTap: () {
    // Handle tap
  },
)
```

| Property | Type | Description |
|----------|------|-------------|
| `leading` | `Widget?` | Left-side widget |
| `title` | `Widget` | Main text |
| `subtitle` | `Widget?` | Secondary text |
| `trailing` | `Widget?` | Right-side widget |
| `onTap` | `VoidCallback?` | Tap handler |

---

### 21. CircularProgressIndicator

A circular progress indicator.

**File:** All pages (loading states)

```dart
CircularProgressIndicator(
  // Main properties
  strokeWidth: 2,
  color: Colors.orange,
  backgroundColor: Colors.grey[200],
)
```

| Property | Type | Description |
|----------|------|-------------|
| `strokeWidth` | `double` | Line thickness |
| `color` | `Color?` | Progress color |
| `backgroundColor` | `Color?` | Track color |

---

## Input Widgets

### 22. TextField

A material design text input field.

**File:** `auth_page.dart`, `register_page.dart`, `profile_edit.dart`, `search_tab.dart`

```dart
TextField(
  // Controller for text access
  controller: _emailController,
  
  // Input type
  keyboardType: TextInputType.emailAddress,
  obscureText: _obscurePassword,
  
  // Styling
  cursorColor: Colors.black,
  style: TextStyle(color: Colors.orange),
  
  // Decoration
  decoration: InputDecoration(
    labelText: 'Email',
    hintText: 'Введите email',
    prefixIcon: Icon(Icons.email),
    suffixIcon: IconButton(...),
    
    // Border styles
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(15),
      borderSide: BorderSide(color: Colors.blue, width: 2),
    ),
    enabledBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(15),
      borderSide: BorderSide(color: Colors.black26),
    ),
    
    // Other properties
    filled: true,
    fillColor: Colors.grey[100],
  ),
  
  // Callbacks
  onChanged: _searchProducts,
)
```

| Property | Type | Description |
|----------|------|-------------|
| `controller` | `TextEditingController` | Text controller |
| `keyboardType` | `TextInputType` | Keyboard type |
| `obscureText` | `bool` | Hide text (password) |
| `decoration` | `InputDecoration` | Field decoration |
| `style` | `TextStyle` | Text styling |
| `onChanged` | `Function(String)` | Text change callback |

---

### 23. InputDecoration

Decoration for TextField.

```dart
InputDecoration(
  // Text labels
  labelText: 'Email',
  hintText: 'Введите email',
  labelStyle: TextStyle(color: Colors.black),
  
  // Icons
  prefixIcon: Icon(Icons.email),
  suffixIcon: IconButton(icon: Icon(Icons.clear), onPressed: ...),
  
  // Borders
  border: OutlineInputBorder(...),
  focusedBorder: OutlineInputBorder(...),
  enabledBorder: OutlineInputBorder(...),
  
  // Colors
  filled: true,
  fillColor: Colors.grey[100],
)
```

| Property | Type | Description |
|----------|------|-------------|
| `labelText` | `String?` | Label text |
| `hintText` | `String?` | Placeholder text |
| `prefixIcon` | `Widget?` | Left icon |
| `suffixIcon` | `Widget?` | Right icon |
| `border` | `InputBorder` | Default border |
| `focusedBorder` | `InputBorder` | Focused border |
| `enabledBorder` | `InputBorder` | Enabled border |

---

### 24. OutlineInputBorder

A rectangular outline border.

```dart
OutlineInputBorder(
  borderRadius: BorderRadius.circular(15),
  borderSide: BorderSide(color: Colors.blue, width: 2),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `borderRadius` | `BorderRadius` | Corner radius |
| `borderSide` | `BorderSide` | Border style |

---

## Display Widgets

### 25. Text

Displays text with styling.

**File:** All pages

```dart
Text(
  'Product Name',
  // Main properties
  style: TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.bold,
    color: Colors.orange,
    height: 1.5,  // Line height multiplier
  ),
  textAlign: TextAlign.center,
  maxLines: 2,
  overflow: TextOverflow.ellipsis,
)
```

| Property | Type | Description |
|----------|------|-------------|
| `style` | `TextStyle` | Text styling |
| `textAlign` | `TextAlign` | Text alignment |
| `maxLines` | `int?` | Maximum lines |
| `overflow` | `TextOverflow` | Overflow handling |

---

### 26. TextStyle

Styling for Text widgets.

```dart
TextStyle(
  fontSize: 24,           // Font size
  fontWeight: FontWeight.bold,  // Weight
  color: Colors.orange,   // Text color
  height: 1.5,           // Line height multiplier
  fontStyle: FontStyle.italic,  // Italic/normal
)
```

| Property | Type | Description |
|----------|------|-------------|
| `fontSize` | `double` | Font size |
| `fontWeight` | `FontWeight` | Font weight |
| `color` | `Color` | Text color |
| `height` | `double?` | Line height |
| `fontStyle` | `FontStyle` | Italic or normal |

---

### 27. Icon

Displays an icon.

**File:** All pages

```dart
Icon(
  Icons.search,
  size: 64,
  color: Colors.orange,
)
```

| Property | Type | Description |
|----------|------|-------------|
| `icon` | `IconData` | Icon data |
| `size` | `double` | Icon size |
| `color` | `Color?` | Icon color |

---

### 28. Image

Displays an image.

**File:** `auth_page.dart`, `register_page.dart`

```dart
Image.asset(
  'assets/images/logo.png',
  fit: BoxFit.contain,
  height: MediaQuery.of(context).size.height * 0.3,
  width: MediaQuery.of(context).size.width * 0.45,
)
```

| Property | Type | Description |
|----------|------|-------------|
| `fit` | `BoxFit` | How to fit image |
| `height` | `double?` | Image height |
| `width` | `double?` | Image width |

---

### 29. CachedNetworkImage

Caches and displays network images.

**File:** `home_tab.dart`, `product_page.dart`, `sell_tab.dart`

```dart
CachedNetworkImage(
  imageUrl: product.image!,
  fit: BoxFit.cover,
  
  // Loading placeholder
  placeholder: (context, url) => Container(
    color: Colors.grey[200],
    child: Center(child: CircularProgressIndicator()),
  ),
  
  // Error widget
  errorWidget: (context, url, error) => Container(
    color: Colors.grey[200],
    child: Icon(Icons.image_not_supported_outlined),
  ),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `imageUrl` | `String` | Image URL |
| `fit` | `BoxFit` | How to fit image |
| `placeholder` | `Widget Function` | Loading widget |
| `errorWidget` | `Widget Function` | Error widget |

---

### 30. BoxDecoration

Decoration for Container.

```dart
DecorationBox(
  color: Colors.grey[200],
  borderRadius: BorderRadius.circular(12),
  border: Border.all(color: Colors.grey[300]!),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `color` | `Color?` | Background color |
| `borderRadius` | `BorderRadius` | Corner radius |
| `border` | `Border` | Border style |

---

### 31. BorderRadius

Rounded corners for containers.

```dart
BorderRadius.circular(12),
BorderRadius.only(topLeft: Radius.circular(10), topRight: Radius.circular(10))
```

---

### 32. Border

Border styling.

```dart
Border.all(
  color: Colors.green,
  width: 1,
)
```

| Property | Type | Description |
|----------|------|-------------|
| `color` | `Color` | Border color |
| `width` | `double` | Border width |

---

## Navigation Widgets

### 33. Navigator

Programmatic navigation.

**File:** All pages

```dart
// Push named route
Navigator.pushNamed(context, '/product', arguments: {'id': product.id});

// Push and remove all previous routes
Navigator.pushNamedAndRemoveUntil(context, '/home', (route) => false);

// Pop (go back)
Navigator.pop(context);

// Pop with result
Navigator.pop(context, 'camera');
```

---

### 34. InkWell

A rectangular area with material ripple effect on tap.

**File:** `home_tab.dart`, `profile_tab.dart`

```dart
InkWell(
  onTap: () {
    Navigator.pushNamed(context, '/product', arguments: {'id': product.id});
  },
  child: Column(...),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `onTap` | `VoidCallback?` | Tap handler |
| `child` | `Widget` | Child widget |

---

## Scrolling Widgets

### 35. SingleChildScrollView

A scrollable widget with a single child.

**File:** `product_page.dart`, `profile_tab.dart`, `profile_edit.dart`

```dart
SingleChildScrollView(
  scrollDirection: Axis.vertical,  // or Axis.horizontal
  child: Column(...),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `scrollDirection` | `Axis` | Scroll direction |
| `child` | `Widget` | Single child widget |

---

### 36. ListView

A scrollable list of widgets.

**File:** `home_tab.dart`

```dart
ListView(
  scrollDirection: Axis.horizontal,  // Horizontal scrolling
  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
  children: [
    _buildCategoryChip(null),
    ..._categories.map((category) => _buildCategoryChip(category)),
  ],
)
```

| Property | Type | Description |
|----------|------|-------------|
| `scrollDirection` | `Axis` | Scroll direction |
| `padding` | `EdgeInsetsGeometry` | List padding |
| `children` | `List<Widget>` | List items |

---

### 37. GridView.builder

A scrollable grid of widgets.

**File:** `home_tab.dart`, `search_tab.dart`, `sell_tab.dart`

```dart
GridView.builder(
  padding: const EdgeInsets.all(8),
  
  // Grid configuration
  gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
    crossAxisCount: 2,           // Items per row
    childAspectRatio: 0.75,      // Width / Height ratio
    crossAxisSpacing: 8,         // Horizontal spacing
    mainAxisSpacing: 8,          // Vertical spacing
  ),
  
  itemCount: _products.length,
  itemBuilder: (context, index) {
    final product = _products[index];
    return _buildProductCard(product);
  },
)
```

| Property | Type | Description |
|----------|------|-------------|
| `gridDelegate` | `SliverGridDelegate` | Grid layout config |
| `itemCount` | `int` | Number of items |
| `itemBuilder` | `Function` | Item builder callback |
| `padding` | `EdgeInsetsGeometry` | Grid padding |

---

### 38. SliverGridDelegateWithFixedCrossAxisCount

Grid layout configuration.

```dart
SliverGridDelegateWithFixedCrossAxisCount(
  crossAxisCount: 2,           // 2 columns
  childAspectRatio: 0.75,      // Width/Height = 0.75
  crossAxisSpacing: 8,         // 8px between columns
  mainAxisSpacing: 8,          // 8px between rows
)
```

| Property | Type | Description |
|----------|------|-------------|
| `crossAxisCount` | `int` | Number of columns |
| `childAspectRatio` | `double` | Width / Height ratio |
| `crossAxisSpacing` | `double` | Horizontal spacing |
| `mainAxisSpacing` | `double` | Vertical spacing |

---

### 39. RefreshIndicator

Pull-to-refresh wrapper.

**File:** `home_tab.dart`, `search_tab.dart`, `sell_tab.dart`

```dart
RefreshIndicator(
  onRefresh: _loadProducts,  // Async callback
  child: GridView.builder(...),
)
```

| Property | Type | Description |
|----------|------|-------------|
| `onRefresh` | `AsyncCallback` | Refresh callback |
| `child` | `Widget` | Scrollable child |

---

### 40. FutureBuilder

Builds widget based on future snapshot.

**File:** `sell_tab.dart`

```dart
FutureBuilder<List<Product>>(
  future: _productsFuture,
  builder: (context, snapshot) {
    if (snapshot.connectionState == ConnectionState.waiting) {
      return CircularProgressIndicator();
    }
    if (snapshot.hasError) {
      return Text('Error: ${snapshot.error}');
    }
    final products = snapshot.data ?? [];
    return GridView.builder(...);
  },
)
```

| Property | Type | Description |
|----------|------|-------------|
| `future` | `Future<T>` | Future to build from |
| `builder` | `Function` | Builder callback |
| `initialData` | `T?` | Initial data |

---

## Common Properties Reference

### EdgeInsets

Padding and margin values.

```dart
// All sides equal
EdgeInsets.all(16)

// Symmetric (horizontal & vertical)
EdgeInsets.symmetric(horizontal: 12, vertical: 8)

// Only specific sides
EdgeInsets.only(left: 10, right: 10, top: 5, bottom: 5)

// FromLTRB
EdgeInsets.fromLTRB(10, 5, 10, 5)
```

---

### BorderRadius

Corner rounding.

```dart
// All corners
BorderRadius.circular(12)

// Specific corners
BorderRadius.only(
  topLeft: Radius.circular(10),
  topRight: Radius.circular(10),
)
```

---

### Alignment

Widget alignment.

```dart
Alignment.center
Alignment.topLeft
Alignment.topRight
Alignment.bottomLeft
Alignment.bottomRight
Alignment.centerLeft
Alignment.centerRight
```

---

### MainAxisAlignment (Column/Row)

```dart
MainAxisAlignment.start      // Start of main axis
MainAxisAlignment.end        // End of main axis
MainAxisAlignment.center     // Center of main axis
MainAxisAlignment.spaceBetween // Evenly spaced
MainAxisAlignment.spaceAround  // Evenly spaced with half-spacing at ends
MainAxisAlignment.spaceEvenly  // Evenly spaced including ends
```

---

### CrossAxisAlignment (Column/Row)

```dart
CrossAxisAlignment.start     // Start of cross axis
CrossAxisAlignment.end       // End of cross axis
CrossAxisAlignment.center    // Center of cross axis
CrossAxisAlignment.stretch   // Stretch to fill
```

---

### BoxFit (Image fitting)

```dart
BoxFit.cover      // Fill completely (may crop)
BoxFit.contain    // Fit entirely (may have empty space)
BoxFit.fill       // Stretch to fill
BoxFit.fitWidth   // Fit width
BoxFit.fitHeight  // Fit height
BoxFit.none       // No scaling
BoxFit.scaleDown  // Scale down if needed
```

---

### Clip (Content clipping)

```dart
Clip.none         // No clipping
Clip.hardEdge     // Fast clipping
Clip.antiAlias    // Smooth clipping
Clip.antiAliasWithSaveLayer  // Smooth with save layer
```

---

### FontWeight

```dart
FontWeight.normal   // 400
FontWeight.bold     // 700
FontWeight.w100     // 100
FontWeight.w200     // 200
...
FontWeight.w900     // 900
```

---

### TextInputType

```dart
TextInputType.text          // Default text
TextInputType.emailAddress  // Email keyboard
TextInputType.number        // Number keyboard
TextInputType.phone         // Phone keyboard
TextInputType.url           // URL keyboard
TextInputType.visiblePassword  // Password keyboard
```

---

### WidgetStatePropertyAll

Used for button styling in newer Flutter versions.

```dart
ButtonStyle(
  backgroundColor: WidgetStatePropertyAll(Colors.orange),
  shape: WidgetStatePropertyAll(
    RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
  ),
)
```

---

### MediaQuery

Get screen dimensions.

```dart
// Screen height
MediaQuery.of(context).size.height

// Screen width
MediaQuery.of(context).size.width

// Example usage
height: MediaQuery.of(context).size.height * 0.3
```

---

## Widget Hierarchy Examples

### Product Card Structure

```
Card
└── InkWell (tap handler)
    └── Column
        ├── Expanded
        │   └── Stack
        │       ├── CachedNetworkImage
        │       └── Positioned (badge)
        └── Padding
            └── Column
                ├── Text (name)
                ├── SizedBox
                └── Text (price)
```

### Error State Structure

```
Center
└── Column
    ├── Icon (error_outline)
    ├── SizedBox
    ├── Text (error message)
    ├── SizedBox
    └── ElevatedButton.icon (retry)
```

### Profile Page Structure

```
Scaffold
└── SingleChildScrollView
    └── Column
        ├── Stack (avatar + camera button)
        ├── SizedBox
        ├── Text (display name)
        ├── SizedBox
        ├── Text (email)
        ├── SizedBox
        ├── InkWell (edit button)
        ├── SizedBox
        ├── Container (settings header)
        ├── SizedBox
        ├── Card
        │   └── Column
        │       └── ListTile × 3
        └── ElevatedButton (create ad)
```

---

## Quick Reference: Common Combinations

### Loading State
```dart
if (_isLoading) {
  return const Center(child: CircularProgressIndicator());
}
```

### Error State
```dart
if (_error != null) {
  return Center(
    child: Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(Icons.error_outline, size: 64, color: Colors.red[300]),
        SizedBox(height: 16),
        Text(_error!),
        SizedBox(height: 16),
        ElevatedButton.icon(
          onPressed: _retry,
          icon: Icon(Icons.refresh),
          label: Text('Попробовать снова'),
        ),
      ],
    ),
  );
}
```

### Empty State
```dart
if (_items.isEmpty) {
  return Center(
    child: Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(Icons.shopping_basket_outlined, size: 64, color: Colors.grey[400]),
        SizedBox(height: 16),
        Text('Нет товаров', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
        SizedBox(height: 8),
        Text('Здесь будут отображаться товары', style: TextStyle(color: Colors.grey[500])),
      ],
    ),
  );
}
```

---

*Document generated for Ala Avito Flutter Application*
