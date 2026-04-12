using System.Data;
using System.Data.Common;

namespace ADO.NET.DAL;

public static class ExtensionDbCommand
{
    // public static void AddParameterWithValue(this DbCommand command, string name, object value)
    // {
    //     
    //     var parameter = command.CreateParameter();
    //     parameter.ParameterName = name;
    //     parameter.Value = value;
    //     parameter.Direction = ParameterDirection.Input;
    // }
    
    public static void AddParameterWithValue(
        this DbCommand command, 
        string name,
        object? value,
        ParameterDirection direction =  ParameterDirection.Input,
        DbType? dbType = null
        )
    {
        
        var parameter = command.CreateParameter();
        
        parameter.ParameterName = name.StartsWith("@") ? name : "@" + name;
        parameter.Value = value ??  DBNull.Value;
        parameter.Direction = direction;
        
        if (dbType.HasValue) parameter.DbType = dbType.Value;
        
        command.Parameters.Add(parameter);
    }
}