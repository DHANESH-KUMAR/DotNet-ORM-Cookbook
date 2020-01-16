﻿using Dapper;
using Microsoft.Data.SqlClient;
using Recipes.Dapper.Models;
using Recipes.ModelWithChildren;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recipes.Dapper.ModelWithChildren
{
    public class ModelWithChildrenScenario : IModelWithChildrenScenario<ProductLine, Product>
    {
        readonly string m_ConnectionString;

        public ModelWithChildrenScenario(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        public int Create(ProductLine productLine)
        {
            if (productLine == null)
                throw new ArgumentNullException(nameof(productLine), $"{nameof(productLine)} is null.");

            const string sql = "INSERT INTO Production.ProductLine ( ProductLineName ) OUTPUT Inserted.ProductLineKey VALUES (@ProductLineName);";

            using (var con = OpenConnection())
            {
                productLine.ProductLineKey = (int)con.ExecuteScalar(sql, productLine);
                productLine.ApplyKeys();

                foreach (var item in productLine.Products)
                    InsertProduct(con, item);
            }
            return productLine.ProductLineKey;
        }

        public void Delete(ProductLine productLine)
        {
            if (productLine == null)
                throw new ArgumentNullException(nameof(productLine), $"{nameof(productLine)} is null.");

            const string sql = @"DELETE FROM Production.Product WHERE ProductLineKey = @ProductLineKey;
DELETE FROM Production.ProductLine WHERE ProductLineKey = @ProductLineKey";

            using (var con = OpenConnection())
                con.Execute(sql, productLine);
        }

        public void DeleteByKey(int productLineKey)
        {
            const string sql = @"DELETE FROM Production.Product WHERE ProductLineKey = @ProductLineKey;
DELETE FROM Production.ProductLine WHERE ProductLineKey = @ProductLineKey";

            using (var con = OpenConnection())
                con.Execute(sql, new { productLineKey });
        }

        public IList<ProductLine> FindByName(string productLineName, bool includeProducts)
        {
            const string sqlA = @"SELECT pl.ProductLineKey, pl.ProductLineName FROM Production.ProductLine pl WHERE pl.ProductLineName = @ProductLineName;
SELECT p.ProductKey, p.ProductName, p.ProductLineKey, p.ShippingWeight, p.ProductWeight FROM Production.Product p INNER JOIN Production.ProductLine pl ON p.ProductLineKey = pl.ProductLineKey WHERE pl.ProductLineName = @ProductLineName;";

            const string sqlB = @"SELECT pl.ProductLineKey, pl.ProductLineName FROM Production.ProductLine pl WHERE pl.ProductLineName = @ProductLineName;";

            using (var con = OpenConnection())
            {
                var sql = includeProducts ? sqlA : sqlB;
                var results = con.QueryMultiple(sql, new { productLineName });
                var productLines = results.Read<ProductLine>().ToList();

                if (includeProducts)
                {
                    var lookup = productLines.ToDictionary(x => x.ProductLineKey);
                    foreach (var product in results.Read<Product>())
                        lookup[product.ProductLineKey].Products.Add(product);
                }

                return productLines;
            }
        }

        public IList<ProductLine> GetAll(bool includeProducts)
        {
            const string sqlA = @"SELECT pl.ProductLineKey, pl.ProductLineName FROM Production.ProductLine pl;
SELECT p.ProductKey, p.ProductName, p.ProductLineKey, p.ShippingWeight, p.ProductWeight FROM Production.Product p INNER JOIN Production.ProductLine pl ON p.ProductLineKey = pl.ProductLineKey;";

            const string sqlB = @"SELECT pl.ProductLineKey, pl.ProductLineName FROM Production.ProductLine pl;";

            using (var con = OpenConnection())
            {
                var sql = includeProducts ? sqlA : sqlB;
                var results = con.QueryMultiple(sql);
                var productLines = results.Read<ProductLine>().ToList();

                if (includeProducts)
                {
                    var lookup = productLines.ToDictionary(x => x.ProductLineKey);
                    foreach (var product in results.Read<Product>())
                        lookup[product.ProductLineKey].Products.Add(product);
                }

                return productLines;
            }
        }

        public ProductLine? GetByKey(int productLineKey, bool includeProducts)
        {
            const string sqlA = @"SELECT pl.ProductLineKey, pl.ProductLineName FROM Production.ProductLine pl WHERE pl.ProductLineKey = @ProductLineKey;
SELECT p.ProductKey, p.ProductName, p.ProductLineKey, p.ShippingWeight, p.ProductWeight FROM Production.Product p WHERE p.ProductLineKey = @ProductLineKey;";

            const string sqlB = @"SELECT pl.ProductLineKey, pl.ProductLineName FROM Production.ProductLine pl WHERE pl.ProductLineKey = @ProductLineKey;";

            using (var con = OpenConnection())
            {
                var sql = includeProducts ? sqlA : sqlB;
                var results = con.QueryMultiple(sql, new { productLineKey });
                var productLine = results.ReadSingleOrDefault<ProductLine>();

                if (productLine == null)
                    return null;

                if (includeProducts)
                    foreach (var product in results.Read<Product>())
                        productLine.Products.Add(product);

                return productLine;
            }
        }

        public void Update(ProductLine productLine)
        {
            if (productLine == null)
                throw new ArgumentNullException(nameof(productLine), $"{nameof(productLine)} is null.");

            using (var con = OpenConnection())
                UpdateProductLine(con, productLine);
        }

        public void Update(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), $"{nameof(product)} is null.");

            using (var con = OpenConnection())
                UpdateProduct(con, product);
        }

        public void UpdateGraph(ProductLine productLine)
        {
            if (productLine == null)
                throw new ArgumentNullException(nameof(productLine), $"{nameof(productLine)} is null.");

            productLine.ApplyKeys();

            using (var con = OpenConnection())
            {
                UpdateProductLine(con, productLine);
                foreach (var item in productLine.Products)
                {
                    if (item.ProductKey == 0)
                        InsertProduct(con, item);
                    else
                        UpdateProduct(con, item);
                }
            }
        }

        public void UpdateGraphWithChildDeletes(ProductLine productLine)
        {
            if (productLine == null)
                throw new ArgumentNullException(nameof(productLine), $"{nameof(productLine)} is null.");

            productLine.ApplyKeys();

            using (var con = OpenConnection())
            {
                //Find products to remove
                var originalProductKeys = GetProductKeys(con, productLine.ProductLineKey);
                foreach (var item in productLine.Products)
                    originalProductKeys.Remove(item.ProductKey);

                UpdateProductLine(con, productLine);
                foreach (var item in productLine.Products)
                {
                    if (item.ProductKey == 0)
                        InsertProduct(con, item);
                    else
                        UpdateProduct(con, item);
                }

                //Remove products
                foreach (var key in originalProductKeys)
                    DeleteProduct(con, key);
            }
        }

        public void UpdateGraphWithDeletes(ProductLine productLine, IList<int> productKeysToRemove)
        {
            if (productLine == null)
                throw new ArgumentNullException(nameof(productLine), $"{nameof(productLine)} is null.");

            productLine.ApplyKeys();

            using (var con = OpenConnection())
            {
                UpdateProductLine(con, productLine);

                foreach (var item in productLine.Products)
                {
                    if (item.ProductKey == 0)
                        InsertProduct(con, item);
                    else
                        UpdateProduct(con, item);
                }

                if (productKeysToRemove != null)
                    foreach (var key in productKeysToRemove)
                        DeleteProduct(con, key);
            }
        }

        static void DeleteProduct(SqlConnection con, int productKey)
        {
            const string sql = "DELETE Production.Product WHERE ProductKey = @ProductKey;";

            con.Execute(sql, new { productKey });
        }

        static HashSet<int> GetProductKeys(SqlConnection con, int productLineKey)
        {
            const string sql = "SELECT p.ProductKey FROM Production.Product p WHERE p.ProductLineKey = @ProductLineKey";

            var results = new HashSet<int>();

            using (var reader = con.ExecuteReader(sql, new { productLineKey }))
                while (reader.Read())
                    results.Add(reader.GetInt32(0));

            return results;
        }

        static void InsertProduct(SqlConnection con, Product product)
        {
            const string sql = "INSERT INTO Production.Product ( ProductName, ProductLineKey, ShippingWeight, ProductWeight ) OUTPUT Inserted.ProductKey VALUES ( @ProductName, @ProductLineKey, @ShippingWeight, @ProductWeight )";

            product.ProductKey = con.ExecuteScalar<int>(sql, product);
        }

        static void UpdateProduct(SqlConnection con, Product product)
        {
            const string sql = "UPDATE Production.Product SET ProductName = @ProductName, ProductLineKey = @ProductLineKey, ShippingWeight = @ShippingWeight, ProductWeight = @ProductWeight WHERE ProductKey = @ProductKey;";

            con.Execute(sql, product);
        }

        static void UpdateProductLine(SqlConnection con, ProductLine productLine)
        {
            const string sql = "UPDATE Production.ProductLine SET ProductLineName = @ProductLineName WHERE ProductLineKey = @ProductLineKey;";

            con.Execute(sql, productLine);
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
