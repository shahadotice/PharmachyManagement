-- Pharmacy Inventory & Billing System Database Script
-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PharmacyDB')
BEGIN
    CREATE DATABASE PharmacyDB;
END
GO

USE PharmacyDB;
GO

-- Create Users Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [Password] NVARCHAR(100) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Create Medicines Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Medicines]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Medicines] (
        [MedicineId] INT IDENTITY(1,1) PRIMARY KEY,
        [MedicineName] NVARCHAR(200) NOT NULL,
        [BatchNo] NVARCHAR(50) NOT NULL,
        [ExpiryDate] DATETIME NOT NULL,
        [Quantity] INT NOT NULL DEFAULT 0,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [SellsPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Description] NVARCHAR(500) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME NULL
    );
END
GO

-- Add SellsPrice column if it doesn't exist (for existing databases)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Medicines]') AND name = 'SellsPrice')
BEGIN
    ALTER TABLE [dbo].[Medicines]
    ADD [SellsPrice] DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO

-- Create SalesMaster Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesMaster]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesMaster] (
        [SalesId] INT IDENTITY(1,1) PRIMARY KEY,
        [InvoiceNumber] NVARCHAR(50) NOT NULL UNIQUE,
        [InvoiceDate] DATETIME NOT NULL,
        [CustomerName] NVARCHAR(100) NOT NULL,
        [CustomerContact] NVARCHAR(50) NULL,
        [SubTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Discount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [GrandTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Create SalesDetail Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesDetail] (
        [SalesDetailId] INT IDENTITY(1,1) PRIMARY KEY,
        [SalesId] INT NOT NULL,
        [MedicineId] INT NOT NULL,
        [BatchNo] NVARCHAR(50) NOT NULL,
        [ExpiryDate] DATETIME NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [LineTotal] DECIMAL(18,2) NOT NULL,
        FOREIGN KEY ([SalesId]) REFERENCES [dbo].[SalesMaster]([SalesId]) ON DELETE CASCADE,
        FOREIGN KEY ([MedicineId]) REFERENCES [dbo].[Medicines]([MedicineId])
    );
END
GO

-- Insert Default User
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE Username = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] (Username, Password, FullName, IsActive, CreatedDate)
    VALUES ('admin', '123', 'Administrator', 1, GETDATE());
END
GO

-- Insert Sample Medicines
IF NOT EXISTS (SELECT * FROM [dbo].[Medicines])
BEGIN
    INSERT INTO [dbo].[Medicines] (MedicineName, BatchNo, ExpiryDate, Quantity, UnitPrice, Description, CreatedDate)
    VALUES 
        ('Paracetamol 500mg', 'BATCH001', '2025-12-31', 100, 25.50, 'Pain reliever and fever reducer', GETDATE()),
        ('Amoxicillin 250mg', 'BATCH002', '2025-11-30', 75, 45.00, 'Antibiotic', GETDATE()),
        ('Ibuprofen 400mg', 'BATCH003', '2025-10-31', 50, 35.75, 'Anti-inflammatory', GETDATE()),
        ('Cetirizine 10mg', 'BATCH004', '2026-01-31', 120, 15.25, 'Antihistamine', GETDATE()),
        ('Omeprazole 20mg', 'BATCH005', '2025-09-30', 80, 55.00, 'Proton pump inhibitor', GETDATE());
END
GO

-- Stored Procedure: sp_SaveOrUpdateSales
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SaveOrUpdateSales]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SaveOrUpdateSales]
GO

CREATE PROCEDURE [dbo].[sp_SaveOrUpdateSales]
    @SalesId INT = NULL,
    @InvoiceNumber NVARCHAR(50),
    @InvoiceDate DATETIME,
    @CustomerName NVARCHAR(100),
    @CustomerContact NVARCHAR(50),
    @SubTotal DECIMAL(18,2),
    @Discount DECIMAL(18,2),
    @GrandTotal DECIMAL(18,2),
    @SalesDetailsXML XML
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @NewSalesId INT;

        -- Insert or Update SalesMaster
        IF @SalesId IS NULL OR @SalesId = 0
        BEGIN
            INSERT INTO SalesMaster (InvoiceNumber, InvoiceDate, CustomerName, CustomerContact, SubTotal, Discount, GrandTotal, CreatedDate)
            VALUES (@InvoiceNumber, @InvoiceDate, @CustomerName, @CustomerContact, @SubTotal, @Discount, @GrandTotal, GETDATE());
            
            SET @NewSalesId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE SalesMaster
            SET InvoiceNumber = @InvoiceNumber,
                InvoiceDate = @InvoiceDate,
                CustomerName = @CustomerName,
                CustomerContact = @CustomerContact,
                SubTotal = @SubTotal,
                Discount = @Discount,
                GrandTotal = @GrandTotal
            WHERE SalesId = @SalesId;
            
            SET @NewSalesId = @SalesId;
            
            -- Delete existing details
            DELETE FROM SalesDetail WHERE SalesId = @SalesId;
        END

        -- Insert SalesDetail from XML
        INSERT INTO SalesDetail (SalesId, MedicineId, BatchNo, ExpiryDate, Quantity, UnitPrice, LineTotal)
        SELECT 
            @NewSalesId,
            T.c.value('(MedicineId)[1]', 'INT') AS MedicineId,
            T.c.value('(BatchNo)[1]', 'NVARCHAR(50)') AS BatchNo,
            T.c.value('(ExpiryDate)[1]', 'DATETIME') AS ExpiryDate,
            T.c.value('(Quantity)[1]', 'INT') AS Quantity,
            T.c.value('(UnitPrice)[1]', 'DECIMAL(18,2)') AS UnitPrice,
            T.c.value('(LineTotal)[1]', 'DECIMAL(18,2)') AS LineTotal
        FROM @SalesDetailsXML.nodes('/SalesDetails/SalesDetail') T(c);

        -- Update stock quantities
        UPDATE M
        SET M.Quantity = M.Quantity - SD.Quantity,
            M.ModifiedDate = GETDATE()
        FROM Medicines M
        INNER JOIN SalesDetail SD ON M.MedicineId = SD.MedicineId
        WHERE SD.SalesId = @NewSalesId;

        COMMIT TRANSACTION;
        
        SELECT @InvoiceNumber AS InvoiceNumber;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- Stored Procedure: sp_GetDashboardStatistics
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetDashboardStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetDashboardStatistics]
GO

CREATE PROCEDURE [dbo].[sp_GetDashboardStatistics]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return all dashboard statistics in a single result set
    SELECT 
        -- Today's statistics
        (SELECT COUNT(*) FROM SalesMaster WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)) AS TodaySalesCount,
        (SELECT ISNULL(SUM(GrandTotal), 0) FROM SalesMaster WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)) AS TodayRevenue,
        
        -- Current week's statistics
        (SELECT COUNT(*) FROM SalesMaster 
         WHERE DATEPART(WEEK, InvoiceDate) = DATEPART(WEEK, GETDATE())
         AND DATEPART(YEAR, InvoiceDate) = DATEPART(YEAR, GETDATE())) AS WeekSalesCount,
        (SELECT ISNULL(SUM(GrandTotal), 0) FROM SalesMaster 
         WHERE DATEPART(WEEK, InvoiceDate) = DATEPART(WEEK, GETDATE())
         AND DATEPART(YEAR, InvoiceDate) = DATEPART(YEAR, GETDATE())) AS WeekRevenue,
        
        -- Current month's statistics
        (SELECT COUNT(*) FROM SalesMaster 
         WHERE MONTH(InvoiceDate) = MONTH(GETDATE())
         AND YEAR(InvoiceDate) = YEAR(GETDATE())) AS MonthSalesCount,
        (SELECT ISNULL(SUM(GrandTotal), 0) FROM SalesMaster 
         WHERE MONTH(InvoiceDate) = MONTH(GETDATE())
         AND YEAR(InvoiceDate) = YEAR(GETDATE())) AS MonthRevenue,
        
        -- Total medicines count
        (SELECT COUNT(*) FROM Medicines) AS TotalMedicines,
        
        -- Low stock medicines count (quantity < 10)
        (SELECT COUNT(*) FROM Medicines WHERE Quantity < 10) AS LowStockMedicines;
END
GO
