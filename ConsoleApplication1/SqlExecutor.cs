using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    public class SqlExecutor
    {
        private readonly string _connectionString;

        public SqlExecutor(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            _connectionString = connectionString;
        }

        public void ExecuteScripts(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(nameof(folderPath));
            }

            var files = Directory.GetFiles(folderPath, "*.sql");
            if (!files.Any())
            {
                return;
            }
            foreach (var file in files.OrderBy(f => f))
            {
                ExecuteScript(file);
            }
        }

        public void CreateDatabase(string databaseName)
        {
            var sentence =
                $@"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}') 
                                BEGIN
                                    CREATE DATABASE [{databaseName}]
                                END";
            ExecuteSentence(sentence, GetMasterConnectionString(_connectionString));
        }

        private static string GetMasterConnectionString(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master",
                Pooling = false
            }.ConnectionString;
        }

        public void DropDatabase(string databaseName)
        {
            var sentence =
                $@"IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                                BEGIN
                                    ALTER DATABASE [{databaseName}] SET single_user WITH ROLLBACK IMMEDIATE
                                    DROP DATABASE [{databaseName}]
                                END";
            ExecuteSentence(sentence, GetMasterConnectionString(_connectionString));
        }

        private static IEnumerable<string> ParseScript(string filePath)
        {
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Split(File.ReadAllText(filePath)).Where(l => !string.IsNullOrWhiteSpace(l));
        }

        public void ExecuteScript(string filePath)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    foreach (var line in ParseScript(filePath))
                    {
                        cmd.CommandText = line;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void ExecuteSentence(string sentence, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sentence;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ExecuteSentence(string sentence)
        {
            ExecuteSentence(sentence, _connectionString);
        }
    }
}