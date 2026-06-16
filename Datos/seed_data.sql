-- =============================================
-- DarkKitchen - Rodrigo Rey, Santiago Pedetti, Roman Ferrero
-- Datos de prueba (seed)
-- Ejecutar SOBRE empty_schema.sql (base recien creada y vacia).
--
-- Users y DeliveryTypes coinciden con el seed de EF Core
-- (AppDbContext.SeedData / migracion Initial). El resto son datos
-- de demostracion (productos, imagenes, promociones y ordenes).
--
-- Passwords (hash bcrypt) -> texto plano para login:
--   admin@darkkitchen.com               Admin@Passw0rd!!xx
--   rodrigo.admin@darkkitchen.com       Admin@Passw0rd!!yy
--   roman.dispatcher@darkkitchen.com    Dispatch@Passw0rd!!x
--   santiago.dispatcher@darkkitchen.com Dispatch@Passw0rd!!y
--   maia@gmail.com                      Cliente@Passw0rd!!aa
--   pilar@gmail.com                     Cliente@Passw0rd!!bb
--   francisco@gmail.com                 Cliente@Passw0rd!!dd
-- =============================================

-- =============================================
-- USERS (seed de EF: 2 Admin, 2 Dispatcher, 3 Client)
-- =============================================
SET IDENTITY_INSERT [Users] ON;

INSERT INTO [Users] ([Id], [FirstName], [LastName], [Email], [Phone], [Password], [Role]) VALUES
(1, 'Admin',     'AdminUser', 'admin@darkkitchen.com',               '099111222', '$2a$11$mMk7V81c60wLjpLLrxZlR.kDQcSoubGtXYe5qIZ4//rnQCbJKoC5W', 'Admin'),
(2, 'Rodrigo',   'Rey',       'rodrigo.admin@darkkitchen.com',       '099111333', '$2a$11$r.Zi1wj4HWTZaln2O7gf0eR8bDzJLx8ep5ZZmVLQK5dFAs/I5NBee', 'Admin'),
(3, 'Roman',     'Ferrero',   'roman.dispatcher@darkkitchen.com',    '099100200', '$2a$11$fl/TH4TtU.chzdOLfNUlnOm1gLgvDllQhxCf0zxzfO1WiVRIwyxEG', 'Dispatcher'),
(4, 'Santiago',  'Pedetti',   'santiago.dispatcher@darkkitchen.com', '099300400', '$2a$11$u7j/7G/u3HPyBCh4jhUKD.DbaYI3Hn0jwAMJB7TxbYAbhNDYySuQS', 'Dispatcher'),
(5, 'Maia',      'Terzaghi',  'maia@gmail.com',                      '099111222', '$2a$11$TAubO9G4nmnElrk1iubOJOhrLe8csJUvuhRk9HhioJ7jUPLVgKZAC', 'Client'),
(6, 'Pilar',     'Fraschini', 'pilar@gmail.com',                     '099333444', '$2a$11$Xn6cqMRr/cTDgoidQnfRf.54veJk3Ofzs..gjPRFh4lRVkclc2LVq', 'Client'),
(8, 'Francisco', 'Suarez',    'francisco@gmail.com',                 '099777888', '$2a$11$bT4q7wvuhLF6iQrOCQojr.eXUaLhZZy5C1SjeBs/GYQYmuovcYDsi', 'Client');

SET IDENTITY_INSERT [Users] OFF;

-- =============================================
-- DELIVERY TYPES (seed de EF)
-- =============================================
SET IDENTITY_INSERT [DeliveryTypes] ON;

INSERT INTO [DeliveryTypes] ([Id], [Name], [ShippingCost]) VALUES
(1, 'Express', 250.00),
(2, 'SameDay', 200.00),
(3, 'NextDay', 180.00);

SET IDENTITY_INSERT [DeliveryTypes] OFF;

-- =============================================
-- PRODUCTS
-- =============================================
SET IDENTITY_INSERT [Products] ON;

INSERT INTO [Products] ([Id], [Code], [Name], [Description], [Line], [Category], [Price], [Active]) VALUES
(1,  'PROD-A1B2C3D4', 'Milanesa napolitana',        'Milanesa de ternera con salsa de tomate casera, jamon cocido y queso gratinado al horno',     'Carnes',      'Principales', 320.00, 1),
(2,  'PROD-E5F6G7H8', 'Pizza mozzarella',           'Pizza artesanal con masa madre, salsa de tomate natural y mozzarella extra derretida',        'Pizzas',      'Principales', 450.00, 1),
(3,  'PROD-I9J0K1L2', 'Empanadas de carne x6',      'Seis empanadas de carne cortada a cuchillo con cebolla, especias criollas y aceitunas',       'Empanadas',   'Entradas',    280.00, 1),
(4,  'PROD-M3N4O5P6', 'Hamburguesa clasica',        'Hamburguesa de res 200g con lechuga, tomate, cebolla caramelizada y cheddar en pan brioche',  'Hamburguesas','Principales', 390.00, 1),
(5,  'PROD-Q7R8S9T0', 'Ensalada cesar completa',    'Ensalada cesar con pollo grillado, crutones artesanales, parmesano y aderezo casero',         'Ensaladas',   'Livianos',    220.00, 1),
(6,  'PROD-U1V2W3X4', 'Pasta bolognesa casera',     'Pasta fresca artesanal con salsa bolognesa de res cocida a fuego lento por cuatro horas',      'Pastas',      'Principales', 350.00, 1),
(7,  'PROD-Y5Z6A7B8', 'Tarta de verduras integral', 'Tarta de espinaca, choclo y queso en masa integral, apta para vegetarianos',                  'Tartas',      'Livianos',    190.00, 1),
(8,  'PROD-C9D0E1F2', 'Sopa de calabaza y jengibre','Sopa cremosa de calabaza asada con jengibre fresco, crema y semillas de zapallo',             'Sopas',       'Entradas',    180.00, 1),
(9,  'PROD-G3H4I5J6', 'Pollo al horno con papas',   'Cuarto de pollo al horno con papas rosti y ensalada fresca de temporada',                      'Carnes',      'Principales', 410.00, 1),
(10, 'PROD-K7L8M9N0', 'Wrap de pollo grillado',     'Wrap de pollo grillado con palta, tomate, lechuga y salsa de yogur a la menta',               'Wraps',       'Livianos',    260.00, 1),
(11, 'PROD-O1P2Q3R4', 'Medialunas de manteca x6',   'Seis medialunas de manteca artesanales, doradas y esponjosas, recien salidas del horno',      'Panaderia',   'Desayuno',    150.00, 1),
(12, 'PROD-S5T6U7V8', 'Budin de pan con crema',     'Budin de pan clasico uruguayo con crema inglesa y caramelo, porcion generosa',                'Postres',     'Postres',     120.00, 1),
(13, 'PROD-W9X0Y1Z2', 'Brownie de chocolate amargo','Brownie intenso de chocolate 70% cacao con nueces y centro humedo, servido tibio',            'Postres',     'Postres',     140.00, 1),
(14, 'PROD-A3B4C5D6', 'Tiramisu clasico italiano',  'Tiramisu con capas de vainillas, mascarpone, cafe espresso y cacao amargo en polvo',          'Postres',     'Postres',     160.00, 1),
(15, 'PROD-E7F8G9H0', 'Limonada natural con menta', 'Limonada natural exprimida al momento con menta fresca, jengibre y miel de abeja',            'Bebidas',     'Bebidas',      90.00, 1);

SET IDENTITY_INSERT [Products] OFF;

-- =============================================
-- PRODUCT IMAGES
-- =============================================
SET IDENTITY_INSERT [ProductImages] ON;

INSERT INTO [ProductImages] ([Id], [Url], [SizeInKb], [ProductId]) VALUES
(1,  'https://images.darkkitchen.com/milanesa-napo-1.jpg',   180.00, 1),
(2,  'https://images.darkkitchen.com/milanesa-napo-2.jpg',   210.00, 1),
(3,  'https://images.darkkitchen.com/pizza-mozza-1.jpg',     195.00, 2),
(4,  'https://images.darkkitchen.com/pizza-mozza-2.jpg',     170.00, 2),
(5,  'https://images.darkkitchen.com/empanadas-carne-1.jpg', 150.00, 3),
(6,  'https://images.darkkitchen.com/empanadas-carne-2.jpg', 165.00, 3),
(7,  'https://images.darkkitchen.com/hamburguesa-1.jpg',     200.00, 4),
(8,  'https://images.darkkitchen.com/hamburguesa-2.jpg',     185.00, 4),
(9,  'https://images.darkkitchen.com/ensalada-cesar-1.jpg',  160.00, 5),
(10, 'https://images.darkkitchen.com/pasta-bolognesa-1.jpg', 175.00, 6),
(11, 'https://images.darkkitchen.com/pasta-bolognesa-2.jpg', 190.00, 6),
(12, 'https://images.darkkitchen.com/tarta-verduras-1.jpg',  145.00, 7),
(13, 'https://images.darkkitchen.com/sopa-calabaza-1.jpg',   155.00, 8),
(14, 'https://images.darkkitchen.com/pollo-horno-1.jpg',     220.00, 9),
(15, 'https://images.darkkitchen.com/pollo-horno-2.jpg',     200.00, 9),
(16, 'https://images.darkkitchen.com/wrap-pollo-1.jpg',      168.00, 10),
(17, 'https://images.darkkitchen.com/medialunas-1.jpg',      140.00, 11),
(18, 'https://images.darkkitchen.com/budin-pan-1.jpg',       135.00, 12),
(19, 'https://images.darkkitchen.com/brownie-1.jpg',         148.00, 13),
(20, 'https://images.darkkitchen.com/tiramisu-1.jpg',        162.00, 14),
(21, 'https://images.darkkitchen.com/tiramisu-2.jpg',        158.00, 14),
(22, 'https://images.darkkitchen.com/limonada-1.jpg',        130.00, 15);

SET IDENTITY_INSERT [ProductImages] OFF;

-- =============================================
-- PROMOTIONS (fechas relativas a 2026-06-16)
--   1 Promo Fin de Semana (15%) -> vigente
--   2 Descuento Estudiantes (10%) -> vigente
--   3 Oferta Especial Julio (20%) -> futura
--   4 Promo Verano (5%) -> vencida
-- =============================================
SET IDENTITY_INSERT [Promotions] ON;

INSERT INTO [Promotions] ([Id], [Name], [DiscountPercentage], [DateFrom], [DateTo]) VALUES
(1, 'Promo Fin de Semana',   15.00, '2026-06-13', '2026-06-22'),
(2, 'Descuento Estudiantes', 10.00, '2026-06-01', '2026-08-31'),
(3, 'Oferta Especial Julio', 20.00, '2026-07-01', '2026-07-31'),
(4, 'Promo Verano',           5.00, '2026-01-01', '2026-03-31');

SET IDENTITY_INSERT [Promotions] OFF;

-- =============================================
-- PROMOTION PRODUCTS
-- =============================================
INSERT INTO [PromotionProducts] ([PromotionId], [ProductsId]) VALUES
(1, 1),  -- Fin de Semana -> Milanesa napolitana
(1, 2),  -- Fin de Semana -> Pizza mozzarella
(1, 4),  -- Fin de Semana -> Hamburguesa clasica
(2, 3),  -- Estudiantes   -> Empanadas de carne
(2, 5),  -- Estudiantes   -> Ensalada cesar
(3, 2),  -- Julio         -> Pizza mozzarella
(3, 4),  -- Julio         -> Hamburguesa clasica
(4, 6),  -- Verano        -> Pasta bolognesa
(4, 7),  -- Verano        -> Tarta de verduras
(4, 8);  -- Verano        -> Sopa de calabaza

-- =============================================
-- ORDERS
--   ClientId en {5, 6, 8} (unicos usuarios Client)
--   DeliveryType en {Express(250), SameDay(200), NextDay(180)}
--   TotalCost = (Subtotal + ShippingCost) * 1.22  (IVA 22%)
-- =============================================
SET IDENTITY_INSERT [Orders] ON;

INSERT INTO [Orders]
  ([OrderId], [OrderNumber], [DeliveryType], [OrderStatus], [ClientId],
   [Subtotal], [ShippingCost], [TotalCost], [OrderDate],
   [Address_Street], [Address_DoorNumber], [Address_Apartment])
VALUES
(1,  101001, 'Express', 'Delivered',    5, 518.00, 250.00,  936.96, '2026-04-02 12:30:00', '18 de Julio',            '1234', 'Apto 3'),
(2,  101002, 'SameDay', 'Delivered',    5, 450.00, 200.00,  793.00, '2026-04-05 18:00:00', 'Av. Italia',             '3800', NULL),
(3,  101003, 'Express', 'Delivered',    6, 650.00, 250.00, 1098.00, '2026-04-08 20:15:00', 'Bulevar Artigas',        '1200', 'Apto 12'),
(4,  101004, 'SameDay', 'Delivered',    6, 432.00, 200.00,  771.04, '2026-04-10 13:00:00', 'Rbla. Rep. de Mexico',   '5600', NULL),
(5,  101005, 'Express', 'Delivered',    8, 700.00, 250.00, 1159.00, '2026-04-12 21:00:00', 'Av. Brasil',             '2700', 'Apto 1'),
(6,  101006, 'SameDay', 'Delivered',    8, 600.00, 200.00,  976.00, '2026-04-15 19:30:00', 'Av. Luis Batlle Berres', '4500', NULL),
(7,  101007, 'Express', 'Cancelled',    5, 300.00, 250.00,  671.00, '2026-04-17 09:00:00', '18 de Julio',            '1234', 'Apto 3'),
(8,  101008, 'Express', 'Delivered',    5, 440.00, 250.00,  841.80, '2026-04-19 16:00:00', 'Dr. Luis Morquio',       '890',  NULL),
(9,  101009, 'SameDay', 'Delivered',    6, 486.00, 200.00,  836.92, '2026-04-21 14:00:00', 'Bulevar Artigas',        '1200', 'Apto 12'),
(10, 101010, 'Express', 'Delivered',    6, 420.00, 250.00,  817.40, '2026-04-23 17:30:00', 'Rbla. Rep. de Mexico',   '5600', NULL),
(11, 101011, 'Express', 'Delivered',    8, 654.50, 250.00, 1103.49, '2026-04-26 12:00:00', 'Av. Brasil',             '2700', 'Apto 1'),
(12, 101012, 'Express', 'NotDelivered', 8, 753.00, 250.00, 1223.66, '2026-04-27 20:00:00', 'Av. Luis Batlle Berres', '4500', NULL),
(13, 101013, 'Express', 'OnTheWay',     5, 608.00, 250.00, 1046.76, '2026-04-28 19:00:00', '18 de Julio',            '1234', 'Apto 3'),
(14, 101014, 'SameDay', 'Prepared',     5, 642.50, 200.00, 1027.85, '2026-04-28 21:30:00', 'Dr. Luis Morquio',       '890',  NULL),
(15, 101015, 'SameDay', 'Pending',      6, 603.50, 200.00,  980.27, '2026-04-29 10:00:00', 'Bulevar Artigas',        '1200', NULL),
(16, 101016, 'Express', 'Pending',      6, 504.00, 250.00,  919.88, '2026-04-29 11:00:00', 'Rbla. Rep. de Mexico',   '5600', NULL),
(17, 101017, 'Express', 'Pending',      8, 704.00, 250.00, 1163.88, '2026-04-29 11:45:00', 'Av. Brasil',             '2700', 'Apto 1');

SET IDENTITY_INSERT [Orders] OFF;

-- =============================================
-- ORDER PRODUCTS
-- =============================================
INSERT INTO [OrderProducts] ([OrderId], [ProductId], [Quantity]) VALUES
(1,  1,  1), (1,  5,  1),
(2,  2,  1),
(3,  4,  1), (3,  10, 1),
(4,  3,  1), (4,  8,  1),
(5,  6,  2),
(6,  9,  1), (6,  7,  1),
(7,  11, 2),
(8,  13, 2), (8,  14, 1),
(9,  5,  2), (9,  15, 1),
(10, 12, 1), (10, 13, 1), (10, 14, 1),
(11, 1,  1), (11, 2,  1),
(12, 4,  2), (12, 15, 1),
(13, 9,  1), (13, 5,  1),
(14, 2,  1), (14, 10, 1),
(15, 1,  1), (15, 4,  1),
(16, 3,  2),
(17, 1,  1), (17, 3,  1), (17, 15, 2);
