namespace QueryX.Tests
{
    public enum ProducType
    {
        RawMaterial,
        FinishedGoods,
        Consumable,
        Service
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public float Price { get; set; }
        public float? Stock { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool Active { get; set; }
        public int Type { get; set; }
        public int? Order { get; set; }
    }

    public class ShoppingCart
    {
        public int Id { get; set; }
        public List<ShoppingCartLine> Lines { get; set; }
        public ShoppingCartStatus Status { get; set; }
    }

    public class ShoppingCartLine
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public float Quantity { get; set; }
    }

    public class Customer
    {
        public ShoppingCart CurrentShoppingCart { get; set; }
    }

    public enum ShoppingCartStatus
    {
        Pending,
        Confirmed,
        Canceled
    }

    public static class Collections
    {
        public static Category[] Categories =>
            new[]
            {
                new Category
                {
                    Id = 1,
                    Name = "Category1"
                },
                new Category
                {
                    Id = 2,
                    Name = "Category2"
                },
                new Category
                {
                    Id = 3,
                    Name = "Category3"
                }
            };

        public static Product[] Products =>
            new[]
            {
                new Product
                {
                    Id = 1,
                    Category = Categories[0],
                    Name = "Product1",
                    Description = "Description1",
                    Price = 10,
                    Stock = 40,
                    RegisteredAt = DateTime.Parse("2023-6-1"),
                    Active = true,
                    Type = (int)ProducType.RawMaterial,
                    Order = 10
                },
                new Product
                {
                    Id = 2,
                    Category = Categories[1],
                    Name = "Product2",
                    Description = "Description2",
                    Price = 20,
                    Stock = 35,
                    RegisteredAt = DateTime.Parse("2023-6-15"),
                    Active = true,
                    Type = (int)ProducType.RawMaterial
                },
                new Product
                {
                    Id = 3,
                    Category = Categories[2],
                    Name = "Product3",
                    Description = "description3",
                    Price = 30,
                    Stock = 30,
                    RegisteredAt = DateTime.Parse("2023-6-30"),
                    Active = true,
                    Type = (int)ProducType.FinishedGoods,
                    Order = 2
                },
                new Product
                {
                    Id = 4,
                    Category = Categories[0],
                    Name = "new Product1",
                    Description = "description4",
                    Price = 40,
                    Stock = 25,
                    RegisteredAt = DateTime.Parse("2023-7-1"),
                    Active = true,
                    Type = (int)ProducType.FinishedGoods
                },
                new Product
                {
                    Id = 5,
                    Category = Categories[1],
                    Name = "new Product2",
                    Price = 50,
                    Stock = 20,
                    RegisteredAt = DateTime.Parse("2023-7-15"),
                    Active = true,
                    Type = (int)ProducType.Consumable,
                    Order = 5
                },
                new Product
                {
                    Id = 6,
                    Category = Categories[2],
                    Name = "new Product3",
                    Price = 60,
                    Stock = 15,
                    RegisteredAt = DateTime.Parse("2023-7-30"),
                    Active = false,
                    Type = (int)ProducType.Consumable
                },
                new Product
                {
                    Id = 7,
                    Category = Categories[0],
                    Name = "Custom1",
                    Price = 70,
                    Stock = 10,
                    RegisteredAt = DateTime.Parse("2023-8-1"),
                    Active = false,
                    Type = (int)ProducType.Service,
                    Order = 20
                },
                new Product
                {
                    Id = 8,
                    Category = Categories[1],
                    Name = "Custom2",
                    Price = 80,
                    Stock = 5,
                    RegisteredAt = DateTime.Parse("2023-8-15"),
                    Active = false,
                    Type = (int)ProducType.Service
                },
                new Product
                {
                    Id = 9,
                    Category = Categories[2],
                    Name = "custom3",
                    Price = 90,
                    Stock = 0,
                    RegisteredAt = DateTime.Parse("2023-8-30"),
                    Active = false,
                    Type = (int)ProducType.FinishedGoods,
                    Order = 15
                },
                new Product
                {
                    Id = 10,
                    Category = Categories[0],
                    Name = "custom4",
                    Price = 100,
                    Stock = 2,
                    RegisteredAt = DateTime.Parse("2023-9-1"),
                    Active = false,
                    Type = (int)ProducType.FinishedGoods
                }
            };

        public static ShoppingCartLine[] ShoppingCartLines =>
            new[]
            {
                new ShoppingCartLine
                    {
                        Id = 1,
                        Product = Products[0],
                        Quantity = 5
                    },
                    new ShoppingCartLine
                    {
                        Id = 2,
                        Product = Products[1],
                        Quantity = 10
                    },
                    new ShoppingCartLine
                    {
                        Id = 3,
                        Product = Products[2],
                        Quantity = 15
                    },
                    new ShoppingCartLine
                    {
                        Id = 4,
                        Product = Products[0],
                        Quantity = 20
                    },
                    new ShoppingCartLine
                    {
                        Id = 5,
                        Product = Products[2],
                        Quantity = 25
                    },
                    new ShoppingCartLine
                    {
                        Id = 6,
                        Product = Products[4],
                        Quantity = 30
                    },
                    new ShoppingCartLine
                    {
                        Id = 7,
                        Product = Products[0],
                        Quantity = 35
                    },
                    new ShoppingCartLine
                    {
                        Id = 8,
                        Product = Products[3],
                        Quantity = 40
                    },
                    new ShoppingCartLine
                    {
                        Id = 9,
                        Product = Products[6],
                        Quantity = 45
                    }
            };

        public static ShoppingCart[] ShoppingCarts =>
            new[]
            {
                new ShoppingCart
                {
                    Id = 1,
                    Status = ShoppingCartStatus.Pending,
                    Lines = new[]
                    {
                        ShoppingCartLines[0],
                        ShoppingCartLines[1],
                        ShoppingCartLines[2]
                    }.ToList()
                },
                new ShoppingCart
                {
                    Id = 2,
                    Status = ShoppingCartStatus.Confirmed,
                    Lines = new[]
                    {
                        ShoppingCartLines[3],
                        ShoppingCartLines[4],
                        ShoppingCartLines[5]
                    }.ToList()
                },
                new ShoppingCart
                {
                    Id = 3,
                    Status = ShoppingCartStatus.Canceled,
                    Lines = new[]
                    {
                        ShoppingCartLines[6],
                        ShoppingCartLines[7],
                        ShoppingCartLines[8]
                    }.ToList()
                }
            };

        public static Customer[] Customers =>
            new[]
            {
                new Customer
                {
                    CurrentShoppingCart = ShoppingCarts[0]
                },
                new Customer
                {
                    CurrentShoppingCart = ShoppingCarts[1]
                },
                new Customer
                {
                    CurrentShoppingCart = ShoppingCarts[2]
                }
            };
    }
}
