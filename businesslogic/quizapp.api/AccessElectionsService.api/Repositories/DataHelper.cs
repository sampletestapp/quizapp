using System.Data;

namespace AccessElectionsService.api.Repositories
{
    public static class DataHelper
    {
        public static SqlDbType GetSqlDbTypeForColumn(string columnName, DataTable schemaTable)
        {
            var row = schemaTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("COLUMN_NAME") == columnName);
            if (row != null)
            {
                var dataType = row.Field<string>("DATA_TYPE")?.ToLower();
                switch (dataType)
                {
                    case "int":
                        return SqlDbType.Int;
                    case "image":
                        return SqlDbType.VarBinary; // Adjust this if your data type is different
                    case "datetime":
                        return SqlDbType.DateTime;
                    case "varchar":
                        return SqlDbType.VarChar; // Adjust this if your data type is different
                    case "uniqueidentifier":
                        return SqlDbType.UniqueIdentifier;
                    case "decimal":
                        return SqlDbType.Decimal; // Add this case for decimal data type
                    case "xml":
                        return SqlDbType.Xml;// Add more cases for other data types as needed
                    case "bit":
                        return SqlDbType.Bit;
                    default:
                        throw new NotSupportedException($"Unsupported data type for column {columnName}: {dataType}");
                }
            }
            throw new ArgumentException($"Column {columnName} not found in schema table.");
        }

        public static int GetSizeForColumn(string columnName, DataTable schemaTable)
        {
            var row = schemaTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("COLUMN_NAME") == columnName);
            if (row != null && row.Table.Columns.Contains("CHARACTER_MAXIMUM_LENGTH"))
            {
                var maxSize = row.Field<int?>("CHARACTER_MAXIMUM_LENGTH");
                return maxSize ?? -1;
            }
            return -1; // Return -1 if size information is not available or not applicable
        }
    }
}
