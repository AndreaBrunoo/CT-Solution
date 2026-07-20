using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        var dbPath = @"C:\Users\StageOne\Documents\CT-Solution\Api\AuthDatabase.db";
        using var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();

        string[] tables = { "User", "Employee", "Worklog", "Category", "Company", "Project", "Role", "Status" };

        foreach (var table in tables)
        {
            Console.WriteLine($"--- {table} Columns ---");
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"PRAGMA table_info({table});";
                using var reader = cmd.ExecuteReader();
                bool hasDeletedAt = false;
                bool hasIsDeleted = false;
                while (reader.Read())
                {
                    var name = reader["name"].ToString();
                    Console.WriteLine($"- {name} ({reader["type"]})");
                    if (name == "DeletedAt") hasDeletedAt = true;
                    if (name == "IsDeleted") hasIsDeleted = true;
                }
                Console.WriteLine($"Table '{table}' has DeletedAt: {hasDeletedAt}, IsDeleted: {hasIsDeleted}");
            }
            Console.WriteLine();
        }
    }
}
