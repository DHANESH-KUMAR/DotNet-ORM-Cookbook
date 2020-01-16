﻿using Dapper;
using Microsoft.Data.SqlClient;
using Recipes.PopulateDataTable;
using System.Data;

namespace Recipes.Dapper.PopulateDataTable
{
    public class PopulateDataTableScenario : IPopulateDataTableScenario
    {
        readonly string m_ConnectionString;

        public PopulateDataTableScenario(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        public DataTable FindByFlags(bool isExempt, bool isEmployee)
        {
            var sql = @"SELECT ec.EmployeeClassificationKey, ec.EmployeeClassificationName, ec.IsExempt, ec.IsEmployee FROM HR.EmployeeClassification ec WHERE ec.IsExempt = @IsExempt AND ec.IsEmployee = @IsEmployee;";

            var result = new DataTable();

            using (var con = OpenConnection())
            using (var reader = con.ExecuteReader(sql, new { isExempt, isEmployee }))
                result.Load(reader);

            return result;
        }

        public DataTable GetAll()
        {
            var sql = @"SELECT ec.EmployeeClassificationKey, ec.EmployeeClassificationName, ec.IsExempt, ec.IsEmployee FROM HR.EmployeeClassification ec;";

            var result = new DataTable();

            using (var con = OpenConnection())
            using (var reader = con.ExecuteReader(sql))
                result.Load(reader);

            return result;
        }

        /// <summary>
        /// Opens a database connection.
        /// </summary>
        /// <remarks>Caller must dispose the connection.</remarks>
        SqlConnection OpenConnection()
        {
            var con = new SqlConnection(m_ConnectionString);
            con.Open();
            return con;
        }
    }
}
