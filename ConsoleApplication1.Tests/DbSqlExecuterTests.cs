using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;

namespace ConsoleApplication1.Tests
{
    [TestFixture]
    public class DbSqlExecuterTests
    {
        private string _initialCatalog;
        private string _dataSource;
        private string _userId;
        private string _password;
        private DbSqlExecuter _sut;
        private string _folderPath;
        private const string ScriptsDirectoryName = "Scripts";

        [TestFixtureSetUp]
        public void SetUp()
        {
            _initialCatalog = "TestDB";
            _dataSource = "(localdb)\\MSSQLLocalDB";
            _userId = "sa";
            _password = "sqltab2005";

            _sut = GetNewDbSqlExecuter();
            _folderPath = Path.Combine(Environment.CurrentDirectory, ScriptsDirectoryName);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _sut.DropDatabase(_initialCatalog);
        }

        private bool TryConnect(string databaseName)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = databaseName,
                DataSource = _dataSource,
                UserID = _userId,
                Password = _password
            };
            var connection = new SqlConnection(connectionStringBuilder.ConnectionString);

            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private DbSqlExecuter GetNewDbSqlExecuter()
        {
            return new DbSqlExecuter(_initialCatalog, _dataSource, _userId, _password);
        }

        [Test]
        public void CreateDatabaseIfNotExists_new_database_will_be_created()
        {
            // Arrange
            // Act
            _sut.CreateDatabase(_initialCatalog);

            // Assert
            Assert.IsTrue(TryConnect(_initialCatalog));
        }

        [Test]
        public void DropDatabaseIfExists_existing_database_will_be_deleted()
        {
            // Arrange
            // Act
            _sut.DropDatabase(_initialCatalog);

            // Assert
            Assert.IsFalse(TryConnect(_initialCatalog));
        }

        [Test]
        public void ExecuteScriptFile_will_execute_the_file()
        {
            // Arrange
            _sut.CreateDatabase(_initialCatalog);

            // Act
            _sut.ExecuteScriptFile(Path.Combine(_folderPath, "1 - Customers.sql"));

            // Assert
            Assert.Pass();
        }

        [Test]
        public void ExecuteScriptFilesFromFolder_will_execute_all_scripts()
        {
            // Arrange
            _sut.CreateDatabase(_initialCatalog, true);

            // Act
            _sut.ExecuteScriptFilesFromFolder(_folderPath);

            // Assert
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteScriptFilesFromFolder_with_null_path_throw_ArgumentNullException()
        {
            _sut.ExecuteScriptFilesFromFolder(null);
        }

        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void ExecuteScriptFilesFromFolder_with_wrong_path_throw_DirectoryNotFoundException()
        {
            // Arrage
            const string path = "C:\\NoExistsDirectory";

            // Act
            _sut.ExecuteScriptFilesFromFolder(path);
        }

        [Test]
        public void ExecuteSentence()
        {
            // Arrange
            _sut.CreateDatabase(_initialCatalog);

            // Act
            _sut.ExecuteSentence("SELECT @@VERSION");

            // Assert
            Assert.Pass();
        }
    }
}
