﻿using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipes.RowCount;
using Recipes.ServiceStack.Entities;

namespace Recipes.ServiceStack.RowCount
{
    [TestClass]
    public class RowCountTests : RowCountTests<Employee>
    {
        protected override IRowCountScenario<Employee> GetScenario()
        {
            return new RowCountScenario(Setup.DbConnectionFactory);
        }
    }
}
