using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    public class DbSqlExecuter
    {
        private const string MasterDatabaseName = "master";

        private readonly string _dataSource;
        private readonly string _initialCatalog;
        private readonly string _password;
        private readonly string _userId;

        public DbSqlExecuter(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            _initialCatalog = connectionStringBuilder.InitialCatalog;
            _dataSource = connectionStringBuilder.DataSource;
            _userId = connectionStringBuilder.UserID;
            _password = connectionStringBuilder.Password;
        }

        public DbSqlExecuter(string initialCatalog, string dataSource, string userId, string password)
        {
            if (string.IsNullOrWhiteSpace(initialCatalog)) throw new ArgumentNullException("initialCatalog");
            if (string.IsNullOrWhiteSpace(dataSource)) throw new ArgumentNullException("dataSource");
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("password");

            _initialCatalog = initialCatalog;
            _dataSource = dataSource;
            _userId = userId;
            _password = password;
        }

        private SqlConnection CreateConnection(string databaseName)
        {
            return new SqlConnection(GetConnectionString(databaseName));
        }

        public void CreateDatabase(string databaseName, bool force = false)
        {
            if (force)
                DropDatabase(databaseName);
            var sentence = string.Format(
                @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                BEGIN
                    CREATE DATABASE [{0}]
                END",
                databaseName);
            ExecuteSentence(sentence, MasterDatabaseName);
        }

        public void DropDatabase(string databaseName)
        {
            var sentence = string.Format(
                @"IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
                BEGIN
                    ALTER DATABASE [{0}] SET single_user WITH ROLLBACK IMMEDIATE
                    DROP DATABASE [{0}]
                END",
                databaseName);
            ExecuteSentence(sentence, MasterDatabaseName);
        }

        public void ExecuteScriptFile(string path)
        {
            var sentences = GetSentencesFromFile(path);

            using (var connection = CreateConnection(_initialCatalog))
            {
                connection.Open();

                foreach (var sentence in sentences)
                {
                    ExecuteSentence(sentence, connection);
                }
            }
        }

        public void ExecuteScriptFilesFromFolder(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("scriptsPath not found");

            const string searchPattern = "*.sql";
            var files = Directory.GetFiles(path, searchPattern).OrderBy(p => p);
            if (!files.Any())
                return;

            foreach (var file in files)
            {
                ExecuteScriptFile(file);
            }
        }

        private int ExecuteSentence(string sentence, string databaseName)
        {
            using (var connection = CreateConnection(databaseName))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sentence;
                    cmd.CommandType = CommandType.Text;
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private static int ExecuteSentence(string sentence, SqlConnection connection)
        {
            var close = false;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                close = true;
            }

            int retval;
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sentence;
                    cmd.CommandType = CommandType.Text;
                    retval = cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (close)
                {
                    connection.Close();
                }
            }
            return retval;
        }

        public int ExecuteSentence(string sentence)
        {
            return ExecuteSentence(sentence, _initialCatalog);
        }

        private string GetConnectionString(string databaseName)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = databaseName,
                DataSource = _dataSource,
                Pooling = false,
                MultipleActiveResultSets = true
            };
            if (!string.IsNullOrWhiteSpace(_userId))
            {
                connectionStringBuilder.UserID = _userId;
                connectionStringBuilder.Password = _password;
            }
            else
            {
                connectionStringBuilder.IntegratedSecurity = true;
            }
            return connectionStringBuilder.ConnectionString;
        }

        private static IEnumerable<string> GetSentencesFromFile(string path)
        {
            return GetSentencesFromText(File.ReadAllText(path));
        }

        private static IEnumerable<string> GetSentencesFromText(string text)
        {
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Split(text).Where(l => !string.IsNullOrWhiteSpace(l));
        }
    }
}