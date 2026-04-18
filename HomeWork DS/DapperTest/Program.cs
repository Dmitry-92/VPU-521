using Dapper;
using Npgsql;

namespace DapperTest;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Dapper Test Application");
        Console.WriteLine("======================");
        
        string connectionString = GetConnectionString();
        
        // Создаём базу данных и таблицы, если их нет
        EnsureDatabaseExists();
        EnsureTableExists();
        EnsurePostsTableExists(); // Таблица для постов
        
        // ============================================================
        // ЗАДАНИЕ №1: Массовая вставка (Batch Insert)
        // ============================================================
        
        var authors = new List<Author>
        {
            new Author { Name = "Иван Петров", Bio = "Специалист по C#" },
            new Author { Name = "Мария Сидорова", Bio = "Эксперт по базам данных" },
            new Author { Name = "Алексей Иванов", Bio = "Full-stack разработчик" }
        };
        
        int insertedCount = BulkInsertAuthors(connectionString, authors);
        Console.WriteLine($"\nЗадание №1 - Добавлено авторов: {insertedCount}");
        
        // ============================================================
        // ЗАДАНИЕ №2: Маппинг «Связанные сущности» (Many-to-One)
        // ============================================================
        
        // Создаём тестовые посты для проверки
        EnsureTestPostsExist(connectionString);
        
        // Получаем посты с авторами
        var postsWithAuthors = GetPostsWithAuthors(connectionString);
        
        Console.WriteLine($"\nЗадание №2 - Получено постов: {postsWithAuthors.Count()}");
        Console.WriteLine("\nСписок постов с авторами:");
        Console.WriteLine(new string('-', 60));
        
        foreach (var post in postsWithAuthors)
        {
            Console.WriteLine($"Пост: {post.Title}");
            Console.WriteLine($"  Автор: {post.Author?.Name ?? "Неизвестен"}");
            Console.WriteLine($"  Содержание: {(post.Content?.Length > 50 ? post.Content.Substring(0, 50) + "..." : post.Content)}");
            Console.WriteLine();
        }
        
        Console.ReadLine();
    }
    
    // ============================================================
    // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
    // ============================================================
    
    static string _adminConnectionString = "Host=127.0.0.1;Port=5432;Database=postgres;Username=postgres;Password=123";
    static string _blogConnectionString = "Host=127.0.0.1;Port=5432;Database=blog_db;Username=postgres;Password=123";
    
    public static string GetConnectionString()
    {
        return _blogConnectionString;
    }
    
    // Создание базы данных
    public static void EnsureDatabaseExists()
    {
        using (var connection = new NpgsqlConnection(_adminConnectionString))
        {
            connection.Open();
            
            var dbExists = connection.QueryFirstOrDefault<int?>(
                "SELECT 1 FROM pg_database WHERE datname = 'blog_db'");
            
            if (dbExists == null)
            {
                connection.Execute("CREATE DATABASE blog_db");
                Console.WriteLine("✅ База данных blog_db создана");
            }
            else
            {
                Console.WriteLine("✅ База данных blog_db уже существует");
            }
        }
    }
    
    // Создание таблицы authors
    public static void EnsureTableExists()
    {
        using (var connection = new NpgsqlConnection(_blogConnectionString))
        {
            connection.Open();
            
            string sql = @"
                CREATE TABLE IF NOT EXISTS table_authors (
                    id SERIAL PRIMARY KEY,
                    name TEXT NOT NULL,
                    bio TEXT
                );";
            
            connection.Execute(sql);
            Console.WriteLine("✅ Таблица table_authors создана или уже существует");
        }
    }
    
    // Создание таблицы posts
    public static void EnsurePostsTableExists()
    {
        using (var connection = new NpgsqlConnection(_blogConnectionString))
        {
            connection.Open();
            
            string sql = @"
                CREATE TABLE IF NOT EXISTS table_posts (
                    id SERIAL PRIMARY KEY,
                    title TEXT NOT NULL,
                    content TEXT,
                    author_id INTEGER REFERENCES table_authors(id)
                );";
            
            connection.Execute(sql);
            Console.WriteLine("✅ Таблица table_posts создана или уже существует");
        }
    }
    
    // Создание тестовых постов
    public static void EnsureTestPostsExist(string connectionString)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            
            var postCount = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM table_posts");
            
            if (postCount == 0)
            {
                var authors = connection.Query<Author>("SELECT id, name FROM table_authors").ToList();
                
                if (authors.Any())
                {
                    var posts = new List<Post>();
                    
                    foreach (var author in authors)
                    {
                        posts.Add(new Post
                        {
                            Title = $"Пост от {author.Name}: Введение в C#",
                            Content = "Это содержание поста о языке программирования C#. Здесь подробно рассказывается об основах.",
                            AuthorId = author.Id
                        });
                        
                        posts.Add(new Post
                        {
                            Title = $"Пост от {author.Name}: Работа с Dapper",
                            Content = "Dapper — это микро-ORM для .NET, которая позволяет легко работать с базами данных.",
                            AuthorId = author.Id
                        });
                    }
                    
                    string insertSql = @"
                        INSERT INTO table_posts (title, content, author_id) 
                        VALUES (@Title, @Content, @AuthorId)";
                    
                    connection.Execute(insertSql, posts);
                    Console.WriteLine($"✅ Создано тестовых постов: {posts.Count}");
                }
            }
            else
            {
                Console.WriteLine($"✅ Постов в базе: {postCount}");
            }
        }
    }
    
    // ============================================================
    // ЗАДАНИЕ №1: Метод массовой вставки авторов
    // ============================================================
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
    
    // ============================================================
    // ЗАДАНИЕ №2: Метод получения постов с авторами
    // ============================================================
    public static IEnumerable<Post> GetPostsWithAuthors(string connectionString)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            
            string sql = @"
                SELECT 
                    p.id, 
                    p.title, 
                    p.content, 
                    p.author_id,
                    a.id, 
                    a.name, 
                    a.bio
                FROM table_posts p
                LEFT JOIN table_authors a ON p.author_id = a.id
                ORDER BY p.id";
            
            // splitOn: указываем колонку, с которой начинаются поля Author (a.id)
            var posts = connection.Query<Post, Author, Post>(
                sql,
                (post, author) =>
                {
                    post.Author = author;
                    return post;
                },
                splitOn: "id"
            );
            
            return posts.ToList();
        }
    }
}

// ============================================================
// МОДЕЛИ
// ============================================================

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }
    public List<Post> Posts { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int AuthorId { get; set; }
    
    // Это свойство заполняется в Задании №2
    public Author Author { get; set; }
}