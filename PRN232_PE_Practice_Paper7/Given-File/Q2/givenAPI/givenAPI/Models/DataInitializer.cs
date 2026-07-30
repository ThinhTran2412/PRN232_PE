using System;
using System.Collections.Generic;

namespace givenAPI.Models
{
    public static class DataInitializer
    {
        public static List<Supplier> Suppliers = new List<Supplier> {
            new Supplier { SupplierId = 1, SupplierName = "Cong ty TNHH Alpha", Specialty = "Thiet bi dien tu", ContractDate = new DateTime(2015,5,10) },
            new Supplier { SupplierId = 2, SupplierName = "Cong ty CP Beta", Specialty = "Do gia dung", ContractDate = new DateTime(2018,2,15) },
            new Supplier { SupplierId = 3, SupplierName = "Tap doan Gamma", Specialty = "Thoi trang", ContractDate = new DateTime(2010,11,20) },
            new Supplier { SupplierId = 4, SupplierName = "Cong ty TNHH Delta", Specialty = "Thuc pham sach", ContractDate = new DateTime(2020,1,5) },
            new Supplier { SupplierId = 5, SupplierName = "Cong ty CP Epsilon", Specialty = "Van phong pham", ContractDate = new DateTime(2021,9,12) }
        };

        public static List<Product> Products = new List<Product> {
            new Product { ProductId = 1, ProductName = "Tai nghe khong day", Price = 45, Category = "Dien tu" },
            new Product { ProductId = 2, ProductName = "Noi chien khong dau", Price = 80, Category = "Gia dung" },
            new Product { ProductId = 3, ProductName = "Ao khoac gio", Price = 30, Category = "Thoi trang" },
            new Product { ProductId = 4, ProductName = "Gao huu co 5kg", Price = 12, Category = "Thuc pham" },
            new Product { ProductId = 5, ProductName = "But bi cao cap", Price = 5, Category = "Van phong" }
        };

        public static List<ProductSupplier> ProductSuppliers = new List<ProductSupplier> {
            new ProductSupplier { ProductId = 1, SupplierId = 1 }, new ProductSupplier { ProductId = 1, SupplierId = 2 },
            new ProductSupplier { ProductId = 2, SupplierId = 2 }, new ProductSupplier { ProductId = 2, SupplierId = 4 },
            new ProductSupplier { ProductId = 3, SupplierId = 3 }, new ProductSupplier { ProductId = 4, SupplierId = 3 },
            new ProductSupplier { ProductId = 4, SupplierId = 1 }, new ProductSupplier { ProductId = 5, SupplierId = 4 },
            new ProductSupplier { ProductId = 5, SupplierId = 5 }, new ProductSupplier { ProductId = 3, SupplierId = 5 }
        };
    }
}
