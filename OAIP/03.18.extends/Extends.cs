namespace OAIP
{
    using static System.Console;
    internal class Extends : Object
    {
        public bool isDevoperEdition;

        public Extends(bool isDevoperEdition)
        {
            Clear();

            WriteLine("Практика 18.03");
            WriteLine("Наследование");

            Admin admin = new Admin("AdminName", "admin@example.com");
            admin.ManageUsers();
            admin.DisplayInfo();

            Customer customer = new Customer("CustomerName", "customer@example.com", 100);
            customer.DisplayInfo();

            Product product1 = new Product("Laptop", 1000);
            Product product2 = new Product("Mouse", 25);
            Product product3 = new Product("Keyboard", 75);

            Order order = new Order(customer);
            order.Products.Add(product1);
            order.Products.Add(product2);

            WriteLine("Содержимое корзины:");
            foreach (var product in order.Products)
            {
                WriteLine($"- {product.Name}: {product.Price:C}");
            }

            WriteLine("\nПопытка оформить заказ с недостаточным балансом:");
            customer.PlaceOrder(new List<Product> { product1, product2, product3 }); // Сумма > баланса

            order.Products.Add(product3);

            WriteLine("\nСодержимое корзины после добавления продукта:");
            foreach (var product in order.Products)
            {
                WriteLine($"- {product.Name}: {product.Price:C}");
            }

            WriteLine("\nПопытка оформить заказ с достаточным балансом:");
            customer.PlaceOrder(order.Products);

            WriteLine("\nОбновлённый баланс пользователя:");
            customer.DisplayInfo();


            WriteLine("\nДополнительное задание: VIP клиент");
            VipCustomer vipCustomer = new VipCustomer("VipCustomer", "vip@example.com", 2000);
            vipCustomer.DisplayInfo();

            Order vipOrder = new Order(vipCustomer);
            vipOrder.Products.Add(product1);
            vipOrder.Products.Add(product2);
            vipOrder.Products.Add(product3);
            WriteLine($"Общая стоимость товаров в заказе VIP клиента до скидки: {vipOrder.GetTotalPrice():C}");
            vipCustomer.PlaceOrder(vipOrder.Products);
            vipCustomer.DisplayInfo();

            WriteLine("\nДополнительное задание: Удаление товара из заказа");
            vipOrder.Products.Remove(product2);
            WriteLine("Содержимое корзины после удаления товара:");
            foreach (var product in vipOrder.Products)
            {
                WriteLine($"- {product.Name}: {product.Price:C}");
            }

            WriteLine($"Общая стоимость товаров в заказе VIP клиента после удаления товара: {vipOrder.GetTotalPrice():C}");
            vipCustomer.PlaceOrder(vipOrder.Products);
            vipCustomer.DisplayInfo();

            WriteLine("\nДополнительное задание: Категории товаров");
            Category electronics = new Category("Electronics");
            electronics.AddProduct(product1);
            electronics.AddProduct(product2);
            electronics.DisplayProducts();

            Category books = new Category("Books");
            Product book1 = new Product("Clean Code", 50);
            books.AddProduct(book1);
            books.DisplayProducts();

            ReadKey();
        }

        private class User
        {
            public string Name;
            public string Email;

            public virtual void DisplayInfo()
            {
                WriteLine($"Имя: {Name}, Email: {Email}");
            }
        }

        private class Customer : User
        {
            public decimal Balance;

            public Customer(string name, string email, decimal balance)
            {
                Name = name;
                Email = email;
                Balance = balance;
            }

            public virtual bool PlaceOrder(List<Product> products)
            {
                decimal totalPrice = products.Sum(p => p.Price);
                if (Balance >= totalPrice)
                {
                    Balance -= totalPrice;
                    WriteLine("Заказ успешно оформлен.");
                    return true;
                }
                else
                {
                    WriteLine("Недостаточно средств на балансе.");
                    return false;
                }
            }

            public override void DisplayInfo()
            {
                base.DisplayInfo();
                WriteLine($"Баланс: {Balance:C}");
            }
        }

        private class Admin : User
        {
            public Admin(string name, string email)
            {
                Name = name;
                Email = email;
            }

            public void ManageUsers()
            {
                WriteLine("Администратор управляет пользователями.");
            }

            public override void DisplayInfo()
            {
                base.DisplayInfo();
                WriteLine("Администратор");
            }
        }

        private class Product
        {
            public string Name;
            public decimal Price;

            public Product(string name, decimal price)
            {
                Name = name;
                Price = price;
            }
        }

        private class Order
        {
            public Customer Customer;
            public List<Product> Products;

            public Order(Customer customer)
            {
                Customer = customer;
                Products = new List<Product>();
            }

            public decimal GetTotalPrice()
            {
                return Products.Sum(p => p.Price);
            }

            public void RemoveProduct(Product product)
            {
                Products.Remove(product);
            }
        }

        private class VipCustomer : Customer
        {
            public VipCustomer(string name, string email, decimal balance) : base(name, email, balance) { }

            public override bool PlaceOrder(List<Product> products)
            {
                decimal totalPrice = products.Sum(p => p.Price * 0.9m); // Скидка 10%
                if (Balance >= totalPrice)
                {
                    Balance -= totalPrice;
                    WriteLine("Заказ успешно оформлен (с учетом VIP скидки).");
                    return true;
                }
                else
                {
                    WriteLine("Недостаточно средств на балансе.");
                    return false;
                }
            }

            public override void DisplayInfo()
            {
                base.DisplayInfo();
                WriteLine("VIP Клиент");
            }
        }


        private class Category
        {
            public string Name;
            public List<Product> Products;

            public Category(string name)
            {
                Name = name;
                Products = new List<Product>();
            }

            public void AddProduct(Product product)
            {
                Products.Add(product);
            }

            public void DisplayProducts()
            {
                WriteLine($"Категория: {Name}");
                foreach (var product in Products)
                {
                    WriteLine($"- {product.Name}: {product.Price:C}");
                }
            }
        }
    }
}
