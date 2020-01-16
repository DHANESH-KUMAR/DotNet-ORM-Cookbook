﻿using Dapper;
using Microsoft.Data.SqlClient;
using Recipes.Dapper.Models;
using Recipes.ModelWithLookup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recipes.Dapper.ModelWithLookup
{
    public class ModelWithLookupComplexScenario : IModelWithLookupComplexScenario<EmployeeComplex>
    {
        readonly string m_ConnectionString;

        public ModelWithLookupComplexScenario(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        public int Create(EmployeeComplex employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), $"{nameof(employee)} is null.");
            if (employee.EmployeeClassification == null)
                throw new ArgumentNullException(nameof(employee), $"{nameof(employee.EmployeeClassification)} is null.");

            const string sql = @"INSERT INTO HR.Employee
(FirstName, MiddleName, LastName, Title, OfficePhone, CellPhone, EmployeeClassificationKey)
OUTPUT Inserted.EmployeeKey
VALUES
(@FirstName, @MiddleName, @LastName, @Title, @OfficePhone, @CellPhone, @EmployeeClassificationKey);";

            using (var con = OpenConnection())
                return (int)con.ExecuteScalar(sql, employee);
        }

        public void Delete(EmployeeComplex employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), $"{nameof(employee)} is null.");

            const string sql = @"DELETE HR.Employee WHERE EmployeeKey = @EmployeeKey;";

            using (var con = OpenConnection())
                con.Execute(sql, employee);
        }

        public void DeleteByKey(int employeeKey)
        {
            const string sql = @"DELETE HR.Employee WHERE EmployeeKey = @EmployeeKey;";

            using (var con = OpenConnection())
                con.Execute(sql, new { employeeKey });
        }

        public IList<EmployeeComplex> FindByLastName(string lastName)
        {
            const string sql = @"SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed WHERE ed.LastName = @LastName";

            var result = new List<EmployeeComplex>();

            using (var con = OpenConnection())
                return con.Query<EmployeeComplex, EmployeeClassification, EmployeeComplex>(sql,
                    (e, ec) => { e.EmployeeClassification = ec; return e; },
                    new { LastName = lastName },
                    splitOn: "EmployeeClassificationKey")
                    .ToList();
        }

        public IList<EmployeeComplex> GetAll()
        {
            const string sql = @"SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed";

            var result = new List<EmployeeComplex>();

            using (var con = OpenConnection())
                return con.Query<EmployeeComplex, EmployeeClassification, EmployeeComplex>(sql,
                    (e, ec) => { e.EmployeeClassification = ec; return e; },
                    splitOn: "EmployeeClassificationKey")
                    .ToList();
        }

        public EmployeeComplex? GetByKey(int employeeKey)
        {
            const string sql = @"SELECT ed.EmployeeKey, ed.FirstName, ed.MiddleName, ed.LastName, ed.Title, ed.OfficePhone, ed.CellPhone, ed.EmployeeClassificationKey, ed.EmployeeClassificationName, ed.IsExempt, ed.IsEmployee FROM HR.EmployeeDetail ed WHERE ed.EmployeeKey = @EmployeeKey";

            using (var con = OpenConnection())
                return con.Query<EmployeeComplex, EmployeeClassification, EmployeeComplex>(sql,
                    (e, ec) => { e.EmployeeClassification = ec; return e; },
                    new { EmployeeKey = employeeKey },
                    splitOn: "EmployeeClassificationKey")
                    .SingleOrDefault();
        }

        public IEmployeeClassification? GetClassification(int employeeClassificationKey)
        {
            const string sql = @"SELECT ec.EmployeeClassificationKey, ec.EmployeeClassificationName, ec.IsExempt, ec.IsEmployee
                        FROM HR.EmployeeClassification ec
                        WHERE ec.EmployeeClassificationKey = @EmployeeClassificationKey;";

            using (var con = OpenConnection())
                return con.QuerySingle<EmployeeClassification>(sql, new { employeeClassificationKey });
        }

        public void Update(EmployeeComplex employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), $"{nameof(employee)} is null.");
            if (employee.EmployeeClassification == null)
                throw new ArgumentNullException(nameof(employee), $"{nameof(employee.EmployeeClassification)} is null.");

            const string sql = @"UPDATE HR.Employee
SET FirstName = @FirstName,
    MiddleName = @MiddleName,
    LastName = @LastName,
    Title = @Title,
    OfficePhone = @OfficePhone,
    CellPhone = @CellPhone,
    EmployeeClassificationKey = @EmployeeClassificationKey
WHERE EmployeeKey = @EmployeeKey;";

            using (var con = OpenConnection())
                con.Execute(sql, employee);
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
