using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCore.BulkExtensions.SQLAdapters
{
    /// <summary>
    /// Contains a list of methods to generate Adpaters and helpers instances
    /// </summary>
    public abstract class QueryBuilderExtensions
    {
        /// <summary>
        /// Generates SQL query to select output from a table
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public abstract string SelectFromOutputTable(TableInfo tableInfo);

        /// <summary>
        /// Restructures a sql query for batch commands
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isDelete"></param>
        public abstract string RestructureForBatch(string sql, bool isDelete = false);

        /// <summary>
        /// Returns a DbParameters intanced per provider
        /// </summary>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public abstract object CreateParameter(SqlParameter sqlParameter);

        /// <summary>
        /// Returns NpgsqlDbType for PostgreSql parameters. Throws <see cref="NotImplementedException"/> for anothers providers
        /// </summary>
        /// <returns></returns>
        public abstract object Dbtype();

        /// <summary>
        /// Returns void. Throws <see cref="NotImplementedException"/> for anothers providers
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public abstract void SetDbTypeParam(object npgsqlParameter, object dbType);
    }
}
