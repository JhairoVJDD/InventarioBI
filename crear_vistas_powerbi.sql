-- ==========================================================
-- SCRIPT: Creación de Vistas Optimizadas para Power BI (Corregido)
-- PROYECTO: InventarioBI (Business Intelligence)
-- BASE DE DATOS: InventarioBI_DB
-- ==========================================================

USE [InventarioBI_DB];
GO

-- 1. Vista de Productos con Categoría, Marca y Valorización
IF OBJECT_ID('v_ProductosBI', 'V') IS NOT NULL 
    DROP VIEW v_ProductosBI;
GO

CREATE VIEW v_ProductosBI AS
SELECT 
    p.IdProducto,
    p.CodigoBarras AS Codigo,
    p.Descripcion AS Producto,
    c.Nombre AS Categoria,
    m.Nombre AS Marca,
    p.StockActual,
    p.PrecioCosto,
    p.PrecioVenta,
    (p.StockActual * p.PrecioCosto) AS StockValorizadoCosto,
    (p.StockActual * p.PrecioVenta) AS StockValorizadoVenta,
    CASE WHEN p.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS EstadoProducto
FROM Productos p
LEFT JOIN Categorias c ON p.Categoria = c.IdCategoria
LEFT JOIN Marcas m ON p.Marca = m.IdMarca;
GO

-- 2. Vista de Movimientos de Inventario
IF OBJECT_ID('v_MovimientosBI', 'V') IS NOT NULL 
    DROP VIEW v_MovimientosBI;
GO

CREATE VIEW v_MovimientosBI AS
SELECT 
    m.IdMovimiento,
    p.Descripcion AS Producto,
    c.Nombre AS Categoria,
    b.Nombre AS Marca,
    t.Nombre AS Tienda,
    m.TipoMovimiento,
    m.Cantidad,
    m.StockAnterior,
    m.StockNuevo,
    m.Motivo,
    m.UsuarioResponsable,
    m.Fecha
FROM MovimientosInventario m
LEFT JOIN Productos p ON m.IdProducto = p.IdProducto
LEFT JOIN Categorias c ON p.Categoria = c.IdCategoria
LEFT JOIN Marcas b ON p.Marca = b.IdMarca
LEFT JOIN Tiendas t ON m.IdTienda = t.IdTienda;
GO

-- 3. Vista de Conteos Físicos y Anomalías
IF OBJECT_ID('v_ConteosBI', 'V') IS NOT NULL 
    DROP VIEW v_ConteosBI;
GO

CREATE VIEW v_ConteosBI AS
SELECT 
    cf.IdConteo,
    p.Descripcion AS Producto,
    c.Nombre AS Categoria,
    b.Nombre AS Marca,
    t.Nombre AS Tienda,
    cf.StockSistema,
    cf.StockFisico,
    (cf.StockFisico - cf.StockSistema) AS Diferencia,
    ABS(cf.StockFisico - cf.StockSistema) AS DiferenciaAbsoluta,
    cf.Estado,
    cf.Comentario,
    cf.FechaConteo
FROM ConteosFisicos cf
LEFT JOIN Productos p ON cf.IdProducto = p.IdProducto
LEFT JOIN Categorias c ON p.Categoria = c.IdCategoria
LEFT JOIN Marcas b ON p.Marca = b.IdMarca
LEFT JOIN Tiendas t ON cf.IdTienda = t.IdTienda;
GO
