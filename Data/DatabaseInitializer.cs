using System;
using System.Data.SQLite;
using System.IO;

namespace BusinessManagementSystem.Data
{
    public static class DatabaseInitializer
    {
        private static readonly string dbFile = "ERPSystem.db";
        private static readonly string connectionString =
            $"Data Source={dbFile};Version=3;";

        public static void Initialize()
        {
            CreateDatabase();
            CreateTables();
        }

        private static void CreateDatabase()
        {
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
                Console.WriteLine("Database created successfully.");
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }
        }

        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Enable Foreign Keys
                using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    pragma.ExecuteNonQuery();
                }

                string sql = @"

                CREATE TABLE IF NOT EXISTS Clients (
                    ClientId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientName TEXT NOT NULL,
                    Address TEXT,
                    Phone TEXT,
                    Email TEXT,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS Suppliers (
                    SupplierId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SupplierName TEXT NOT NULL,
                    Address TEXT,
                    Phone TEXT,
                    Email TEXT,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS Products (
                    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductName TEXT NOT NULL,
                    Description TEXT,
                    UnitPrice REAL,
                    QuantityInStock INTEGER DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS ClientInquiries (
                    InquiryId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    ProductId INTEGER,
                    InquiryDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Status TEXT,
                    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId),
                    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
                );

                CREATE TABLE IF NOT EXISTS SupplierInquiries (
                    SupplierInquiryId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SupplierId INTEGER,
                    ProductId INTEGER,
                    AvailableQuantity INTEGER,
                    ExpectedDeliveryDate DATETIME,
                    InquiryDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
                    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
                );

                CREATE TABLE IF NOT EXISTS Quotations (
                    QuotationId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    QuotationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Status TEXT,
                    TotalAmount REAL,
                    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
                );

                CREATE TABLE IF NOT EXISTS QuotationItems (
                    QuotationItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                    QuotationId INTEGER,
                    ProductId INTEGER,
                    Quantity INTEGER,
                    UnitPrice REAL,
                    FOREIGN KEY (QuotationId) REFERENCES Quotations(QuotationId),
                    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
                );

                CREATE TABLE IF NOT EXISTS CustomerOrders (
                    OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Status TEXT,
                    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
                );

                CREATE TABLE IF NOT EXISTS PurchaseOrders (
                    PurchaseOrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                    SupplierId INTEGER,
                    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Status TEXT,
                    ExpectedDeliveryDate DATETIME,
                    FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId)
                );

                CREATE TABLE IF NOT EXISTS PurchaseOrderItems (
                    PurchaseOrderItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PurchaseOrderId INTEGER,
                    ProductId INTEGER,
                    Quantity INTEGER,
                    UnitCost REAL,
                    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId),
                    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
                );

                CREATE TABLE IF NOT EXISTS MaterialReceipts (
                    ReceiptId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PurchaseOrderId INTEGER,
                    ReceivedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId)
                );

                CREATE TABLE IF NOT EXISTS DeliveryNotes (
                    DeliveryNoteId INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER,
                    DeliveryDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (OrderId) REFERENCES CustomerOrders(OrderId)
                );

                CREATE TABLE IF NOT EXISTS SalesInvoices (
                    InvoiceId INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER,
                    InvoiceDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    TotalAmount REAL,
                    FOREIGN KEY (OrderId) REFERENCES CustomerOrders(OrderId)
                );

                CREATE TABLE IF NOT EXISTS StatementsOfAccount (
                    StatementId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER,
                    StatementDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Balance REAL,
                    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
                );

                CREATE TABLE IF NOT EXISTS Receipts (
                    ReceiptId INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER,
                    ReceiptType TEXT,
                    AmountPaid REAL,
                    ReceiptDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (InvoiceId) REFERENCES SalesInvoices(InvoiceId)
                );

                CREATE TABLE IF NOT EXISTS Employees (
                    EmployeeId INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT,
                    LastName TEXT,
                    Position TEXT,
                    HireDate DATETIME,
                    Email TEXT,
                    Phone TEXT
                );

                CREATE TABLE IF NOT EXISTS Recruitment (
                    RecruitmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ApplicantName TEXT,
                    PositionApplied TEXT,
                    ApplicationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Status TEXT
                );

                ";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("All tables created successfully.");
            }
        }
    }
}
