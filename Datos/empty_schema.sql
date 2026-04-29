-- =============================================
-- DarkKitchen - Rodrigo Rey, Santiago Pedetti, Roman Ferrero
-- Base de datos VACIA 
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
    [Url]       NVARCHAR(500)   NOT NULL,
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
    [Name]               NVARCHAR(150)   NOT NULL,
    [DiscountPercentage] DECIMAL(5,2)    NOT NULL,
    [DateFrom]           DATE            NOT NULL,
    [DateTo]             DATE            NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);

-- =============================================
-- TABLA: PromotionProducts
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
    [Password]  NVARCHAR(25)   NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

-- Admin por defecto (seed de EF)
SET IDENTITY_INSERT Users ON;
INSERT INTO [Users] ([Id], [Email], [FirstName], [LastName], [Password], [Phone], [Role])
VALUES (1, 'admin@darkkitchen.com', 'Admin', 'AdminUser', 'Admin@Passw0rd!!xx', '099111222', 'Admin');
SET IDENTITY_INSERT Users OFF;

-- =============================================
-- TABLA: Orders
-- =============================================
CREATE TABLE [Orders] (
    [OrderId]            INT            IDENTITY(1,1) NOT NULL,
    [OrderNumber]        INT            NOT NULL,
    [DeliveryType]       NVARCHAR(MAX)  NOT NULL,
    [OrderStatus]        NVARCHAR(MAX)  NOT NULL,
    [ClientId]           INT            NOT NULL,
    [Subtotal]           DECIMAL(18,2)  NOT NULL,
    [ShippingCost]       DECIMAL(18,2)  NOT NULL,
    [TotalCost]          DECIMAL(18,2)  NOT NULL,
    [OrderDate]          DATETIME2      NOT NULL,
    [Address_Street]     NVARCHAR(200)  NOT NULL,
    [Address_DoorNumber] NVARCHAR(20)   NOT NULL,
    [Address_Apartment]  NVARCHAR(50)   NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_Orders_Users_ClientId]
        FOREIGN KEY ([ClientId]) REFERENCES [Users]([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Orders_ClientId] ON [Orders] ([ClientId]);

-- =============================================
-- TABLA: OrderProducts
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
