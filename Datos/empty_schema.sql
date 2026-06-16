-- =============================================
-- DarkKitchen - Rodrigo Rey, Santiago Pedetti, Roman Ferrero
-- Base de datos VACIA (solo estructura, sin datos)
-- Generado a partir del modelo de EF Core (migracion Initial)
-- =============================================

-- =============================================
-- TABLA: Products
-- =============================================
CREATE TABLE [Products] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [Code]        NVARCHAR(20)   NOT NULL,
    [Name]        NVARCHAR(50)   NOT NULL,
    [Description] NVARCHAR(500)  NOT NULL,
    [Line]        NVARCHAR(100)  NOT NULL,
    [Category]    NVARCHAR(100)  NOT NULL,
    [Price]       DECIMAL(18,2)  NOT NULL,
    [Active]      BIT            NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_Products_Code] ON [Products] ([Code]);

-- =============================================
-- TABLA: ProductImages
-- =============================================
CREATE TABLE [ProductImages] (
    [Id]        INT             IDENTITY(1,1) NOT NULL,
    [Url]       NVARCHAR(MAX)   NOT NULL,
    [SizeInKb]  DECIMAL(10,2)   NOT NULL,
    [ProductId] INT             NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductImages_Products_ProductId]
        FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ProductImages_ProductId] ON [ProductImages] ([ProductId]);

-- =============================================
-- TABLA: Promotions
-- =============================================
CREATE TABLE [Promotions] (
    [Id]                 INT             IDENTITY(1,1) NOT NULL,
    [DateFrom]           DATE            NOT NULL,
    [DateTo]             DATE            NOT NULL,
    [Name]               NVARCHAR(150)   NOT NULL,
    [DiscountPercentage] DECIMAL(5,2)    NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);

-- =============================================
-- TABLA: PromotionProducts (relacion N:N Promotions <-> Products)
-- =============================================
CREATE TABLE [PromotionProducts] (
    [ProductsId]  INT NOT NULL,
    [PromotionId] INT NOT NULL,
    CONSTRAINT [PK_PromotionProducts] PRIMARY KEY ([ProductsId], [PromotionId]),
    CONSTRAINT [FK_PromotionProducts_Products_ProductsId]
        FOREIGN KEY ([ProductsId]) REFERENCES [Products]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PromotionProducts_Promotions_PromotionId]
        FOREIGN KEY ([PromotionId]) REFERENCES [Promotions]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PromotionProducts_PromotionId] ON [PromotionProducts] ([PromotionId]);

-- =============================================
-- TABLA: Users
-- =============================================
CREATE TABLE [Users] (
    [Id]        INT            IDENTITY(1,1) NOT NULL,
    [Role]      NVARCHAR(MAX)  NOT NULL,
    [FirstName] NVARCHAR(50)   NOT NULL,
    [LastName]  NVARCHAR(50)   NOT NULL,
    [Email]     NVARCHAR(50)   NOT NULL,
    [Phone]     NVARCHAR(20)   NOT NULL,
    [Password]  NVARCHAR(100)  NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

-- =============================================
-- TABLA: DeliveryTypes
-- =============================================
CREATE TABLE [DeliveryTypes] (
    [Id]           INT            IDENTITY(1,1) NOT NULL,
    [Name]         NVARCHAR(100)  NOT NULL,
    [ShippingCost] DECIMAL(18,2)  NOT NULL,
    CONSTRAINT [PK_DeliveryTypes] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_DeliveryTypes_Name] ON [DeliveryTypes] ([Name]);

-- =============================================
-- TABLA: Orders
-- =============================================
CREATE TABLE [Orders] (
    [OrderId]            INT            IDENTITY(1,1) NOT NULL,
    [DeliveryType]       NVARCHAR(100)  NOT NULL,
    [Address_Street]     NVARCHAR(200)  NOT NULL,
    [Address_DoorNumber] NVARCHAR(20)   NOT NULL,
    [Address_Apartment]  NVARCHAR(50)   NULL,
    [OrderStatus]        NVARCHAR(MAX)  NOT NULL,
    [ClientId]           INT            NOT NULL,
    [OrderNumber]        INT            NOT NULL,
    [Subtotal]           DECIMAL(18,2)  NOT NULL,
    [ShippingCost]       DECIMAL(18,2)  NOT NULL,
    [TotalCost]          DECIMAL(18,2)  NOT NULL,
    [OrderDate]          DATETIME2      NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_Orders_Users_ClientId]
        FOREIGN KEY ([ClientId]) REFERENCES [Users]([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Orders_ClientId] ON [Orders] ([ClientId]);

-- =============================================
-- TABLA: OrderProducts (relacion N:N Orders <-> Products)
-- =============================================
CREATE TABLE [OrderProducts] (
    [OrderId]   INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity]  INT NOT NULL,
    CONSTRAINT [PK_OrderProducts] PRIMARY KEY ([OrderId], [ProductId]),
    CONSTRAINT [FK_OrderProducts_Orders_OrderId]
        FOREIGN KEY ([OrderId]) REFERENCES [Orders]([OrderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderProducts_Products_ProductId]
        FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_OrderProducts_ProductId] ON [OrderProducts] ([ProductId]);

-- =============================================
-- TABLA: AuditLogs
-- =============================================
CREATE TABLE [AuditLogs] (
    [Id]              INT            IDENTITY(1,1) NOT NULL,
    [Timestamp]       DATETIME2      NOT NULL,
    [EntityName]      NVARCHAR(100)  NOT NULL,
    [EntityId]        INT            NOT NULL,
    [Description]     NVARCHAR(500)  NOT NULL,
    [ResponsibleUser] NVARCHAR(100)  NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);
