﻿using Recipes.SingleModelCrud;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes.Dapper.SingleModelCrud
{
    public class EmployeeClassification : IEmployeeClassification
    {
        public int EmployeeClassificationKey { get; set; }

        /// <exclude />
        public string? EmployeeClassificationName { get; set; }
    }
}
