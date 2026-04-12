using ADO.NET.DAL.ConnectionStringProvider;

namespace ConnectionStringTest;

class Program
{
    static void Main(string[] args)
    {
        // var xmlConnectionString = ConnectionStringProvider.GetConnectionStringFromXml();
        //
        // var jsonConnectionString = ConnectionStringProvider.GetConnectionStringFromJson();
        //
        // var userSecretsConnectionString  = ConnectionStringProvider.GetConnectionStringFromUserSecrets();
        //
        // var envConnectionString = ConnectionStringProvider.GetConnectionStringFromEnv();
        //
        // Console.WriteLine(nameof(xmlConnectionString) +  ":" + xmlConnectionString);
        // Console.WriteLine();
        // Console.WriteLine(nameof(jsonConnectionString) +  ":" + jsonConnectionString);
        // Console.WriteLine();
        // Console.WriteLine(nameof(userSecretsConnectionString) +  ":" + userSecretsConnectionString);
        // Console.WriteLine();
        // Console.WriteLine(nameof(envConnectionString) +  ":" + envConnectionString);

        var connectionString = ConnectionStringProvider.GetConnectionString();
        
        Console.WriteLine();
        Console.WriteLine(nameof(connectionString) +  ":" + connectionString);
        
        Console.ReadLine();
        
        connectionString = ConnectionStringProvider.GetConnectionString();
        
        Console.WriteLine();
        Console.WriteLine(nameof(connectionString) +  "New :" + connectionString);


    }
}