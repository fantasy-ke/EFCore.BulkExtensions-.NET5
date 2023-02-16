using EFCore.BulkExtensions.SqlAdapters;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace EFCore.BulkExtensions.SQLAdapters.SQLServer
{
    public class SqlServerDbServer : IDbServer
    {
        DbServerType IDbServer.Type => DbServerType.SQLServer;

        SqlOperationsServerAdapter _adapter = new();
        ISqlOperationsAdapter IDbServer.Adapter => _adapter;

        SqlServerDialect _dialect = new();
        IQueryBuilderSpecialization IDbServer.Dialect => _dialect;

        /// <inheritdoc/>
        public DbConnection? DbConnection { get; set; }

        /// <inheritdoc/>
        public DbTransaction? DbTransaction { get; set; }

        QueryBuilderExtensions _queryBuilder = new SqlQueryBuilderSqlServer();
        /// <inheritdoc/>
        public QueryBuilderExtensions QueryBuilder => _queryBuilder;

        string IDbServer.ValueGenerationStrategy => nameof(SqlServerValueGenerationStrategy);

        bool IDbServer.PropertyHasIdentity(IAnnotation annotation) => (SqlServerValueGenerationStrategy?)annotation.Value == SqlServerValueGenerationStrategy.IdentityColumn;
    }
}
