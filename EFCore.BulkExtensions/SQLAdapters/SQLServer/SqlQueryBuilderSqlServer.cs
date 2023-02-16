using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCore.BulkExtensions.SQLAdapters.SQLServer
{
    public class SqlQueryBuilderSqlServer: QueryBuilderExtensions
    {
        /// <inheritdoc/>
        public override object CreateParameter(SqlParameter sqlParameter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object Dbtype()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string RestructureForBatch(string sql, bool isDelete = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string SelectFromOutputTable(TableInfo tableInfo)
        {
            return EFCore.BulkExtensions.SqlQueryBuilder.SelectFromOutputTable(tableInfo);
        }

        /// <inheritdoc/>
        public override void SetDbTypeParam(object npgsqlParameter, object dbType)
        {
            throw new NotImplementedException();
        }
    }
}
