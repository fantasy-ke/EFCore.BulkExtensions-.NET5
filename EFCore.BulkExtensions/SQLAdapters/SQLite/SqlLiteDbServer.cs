using EFCore.BulkExtensions;
using EFCore.BulkExtensions.SqlAdapters.SQLite;
using EFCore.BulkExtensions.SQLAdapters;
using EFCore.BulkExtensions.SQLAdapters.SQLite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data.Common;

namespace EFCore.BulkExtensions.SqlAdapters.SQLite;

/// <inheritdoc/>
public class SqlLiteDbServer : IDbServer
{
    DbServerType IDbServer.Type => DbServerType.SQLite;

    SqLiteOperationsAdapter _adapter = new ();
    ISqlOperationsAdapter IDbServer.Adapter => _adapter;

    SqLiteDialect _dialect = new();
    IQueryBuilderSpecialization IDbServer.Dialect => _dialect;

    /// <inheritdoc/>
    public DbConnection? DbConnection { get; set; }

    /// <inheritdoc/>
    public DbTransaction? DbTransaction { get; set; }

    QueryBuilderExtensions _queryBuilder = new SqlQueryBuilderSqlite();
    /// <inheritdoc/>
    public QueryBuilderExtensions QueryBuilder => _queryBuilder;

    string IDbServer.ValueGenerationStrategy => String.Empty;

    bool IDbServer.PropertyHasIdentity(IAnnotation annotation) => false;
}
