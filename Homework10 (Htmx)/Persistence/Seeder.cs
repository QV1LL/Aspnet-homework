using Homework10__Htmx_.Models;

namespace Homework10__Htmx_.Persistence;

public static class Seeder
{
    public static void Seed(this ContactsAppContext dbContext) =>
        dbContext.Contacts.AddRange(
            new Contact { Name = "Олександр Коваленко", Email = "alex.k@example.com", Phone = "+380501234567" },
            new Contact { Name = "Марія Петренко", Email = "mariya.p@example.com", Phone = "+380671234567" },
            new Contact { Name = "Іван Сірко", Email = "sirko@history.ua", Phone = "+380631112233" },
            new Contact { Name = "Олена Пчілка", Email = "pchilka@lit.ua" },
            new Contact { Name = "Дмитро Яворницький", Phone = "+380445556677" }
        );
}