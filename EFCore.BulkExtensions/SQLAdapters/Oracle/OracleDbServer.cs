using EFCore.BulkExtensions.SqlAdapters;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Oracle.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.BulkExtensions.SQLAdapters.Oracle
{
    public class OracleDbServer : IDbServer
    {
        DbServerType IDbServer.Type => DbServerType.Oracle;

        private OracleAdapter _adapter = new();
        ISqlOperationsAdapter IDbServer.Adapter => _adapter;

        private OracleDialect _dialect = new();
        IQueryBuilderSpecialization IDbServer.Dialect => _dialect;

        private QueryBuilderExtensions _queryBuilder = new SqlQueryBuilderOracle();

        /// <inheritdoc/>
        public QueryBuilderExtensions QueryBuilder => _queryBuilder;

        string IDbServer.ValueGenerationStrategy => nameof(OracleValueGenerationStrategy);

        /// <inheritdoc/>
        public DbConnection? DbConnection { get; set; }

        /// <inheritdoc/>
        public DbTransaction? DbTransaction { get; set; }

        bool IDbServer.PropertyHasIdentity(IAnnotation annotation) => (OracleValueGenerationStrategy?)annotation.Value == OracleValueGenerationStrategy.IdentityColumn;
    }
}

/// <inheritdoc/>

