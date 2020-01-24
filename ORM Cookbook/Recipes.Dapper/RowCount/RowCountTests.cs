﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipes.Dapper.Models;
using Recipes.RowCount;

namespace Recipes.Dapper.RowCount
{
    [TestClass]
    public class RowCountTests : RowCountTests<EmployeeSimple>
    {
        protected override IRowCountScenario<EmployeeSimple> GetScenario()
        {
            return new RowCountScenario(Setup.ConnectionString);
        }
    }
}
