using Microsoft.Data.Sqlite;
/// <summary>
/// Helper class for working with SQLite databases.
/// </summary>
public static class SQLiteHelper
{
    /// <summary>
    /// Loads a SQLite database from a file and returns an open connection.
    /// </summary>
    /// <param name="sqliteDbPath">Path to the SQLite database file.</param>
    /// <returns>An open <see cref="SqliteConnection"/>.</returns>
    public static SqliteConnection LoadDatabase(string sqliteDbPath)
    {
        var connection = new SqliteConnection($"Data Source={sqliteDbPath}");
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Executes a non-query SQL statement (e.g., INSERT, UPDATE, DELETE, or DDL) against the database.
    /// </summary>
    /// <param name="connection">An open <see cref="SqliteConnection"/>.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    public static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a SQL query that returns rows and returns the results as a list of dictionaries.
    /// Each dictionary represents a row with column names as keys.
    /// </summary>
    /// <param name="connection">An open <see cref="SqliteConnection"/>.</param>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A list of dictionaries representing the query result rows.</returns>
    public static List<Dictionary<string, object>> ExecuteQuery(SqliteConnection connection, string sql)
    {
        var results = new List<Dictionary<string, object>>();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // If the field is DBNull, assign null.
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }
        return results;
    }

    /// <summary>
    /// Executes a SQL statement that creates a new table or view.
    /// </summary>
    /// <param name="connection">An open SQLite connection.</param>
    /// <param name="createQuery">The CREATE TABLE or CREATE VIEW SQL statement.</param>
    public static void CreateTableOrView(SqliteConnection connection, string createQuery)
    {
        using var command = connection.CreateCommand();
        command.CommandText = createQuery;
        command.ExecuteNonQuery();
    }

    public static void DropTableOrView(SqliteConnection connection, string objectName, string objectType = "table")
    {
        string sql;
        if (objectType.Equals("table", StringComparison.OrdinalIgnoreCase))
        {
            sql = $"DROP TABLE IF EXISTS [{objectName}];";
        }
        else if (objectType.Equals("view", StringComparison.OrdinalIgnoreCase))
        {
            sql = $"DROP VIEW IF EXISTS [{objectName}];";
        }
        else
        {
            throw new ArgumentException("objectType must be either 'table' or 'view'");
        }

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public static void UpdateTableRow(
    SqliteConnection connection,
    string tableName,
    string keyColumn,
    object keyValue,
    Dictionary<string, object> updatedValues)
    {
        // Build the SET clause from the dictionary.
        var setClauses = updatedValues.Select((kv, i) => $"[{kv.Key}] = @p{i}").ToList();
        string setClause = string.Join(", ", setClauses);
        string sql = $"UPDATE [{tableName}] SET {setClause} WHERE [{keyColumn}] = @keyValue;";

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // Add parameters for updated columns.
        int index = 0;
        foreach (var kv in updatedValues)
        {
            command.Parameters.AddWithValue($"@p{index}", kv.Value);
            index++;
        }
        // Add the key value parameter.
        command.Parameters.AddWithValue("@keyValue", keyValue);

        command.ExecuteNonQuery();
    }

    public static List<string> GetTableColumns(SqliteConnection connection, string tableName)
    {
        var columns = new List<string>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"PRAGMA table_info([{tableName}]);";
            using (var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    columns.Add(reader["name"].ToString());
                }
            }
        }
        return columns;
    }
}
