﻿using System.Data.Common;

namespace Recipes.ConnectionSharing
{
    public interface IConnectionSharingScenario<TModel, TConnection, TTransaction>
       where TModel : class, IEmployeeClassification, new()
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        /// <summary>
        /// Closes a previous opened connection.
        /// </summary>
        /// <param name="state">ORM specific state such as a context/session</param>
        void CloseConnection(object state);

        /// <summary>
        /// Closes a previous opened connection and transaction.
        /// </summary>
        /// <param name="state">ORM specific state such as a context/session</param>
        void CloseConnectionAndTransaction(object state);

        /// <summary>
        /// Open and return a connection that can be used by another ORM.
        /// </summary>
        /// <returns>The open connection and any ORM-specific state such as a context/session.</returns>
        (TConnection Connection, object State) OpenConnection();

        /// <summary>
        /// Open and return a connection/transaction pair that can be used by another ORM.
        /// </summary>
        /// <returns>The open connection/transaction pair and any ORM-specific state such as a context/session.</returns>
        (TConnection Connection, TTransaction Transaction, object State) OpenConnectionAndTransaction();

        /// <summary>
        /// Gets an EmployeeClassification row by its primary key, reusing an open connection.
        /// </summary>
        /// <param name="connection">A open database connection</param>
        /// <param name="transaction">An open transaction. May be null.</param>
        TModel UseOpenConnection(TConnection connection, TTransaction? transaction, int employeeClassificationKey);

        /// <summary>
        /// Returns a connection string.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This should come directly from the config file, not the ORM.</remarks>
        string GetConnectionString();
    }
}