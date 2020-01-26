﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipes.Chain.Models;
using Recipes.Transactions;

namespace Recipes.Chain.Transactions
{
    [TestClass]
    public class TransactionsTests : TransactionsTests<EmployeeClassification>
    {
        protected override ITransactionsScenario<EmployeeClassification> GetScenario()
        {
            return new TransactionsScenario(Setup.PrimaryDataSource);
        }
    }
}
