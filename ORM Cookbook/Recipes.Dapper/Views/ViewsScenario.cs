﻿using Dapper;
using Microsoft.Data.SqlClient;
using Recipes.Dapper.Models;
using Recipes.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recipes.Dapper.Views
{
    public class ViewsScenario : IViewsScenario<EmployeeDetail, EmployeeSimple>
    {
        readonly string m_ConnectionString;

        public ViewsScenario(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        public int Create(EmployeeSimple employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), $"{nameof(employee)} is null.");

            const string sql = @"INSERT INTO HR.Employee
(FirstName, MiddleName, LastName, Title, OfficePhone, CellPhone, EmployeeClassificationKey)
OUTPUT Inserted.EmployeeKey
VALUES
(@FirstName, @MiddleName, @LastName, @Title, @OfficePhone, @CellPhone, @EmployeeClassificationKey);";

            using (var con = OpenConnection())
                return con.ExecuteScalar<int>(sql, employee);
        }

        public IList<EmployeeDetail> FindByEmployeeClassificationKey(int employeeClassificationKey)
        {
            const string sql = "SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed WHERE ed.EmployeeClassificationKey = @EmployeeClassificationKey";

            using (var con = OpenConnection())
                return con.Query<EmployeeDetail>(sql, new { employeeClassificationKey }).ToList();
        }

        public IList<EmployeeDetail> FindByLastName(string lastName)
        {
            const string sql = "SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed WHERE ed.LastName = @LastName";

            using (var con = OpenConnection())
                return con.Query<EmployeeDetail>(sql, new { lastName }).ToList();
        }

        public IList<EmployeeDetail> GetAll()
        {
            const string sql = "SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed";

            using (var con = OpenConnection())
                return con.Query<EmployeeDetail>(sql).ToList();
        }

        public EmployeeDetail? GetByEmployeeKey(int employeeKey)
        {
            const string sql = "SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed WHERE ed.EmployeeKey = @EmployeeKey";

            using (var con = OpenConnection())
                return con.QuerySingleOrDefault<EmployeeDetail>(sql, new { employeeKey });
        }

        public IEmployeeClassification? GetClassification(int employeeClassificationKey)
        {
            const string sql = "SELECT ec.EmployeeClassificationKey, ec.EmployeeClassificationName, ec.IsExempt, ec.IsEmployee FROM HR.EmployeeClassification ec WHERE EmployeeClassificationKey = @EmployeeClassificationKey";

            using (var con = OpenConnection())
                return con.QuerySingleOrDefault<EmployeeClassification>(sql, new { employeeClassificationKey });
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
