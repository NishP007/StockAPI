using StockAPI.Data;
using StockAPI.Entities;
using System;
using System.Linq;


namespace StockAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    Password = "admin123", // You should hash this in real scenarios
                    Role = "Admin",
                  
                });
                context.SaveChanges();
            }
        }
    }
}
