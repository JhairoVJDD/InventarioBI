using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using InventarioBI.Models;

namespace InventarioBI.Services
{
    public class ExcelService
    {
        public byte[] ExportarMovimientos(IEnumerable<MovimientoInventario> movimientos)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Movimientos");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Fecha";
            worksheet.Cell(1, 2).Value = "Producto";
            worksheet.Cell(1, 3).Value = "Tipo";
            worksheet.Cell(1, 4).Value = "Cantidad";
            worksheet.Cell(1, 5).Value = "Stock Anterior";
            worksheet.Cell(1, 6).Value = "Stock Nuevo";
            worksheet.Cell(1, 7).Value = "Usuario";
            worksheet.Cell(1, 8).Value = "Motivo";

            int row = 2;
            foreach (var m in movimientos)
            {
                worksheet.Cell(row, 1).Value = m.Fecha;
                worksheet.Cell(row, 2).Value = m.Producto?.Descripcion;
                worksheet.Cell(row, 3).Value = m.TipoMovimiento;
                worksheet.Cell(row, 4).Value = m.Cantidad;
                worksheet.Cell(row, 5).Value = m.StockAnterior;
                worksheet.Cell(row, 6).Value = m.StockNuevo;
                worksheet.Cell(row, 7).Value = m.UsuarioResponsable;
                worksheet.Cell(row, 8).Value = m.Motivo;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportarConteos(IEnumerable<ConteoFisico> conteos)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Conteos Físicos");

            worksheet.Cell(1, 1).Value = "Fecha";
            worksheet.Cell(1, 2).Value = "Producto";
            worksheet.Cell(1, 3).Value = "Stock Sistema";
            worksheet.Cell(1, 4).Value = "Stock Físico";
            worksheet.Cell(1, 5).Value = "Diferencia";
            worksheet.Cell(1, 6).Value = "Estado";

            int row = 2;
            foreach (var c in conteos)
            {
                worksheet.Cell(row, 1).Value = c.FechaConteo;
                worksheet.Cell(row, 2).Value = c.Producto?.Descripcion;
                worksheet.Cell(row, 3).Value = c.StockSistema;
                worksheet.Cell(row, 4).Value = c.StockFisico;
                worksheet.Cell(row, 5).Value = c.Diferencia;
                worksheet.Cell(row, 6).Value = c.Estado;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    public byte[] ExportarProductos(IEnumerable<Producto> productos)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Productos");

            worksheet.Cell(1, 1).Value = "Código de Barras";
            worksheet.Cell(1, 2).Value = "Descripción";
            worksheet.Cell(1, 3).Value = "Categoría";
            worksheet.Cell(1, 4).Value = "Stock Actual";
            worksheet.Cell(1, 5).Value = "Precio Costo";
            worksheet.Cell(1, 6).Value = "Precio Venta";

            int row = 2;
            foreach (var p in productos)
            {
                worksheet.Cell(row, 1).Value = p.CodigoBarras;
                worksheet.Cell(row, 2).Value = p.Descripcion;
                worksheet.Cell(row, 3).Value = p.Categoria;
                worksheet.Cell(row, 4).Value = p.StockActual;
                worksheet.Cell(row, 5).Value = p.PrecioCosto;
                worksheet.Cell(row, 6).Value = p.PrecioVenta;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
            }
        }
}
