-- 1. Tao Database
CREATE DATABASE PE_PRN_26SP_P7;
GO
USE PE_PRN_26SP_P7;
GO

-- 2. Bang Nha cung cap (Suppliers)
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY IDENTITY(1,1),
    SupplierName NVARCHAR(100) NOT NULL,
    Specialty NVARCHAR(200), -- Linh vuc chuyen mon
    ContractDate DATE
);

-- 3. Bang San pham (Products)
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(200) NOT NULL,
    Price INT, -- Don gia (USD)
    Category NVARCHAR(100)
);

-- 4. Bang Cung ung San pham (Product Suppliers)
-- Giai quyet quan he n-n giua Nha cung cap va San pham
CREATE TABLE ProductSuppliers (
    ProductID INT,
    SupplierID INT,
    SupplyDate DATE DEFAULT GETDATE(),
    PRIMARY KEY (ProductID, SupplierID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

-- 5. Bang Lo hang (Batches)
CREATE TABLE Batches (
    BatchID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT,
    WarehouseCode NVARCHAR(20),
    Quarter NVARCHAR(20),
    Quantity INT,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- 6. Bang Khach hang (Customers)
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    DateOfBirth DATE
);

-- 7. Bang Don hang (Orders)
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT,
    BatchID INT,
    OrderDate DATE,
    Rating FLOAT, -- Diem danh gia (NULL = chua danh gia)
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (BatchID) REFERENCES Batches(BatchID)
);
GO

-----------------------------------------------------------
--- CHEN DU LIEU MAU (Khong dau) ---
-----------------------------------------------------------

-- Suppliers (5 records)
INSERT INTO Suppliers (SupplierName, Specialty, ContractDate) VALUES
('Cong ty TNHH Alpha', 'Thiet bi dien tu', '2015-05-10'),
('Cong ty CP Beta', 'Do gia dung', '2018-02-15'),
('Tap doan Gamma', 'Thoi trang', '2010-11-20'),
('Cong ty TNHH Delta', 'Thuc pham sach', '2020-01-05'),
('Cong ty CP Epsilon', 'Van phong pham', '2021-09-12');

-- Products (5 records)
INSERT INTO Products (ProductName, Price, Category) VALUES
('Tai nghe khong day', 45, 'Dien tu'),
('Noi chien khong dau', 80, 'Gia dung'),
('Ao khoac gio', 30, 'Thoi trang'),
('Gao huu co 5kg', 12, 'Thuc pham'),
('But bi cao cap', 5, 'Van phong');

-- ProductSuppliers (10 records)
INSERT INTO ProductSuppliers (ProductID, SupplierID) VALUES
(1, 1), (1, 2), (2, 2), (2, 4), (3, 3),
(4, 3), (4, 1), (5, 4), (5, 5), (3, 5);

-- Batches (5 records)
INSERT INTO Batches (ProductID, WarehouseCode, Quarter, Quantity) VALUES
(1, 'WH-A01', 'Q1-2024', 500),
(2, 'WH-B02', 'Q1-2024', 400),
(3, 'WH-C03', 'Q2-2024', 600),
(4, 'WH-D01', 'Q2-2024', 300),
(5, 'WH-A04', 'Q1-2024', 450);

-- Customers (5 records)
INSERT INTO Customers (CustomerName, Email, DateOfBirth) VALUES
('Nguyen Hoang Long', 'longnh@shop.com.vn', '2004-05-12'),
('Vu Minh Anh', 'anhvm@shop.com.vn', '2004-08-20'),
('Le Bao Chau', 'chaulb@shop.com.vn', '2003-12-01'),
('Dang Tien Dat', 'datdt@shop.com.vn', '2004-01-15'),
('Tran Thu Ha', 'hatt@shop.com.vn', '2004-10-30');

-- Orders (10 records)
INSERT INTO Orders (CustomerID, BatchID, OrderDate, Rating) VALUES
(1, 1, '2024-01-02', 4.5),
(2, 1, '2024-01-02', 3.5),
(3, 2, '2024-01-05', 4.5),
(4, 3, '2024-02-10', NULL),
(5, 4, '2024-02-12', 3.0),
(1, 5, '2024-01-03', 4.0),
(2, 2, '2024-01-06', NULL),
(3, 4, '2024-02-15', NULL),
(4, 1, '2024-01-04', 3.5),
(5, 5, '2024-01-05', 4.5);
GO
