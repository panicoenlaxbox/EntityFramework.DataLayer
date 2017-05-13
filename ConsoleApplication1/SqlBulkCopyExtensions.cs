using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;

namespace ConsoleApplication1
{
    public static class SqlBulkCopyExtensions
    {
        public static DataTable ConvertToOrderedDataTable<T>(this IEnumerable<T> that,
            string tableName, SqlConnection connection) where T : class
        {
            var table = ConvertToDataTable(that, tableName);
            var columnNames = GetColumnNames(tableName, connection);
            SortColumns(table, columnNames);
            RemoveNotExistingColumns(table, columnNames);
            return table;
        }

        public static DataTable ConvertToOrderedDataTable<T>(this IEnumerable<T> that, string tableName,
            string connectionString) where T : class
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return ConvertToOrderedDataTable(that, tableName, connection);
            }
        }

        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> that, string tableName = null)
        {
            var table = new DataTable(tableName ?? typeof(T).Name);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                table.Columns.Add(property.Name, GetDataColumnType(property));
            }

            foreach (var item in that)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item, null);
                    if (value != null && value.ToString().Trim() == "")
                    {
                        value = null;
                    }
                    values[i] = value;
                }

                table.Rows.Add(values);
            }

            return table;
        }

        private static Type GetDataColumnType(PropertyInfo property)
        {
            var type = property.PropertyType;
            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                type = Nullable.GetUnderlyingType(property.PropertyType);
            }
            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }
            return type;
        }

        private static IEnumerable<string> GetColumnNames(string tableName, IDbConnection connection)
        {
            return connection.Query<string>(
                    $@"SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = '{tableName}'
                ORDER BY ORDINAL_POSITION")
                .ToList();
        }

        private static void SortColumns(DataTable table, IEnumerable<string> columnNames)
        {
            var columns = columnNames.ToList();
            foreach (var columnName in columns)
            {
                var ordinal = columns.IndexOf(columnName);
                table.Columns[columnName].SetOrdinal(ordinal);
            }
        }

        private static void RemoveNotExistingColumns(DataTable table, IEnumerable<string> columnNames)
        {
            for (var i = table.Columns.Count - 1; i >= 0; i--)
            {
                var dataColumn = table.Columns[i];
                var removeColumn = !columnNames.Contains(dataColumn.ColumnName);
                if (removeColumn)
                {
                    table.Columns.Remove(dataColumn.ColumnName);
                }
            }
        }

        public static void GenerateColumnMappings(this SqlBulkCopy that, IDataReader dataReader)
        {
            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                var columnName = dataReader.GetName(i);
                that.ColumnMappings.Add(columnName, columnName);
            }
        }

        public static void GenerateColumnMappings<T>(this SqlBulkCopy that)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in properties)
            {
                that.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
            }
        }
    }
}