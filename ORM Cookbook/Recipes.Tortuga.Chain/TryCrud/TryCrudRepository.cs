﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipes.TryCrud;
using Tortuga.Chain;

namespace Recipes.Chain.TryCrud
{
    public class TryCrudRepository : ITryCrudRepository<EmployeeClassification>
    {
        //const string TableName = "HR.EmployeeClassification";
        readonly SqlServerDataSource m_DataSource;

        public TryCrudRepository(SqlServerDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        public int Create(EmployeeClassification classification)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public void DeleteByKeyOrException(int employeeClassificationKey)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public bool DeleteByKeyWithStatus(int employeeClassificationKey)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public void DeleteOrException(EmployeeClassification classification)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public bool DeleteWithStatus(EmployeeClassification classification)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public EmployeeClassification FindByNameOrException(string employeeClassificationName)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public EmployeeClassification? FindByNameOrNull(string employeeClassificationName)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public EmployeeClassification GetByKeyOrException(int employeeClassificationKey)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public EmployeeClassification? GetByKeyOrNull(int employeeClassificationKey)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public void UpdateOrException(EmployeeClassification classification)
        {
            throw new AssertInconclusiveException("TODO");
        }

        public bool UpdateWithStatus(EmployeeClassification classification)
        {
            throw new AssertInconclusiveException("TODO");
        }
    }
}
