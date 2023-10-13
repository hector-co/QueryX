namespace QueryX.Tests
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public float Price { get; set; }
        public float Stock { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool Active { get; set; }
    }

    public static class Collections
    {
        public static Product[] Products =>
            new[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Product1",
                    Description = "Description1",
                    Price = 10,
                    Stock = 0,
                    RegisteredAt = DateTime.Parse("2023-6-1"),
                    Active = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Product2",
                    Description = "Description2",
                    Price = 20,
                    Stock = 0,
                    RegisteredAt = DateTime.Parse("2023-6-15"),
                    Active = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Product3",
                    Description = "description3",
                    Price = 30,
                    Stock = 10,
                    RegisteredAt = DateTime.Parse("2023-6-30"),
                    Active = true
                },
                new Product
                {
                    Id = 4,
                    Name = "new Product1",
                    Description = "description4",
                    Price = 40,
                    Stock = 10,
                    RegisteredAt = DateTime.Parse("2023-7-1"),
                    Active = true
                },
                new Product
                {
                    Id = 5,
                    Name = "new Product2",
                    Price = 50,
                    Stock = 20,
                    RegisteredAt = DateTime.Parse("2023-7-15"),
                    Active = true
                },
                new Product
                {
                    Id = 6,
                    Name = "new Product3",
                    Price = 60,
                    Stock = 20,
                    RegisteredAt = DateTime.Parse("2023-7-30"),
                    Active = false
                },
                new Product
                {
                    Id = 7,
                    Name = "Custom1",
                    Price = 70,
                    Stock = 30,
                    RegisteredAt = DateTime.Parse("2023-8-1"),
                    Active = false
                },
                new Product
                {
                    Id = 8,
                    Name = "Custom2",
                    Price = 80,
                    Stock = 30,
                    RegisteredAt = DateTime.Parse("2023-8-15"),
                    Active = false
                },
                new Product
                {
                    Id = 9,
                    Name = "custom3",
                    Price = 90,
                    Stock = 40,
                    RegisteredAt = DateTime.Parse("2023-8-30"),
                    Active = false
                },
                new Product
                {
                    Id = 10,
                    Name = "custom4",
                    Price = 100,
                    Stock = 40,
                    RegisteredAt = DateTime.Parse("2023-9-1"),
                    Active = false
                }
            };
    }
}
