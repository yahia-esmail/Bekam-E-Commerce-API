INSERT INTO Brands (Name, CreatedBy, CreatedOn)
VALUES
('Apple', 'seed', GETDATE()),
('Samsung', 'seed', GETDATE()),
('Sony', 'seed', GETDATE()),
('Dell', 'seed', GETDATE()),
('HP', 'seed', GETDATE());


INSERT INTO Categories (Name, CreatedBy, CreatedOn)
VALUES
('Smartphones', 'seed', GETDATE()),
('Laptops', 'seed', GETDATE()),
('Accessories', 'seed', GETDATE()),
('Monitors', 'seed', GETDATE());


INSERT INTO Products
(
    Name,
    NormalizedName,
    Description,
    PictureUrl,
    Price,
    BrandId,
    CategoryId,
    CreatedBy,
    CreatedOn
)
VALUES
-- Apple
('iPhone 15 Pro', 'IPHONE 15 PRO', 'Latest Apple flagship phone', NULL, 1200.00, 1, 1, 'seed', GETDATE()),
('iPhone 14', 'IPHONE 14', 'Previous generation iPhone', NULL, 900.00, 1, 1, 'seed', GETDATE()),
('MacBook Air M2', 'MACBOOK AIR M2', 'Lightweight laptop with M2 chip', NULL, 1500.00, 1, 2, 'seed', GETDATE()),
('Apple Watch Series 9', 'APPLE WATCH SERIES 9', 'Smart watch with health tracking', NULL, 450.00, 1, 3, 'seed', GETDATE()),

-- Samsung
('Galaxy S24 Ultra', 'GALAXY S24 ULTRA', 'High-end Android smartphone', NULL, 1100.00, 2, 1, 'seed', GETDATE()),
('Galaxy S23', 'GALAXY S23', 'Compact flagship phone', NULL, 850.00, 2, 1, 'seed', GETDATE()),
('Samsung Galaxy Book', 'SAMSUNG GALAXY BOOK', 'Slim Windows laptop', NULL, 1300.00, 2, 2, 'seed', GETDATE()),
('Samsung 27 Inch Monitor', 'SAMSUNG 27 INCH MONITOR', '4K UHD Monitor', NULL, 400.00, 2, 4, 'seed', GETDATE()),

-- Sony
('Sony Xperia 1 V', 'SONY XPERIA 1 V', 'Premium Sony smartphone', NULL, 1000.00, 3, 1, 'seed', GETDATE()),
('Sony WH-1000XM5', 'SONY WH-1000XM5', 'Noise cancelling headphones', NULL, 380.00, 3, 3, 'seed', GETDATE()),
('Sony 24 Inch Monitor', 'SONY 24 INCH MONITOR', 'Full HD Monitor', NULL, 250.00, 3, 4, 'seed', GETDATE()),
('Sony VAIO Laptop', 'SONY VAIO LAPTOP', 'Business laptop', NULL, 1400.00, 3, 2, 'seed', GETDATE()),

-- Dell
('Dell XPS 13', 'DELL XPS 13', 'Premium ultrabook', NULL, 1600.00, 4, 2, 'seed', GETDATE()),
('Dell Inspiron 15', 'DELL INSPIRON 15', 'Affordable everyday laptop', NULL, 900.00, 4, 2, 'seed', GETDATE()),
('Dell 27 Inch Monitor', 'DELL 27 INCH MONITOR', 'QHD Monitor', NULL, 350.00, 4, 4, 'seed', GETDATE()),
('Dell USB-C Dock', 'DELL USB-C DOCK', 'Docking station', NULL, 220.00, 4, 3, 'seed', GETDATE()),

-- HP
('HP Spectre x360', 'HP SPECTRE X360', 'Convertible premium laptop', NULL, 1700.00, 5, 2, 'seed', GETDATE()),
('HP Pavilion 15', 'HP PAVILION 15', 'Mid-range laptop', NULL, 850.00, 5, 2, 'seed', GETDATE()),
('HP 24 Inch Monitor', 'HP 24 INCH MONITOR', 'IPS Monitor', NULL, 280.00, 5, 4, 'seed', GETDATE()),
('HP Wireless Mouse', 'HP WIRELESS MOUSE', 'Ergonomic wireless mouse', NULL, 45.00, 5, 3, 'seed', GETDATE());
