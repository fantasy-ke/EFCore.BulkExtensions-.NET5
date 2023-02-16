using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EFCore.BulkExtensions.SqlAdapters.SQLite;

/// <summary>
/// Contains a compilation of SQL queries used in EFCore.
/// </summary>
public class SqlQueryBuilderSqlite : SQLAdapters.QueryBuilderExtensions
{
    public static string SelectLastInsertRowId()
    {
        return "SELECT last_insert_rowid();";
    }

    // In Sqlite if table has AutoIncrement then InsertOrUpdate is not supported in one call,
    // we can not simultaneously Insert without PK(being 0,0,...) and Update with PK(1,2,...), separate calls Insert, Update are required.
    public static string InsertIntoTable(TableInfo tableInfo, OperationType operationType, string tableName = null)
    {
        tableName ??= tableInfo.InsertToTempTable ? tableInfo.TempTableName : tableInfo.TableName;

        var tempDict = tableInfo.PropertyColumnNamesDict;
        if (operationType == OperationType.Insert && tableInfo.PropertyColumnNamesDict.Any()) // Only OnInsert ommite colums with Default values
        {
            tableInfo.PropertyColumnNamesDict = tableInfo.PropertyColumnNamesDict.Where(a => !tableInfo.DefaultValueProperties.Contains(a.Key)).ToDictionary(a => a.Key, a => a.Value);
        }

        List<string> columnsList = tableInfo.PropertyColumnNamesDict.Values.ToList();
        List<string> propertiesList = tableInfo.PropertyColumnNamesDict.Keys.ToList();

        bool keepIdentity = tableInfo.BulkConfig.SqlBulkCopyOptions.HasFlag(SqlBulkCopyOptions.KeepIdentity);
        if (operationType == OperationType.Insert && !keepIdentity && tableInfo.HasIdentity)
        {
            var identityPropertyName = tableInfo.PropertyColumnNamesDict.SingleOrDefault(a => a.Value == tableInfo.IdentityColumnName).Key;
            columnsList = columnsList.Where(a => a != tableInfo.IdentityColumnName).ToList();
            propertiesList = propertiesList.Where(a => a != identityPropertyName).ToList();
        }

        var commaSeparatedColumns = SqlQueryBuilder.GetCommaSeparatedColumns(columnsList);
        var commaSeparatedColumnsParams = SqlQueryBuilder.GetCommaSeparatedColumns(propertiesList, "@").Replace("[", "").Replace("]", "").Replace(".", "_");

        var q = $"INSERT INTO [{tableName}] " +
                $"({commaSeparatedColumns}) " +
                $"VALUES ({commaSeparatedColumnsParams})";

        if (operationType == OperationType.InsertOrUpdate)
        {
            List<string> primaryKeys = tableInfo.PrimaryKeysPropertyColumnNameDict.Select(k => tableInfo.PropertyColumnNamesDict[k.Key]).ToList();
            var commaSeparatedPrimaryKeys = SqlQueryBuilder.GetCommaSeparatedColumns(primaryKeys);
            var commaSeparatedColumnsEquals = SqlQueryBuilder.GetCommaSeparatedColumns(columnsList, equalsTable: "", propertColumnsNamesDict: tableInfo.PropertyColumnNamesDict).Replace("]", "").Replace(" = .[", "] = @").Replace(".", "_");
            var commaANDSeparatedPrimaryKeys = SqlQueryBuilder.GetANDSeparatedColumns(primaryKeys, equalsTable: "@", propertColumnsNamesDict: tableInfo.PropertyColumnNamesDict).Replace("]", "").Replace(" = @[", "] = @").Replace(".", "_");

            q += $" ON CONFLICT({commaSeparatedPrimaryKeys}) DO UPDATE" +
                 $" SET {commaSeparatedColumnsEquals}" +
                 $" WHERE {commaANDSeparatedPrimaryKeys}";
        }

        tableInfo.PropertyColumnNamesDict = tempDict;

        return q + ";";
    }

    public static string UpdateSetTable(TableInfo tableInfo, string tableName = null)
    {
        tableName ??= tableInfo.TableName;
        List<string> columnsList = tableInfo.PropertyColumnNamesDict.Values.ToList();
        List<string> primaryKeys = tableInfo.PrimaryKeysPropertyColumnNameDict.Select(k => tableInfo.PropertyColumnNamesDict[k.Key]).ToList();
        var commaSeparatedColumns = SqlQueryBuilder.GetCommaSeparatedColumns(columnsList, equalsTable: "@", propertColumnsNamesDict: tableInfo.PropertyColumnNamesDict).Replace("]", "").Replace(" = @[", "] = @").Replace(".", "_"); ;
        var commaSeparatedPrimaryKeys = SqlQueryBuilder.GetANDSeparatedColumns(primaryKeys, equalsTable: "@", propertColumnsNamesDict: tableInfo.PropertyColumnNamesDict).Replace("]", "").Replace(" = @[", "] = @").Replace(".", "_"); ;

        var q = $"UPDATE [{tableName}] " +
                $"SET {commaSeparatedColumns} " +
                $"WHERE {commaSeparatedPrimaryKeys};";
        return q;
    }

    public static string DeleteFromTable(TableInfo tableInfo, string tableName = null)
    {
        tableName ??= tableInfo.TableName;
        List<string> primaryKeys = tableInfo.PrimaryKeysPropertyColumnNameDict.Select(k => tableInfo.PropertyColumnNamesDict[k.Key]).ToList();
        var commaSeparatedPrimaryKeys = SqlQueryBuilder.GetANDSeparatedColumns(primaryKeys, equalsTable: "@", propertColumnsNamesDict: tableInfo.PropertyColumnNamesDict).Replace("]", "").Replace(" = @[", "] = @").Replace(".", "_");

        var q = $"DELETE FROM [{tableName}] " +
                $"WHERE {commaSeparatedPrimaryKeys};";
        return q;
    }

    public static string CreateTableCopy(string existingTableName, string newTableName) // Used for BulkRead
    {
        var q = $"CREATE TABLE {newTableName} AS SELECT * FROM {existingTableName} WHERE 0;";
        return q;
    }

    public static string DropTable(string tableName)
    {
        string q = $"DROP TABLE IF EXISTS {tableName}";
        return q;
    }

    public override string SelectFromOutputTable(TableInfo tableInfo)
    {
        throw new NotImplementedException();
    }

    public override string RestructureForBatch(string sql, bool isDelete = false)
    {
        throw new NotImplementedException();
    }

    public override object CreateParameter(SqlParameter sqlParameter)
    {
        throw new NotImplementedException();
    }

    public override object Dbtype()
    {
        throw new NotImplementedException();
    }

    public override void SetDbTypeParam(object npgsqlParameter, object dbType)
    {
        throw new NotImplementedException();
    }
}
