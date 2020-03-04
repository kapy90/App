using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

namespace App.EntityFramework
{
    /// <summary>
    /// Represents DB context
    /// </summary>
    public partial interface IDbContext
    {
        #region Methods

        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A set for the given entity type</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        /// <returns>The number of state entries written to the database</returns>
        int SaveChanges();

        /// <summary>
        /// Generate a script to create all tables for the current model
        /// </summary>
        /// <returns>A SQL script</returns>
        string GenerateCreateScript();

        /// <summary>
        /// Executes the given SQL against the database
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <param name="doNotEnsureTransaction">true - the transaction creation is not ensured; false - the transaction creation is ensured.</param>
        /// <param name="timeout">The timeout to use for command. Note that the command timeout is distinct from the connection timeout, which is commonly set on the database connection string</param>
        /// <param name="parameters">Parameters to use with the SQL</param>
        /// <returns>The number of rows affected</returns>
        int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// Detach an entity from the context
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        void Detach<TEntity>(TEntity entity) where TEntity : class;

        #endregion
        DatabaseFacade Database { get; }
    }
}
