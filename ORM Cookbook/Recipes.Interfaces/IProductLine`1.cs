﻿using System.Collections.Generic;

namespace Recipes
{
    public interface IProductLine<TProduct>
        where TProduct : IProduct
    {
        public int ProductLineKey { get; set; }

        string? ProductLineName { get; set; }

        ICollection<TProduct> Products { get; }
    }
}
