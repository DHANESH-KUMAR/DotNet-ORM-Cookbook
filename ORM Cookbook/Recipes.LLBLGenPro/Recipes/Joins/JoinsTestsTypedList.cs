﻿using LLBLGenPro.OrmCookbook.EntityClasses;
using LLBLGenPro.OrmCookbook.TypedListClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipes.Joins;

namespace Recipes.LLBLGenPro.Joins
{
    [TestClass]
    public class JoinsTestsTypedList : JoinsTests<EmployeeJoinedRow, EmployeeEntity>
    {
        protected override IJoinsScenario<EmployeeJoinedRow, EmployeeEntity> GetScenario()
        {
            return new JoinsScenarioTypedList();
        }
    }
}
