using Dapper;
using Npgsql;

namespace DapperTest;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Dapper Test Application");
        Console.WriteLine("======================");
        
        // ПРИНУДИТЕЛЬНО правильная строка подключения
        string connectionString = "Host=127.0.0.1;Port=5432;Database=blog_db;Username=postgres;Password=123";
        
        Console.WriteLine($"Подключение к: 127.0.0.1:5432");
        Console.WriteLine();
        
        // Подготовка тестовых авторов
        var authors = new List<Author>
        {
            new Author { Name = "Иван Петров", Bio = "Специалист по C#" },
            new Author { Name = "Мария Сидорова", Bio = "Эксперт по базам данных" },
            new Author { Name = "Алексей Иванов", Bio = "Full-stack разработчик" }
        };
        
        Console.WriteLine($"Подготовлено авторов для вставки: {authors.Count}");
        Console.WriteLine();
        
        // Проверка подключения и вставка
        try
        {
            // Сначала проверим подключение без вставки
            using (var testConnection = new NpgsqlConnection(connectionString))
            {
                testConnection.Open();
                Console.WriteLine("✅ Подключение к БД успешно!");
                testConnection.Close();
            }
            
            // Теперь выполняем массовую вставку
            int insertedCount = BulkInsertAuthors(connectionString, authors);
            Console.WriteLine($"✅ Успешно добавлено авторов: {insertedCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка: {ex.Message}");
        }
        
        Console.WriteLine();
        Console.WriteLine("Нажмите Enter для выхода...");
        Console.ReadLine();
    }
    
    public static int BulkInsertAuthors(string connectionString, IEnumerable<Author> authors)
    {
        if (authors == null || !authors.Any())
            return 0;
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            
            string sql = @"
                INSERT INTO table_authors (name, bio) 
                VALUES (@Name, @Bio)";
            
            int affectedRows = connection.Execute(sql, authors);
            return affectedRows;
        }
    }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }
}