using System.Diagnostics;
using System.IO;
using System.Windows;
using Gestion.core.model;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;

namespace Gestion.presentation.utils
{
    public static class PrintUtils
    {
        public static string GeneratePdf(string fileName, Action<Document> builder)
        {
            string path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                fileName);

            using var writer = new iText.Kernel.Pdf.PdfWriter(path);
            using var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
            using var doc = new iText.Layout.Document(pdf);

            builder(doc);

            return path; 
        }
        
        public static void PrintFile(string filePath, string printer, string sumatraPath)
        {
            if (!File.Exists(sumatraPath))
                throw new FileNotFoundException("No se encontró SumatraPDF.exe", sumatraPath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("No existe el archivo a imprimir", filePath);

            var p = new Process();
            p.StartInfo.FileName = sumatraPath;
            p.StartInfo.Arguments = $"-print-to \"{printer}\" \"{filePath}\"";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();

            p.WaitForExit();
        }

        public static string GenerarOrdenTrabajoPrint_old(OrdenTrabajo ot)
        {
           return GeneratePdf("orden_trabajo_"+ot.Folio+".pdf", doc =>
            {
                var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
                doc.SetFont(font).SetFontSize(10);

                string T(object? v) =>
                    string.IsNullOrWhiteSpace(v?.ToString()) ? "---" : v!.ToString();

                // ENCABEZADO
                doc.Add(new Paragraph("IMPRENTA MORIS")
                    .SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Center)
                    .SetFont(boldFont)
                    .SetFontSize(12));

                doc.Add(new Paragraph("ADMINISTRADOR DE SERVICIOS  V1.0")
                    .SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Center)
                    .SetFontSize(10));

                doc.Add(new Paragraph($"Emitido : {DateTime.Now:dd/MM/yyyy}       Página : 1")
                    .SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Center));

                doc.Add(new Paragraph("\nORDEN DE TRABAJO")
                    .SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Center)
                    .SetFont(boldFont));

                doc.Add(new Paragraph("============================================")
                    .SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Center));

                // DATOS GENERALES
                doc.Add(new Paragraph(
                    $"Folio O.T.: [{T(ot.Folio)}]     Cliente: [{T(ot.RutCliente)}]\n" +
                    $"Nombre Cliente: {T(ot.Descripcion)}"
                ));

                doc.Add(new Paragraph($"Fecha: [{ot.Fecha:dd/MM/yyyy}]"));
                doc.Add(new Paragraph(new string('-', 80)));

                // TRABAJO
                doc.Add(new Paragraph("► DETALLE DEL TRABAJO").SetFont(boldFont));
                doc.Add(new Paragraph(
                    $"Trabajo:            [{T(ot.Descripcion)}]\n" +
                    $"Cantidad:           [{ot.Cantidad}]\n" +
                    $"Total impresiones:  [{ot.TotalImpresion}]\n" +
                    $"Folio del:          [{T(ot.FolioDesde)}]   al [{T(ot.FolioHasta)}]\n" +
                    $"Cortar a tamaño:    [{ot.CortarTamanio}]  {T(ot.CortarTamanioLargo)} x {T(ot.CortarTamanion)} cms\n" +
                    $"Montar:             [{ot.Montar}]\n" +
                    $"Molde x tamaño:     [{T(ot.MoldeTamanio)}]\n" +
                    $"Tamaño final:       {T(ot.TamanioFinalAncho)} x {T(ot.TamanioFinalLargo)} cms"
                ));

                doc.Add(new Paragraph(new string('-', 80)));

                // CLIENTE PROPORCIONA
                doc.Add(new Paragraph("► CLIENTE PROPORCIONA").SetFont(boldFont));
                doc.Add(new Paragraph(
                    $"Nada [{T(ot.ClienteProporcionanada)}]    " +
                    $"Original [{T(ot.ClienteProporcionaOriginal)}]    " +
                    $"Películas [{(ot.ClienteProporcionaPelicula ? "X" : " ")}]    " +
                    $"Planchas [{(ot.ClienteProporcionaPlancha ? "X" : " ")}]    " +
                    $"Papel [{(ot.ClienteProporcionaPapel ? "X" : " ")}]"
                ));

                doc.Add(new Paragraph(new string('-', 80)));

                // IMPRESIÓN Y MÁQUINAS
                doc.Add(new Paragraph("► IMPRESIÓN Y MÁQUINAS").SetFont(boldFont));
                doc.Add(new Paragraph(
                    $"Tipo impresión:     [{T(ot.TipoImpresion)}]\n" +
                    $"Máquina 1:          [{T(ot.Maquina1)}]\n" +
                    $"Máquina 2:          [{T(ot.Maquina2)}]\n" +
                    $"Pinza:              [{T(ot.Pin)}]\n" +
                    $"Planchas nuevas:    [{ot.Nva}]     Usadas: [{ot.Us}]\n" +
                    $"Planchas CTP nuevas:[{ot.CtpNva}]   Usadas CTP: [{ot.U}]"
                ));

                doc.Add(new Paragraph(new string('-', 80)));

                // TINTAS
                doc.Add(new Paragraph("► TINTAS").SetFont(boldFont));
                doc.Add(new Paragraph(
                    $"1[{T(ot.Tintas1)}]   2[{T(ot.Tintas2)}]   3[{T(ot.Tintas3)}]   4[{T(ot.Tintas4)}]"
                ));

                doc.Add(new Paragraph(new string('-', 80)));

                // MATERIALES
                doc.Add(new Paragraph("► MATERIALES").SetFont(boldFont));
                doc.Add(new Paragraph(
                    $"Sobres: [{T(ot.Sobres)}]\n" +
                    $"Sacos:  [{T(ot.Sacos)}]"
                ));

                doc.Add(new Paragraph(new string('-', 80)));

                // TOTALES
                doc.Add(new Paragraph("Películas y Planchas:     $____________"));
                doc.Add(new Paragraph("Impresión:                 $____________"));
            });
        }
    
        public static string GenerarOrdenTrabajoPrint(OrdenTrabajo ot)
        {
            return GeneratePdf($"orden_trabajo_{ot.Folio}.pdf", doc =>
            {
                var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                var bold = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

                doc.SetFont(font).SetFontSize(8);

                var logo = new Image(ImageDataFactory.Create("C:\\Users\\Jose\\Desktop\\DevMate\\DISEÑOS\\devmate-removebg-preview.png")).ScaleToFit(60, 40);
                // ===== ENCABEZADO =====
                Table header = new Table(new float[] { 1.2f, 4.5f, 2f, 1f, 1f, 1f })
                    .UseAllAvailableWidth()
                    .SetFixedLayout();

                // ALTURA COMÚN
                float headerHeight = 28;

                // LOGO
                header.AddCell(
                    new Cell()
                        .Add(logo)
                        .SetHeight(headerHeight)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.BOTTOM)
                        .SetBorder(Border.NO_BORDER)
                );

                // TITULO
                header.AddCell(
                    new Cell()
                        .Add(new Paragraph("ORDEN DE TRABAJO")
                            .SetFont(bold)
                            .SetFontSize(11))
                        .SetHeight(headerHeight)
                        .SetBorderTop(new SolidBorder(1))     
                        .SetBorderLeft(new SolidBorder(1))    
                        .SetBorderRight(new SolidBorder(1))
                        .SetBorderBottom(Border.NO_BORDER)
                        .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE)
                );

                // N°
                header.AddCell(
                    new Cell()
                        .Add(new Paragraph($"N° {ot.Folio}")
                            .SetFont(bold)
                            .SetFontSize(10)
                            .SetFontColor(new DeviceRgb(0, 120, 0))) // verde similar
                        .SetHeight(headerHeight)
                        .SetBorderTop(new SolidBorder(1))     
                        .SetBorderLeft(new SolidBorder(1))    
                        .SetBorderRight(new SolidBorder(1))
                        .SetBorderBottom(Border.NO_BORDER)
                        .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE)
                );

                // DIA
                header.AddCell(HeaderDateCell("DIA", ot.Fecha.Day.ToString(), headerHeight));
                // MES
                header.AddCell(HeaderDateCell("MES", ot.Fecha.Month.ToString(), headerHeight));
                // AÑO
                header.AddCell(HeaderDateCell("AÑO", ot.Fecha.Year.ToString(), headerHeight));

                doc.Add(header);

                // ===== DATOS CLIENTE =====
                Table datos = new Table(4).UseAllAvailableWidth();

                datos.AddCell(CellBox("CLIENTE"));
                datos.AddCell(CellBox(ot.RazonSocial));
                datos.AddCell(CellBox("RUT"));
                datos.AddCell(CellBox(ot.RutCliente));

                datos.AddCell(CellBox("TRABAJO"));
                datos.AddCell(CellBox(ot.Descripcion));
                datos.AddCell(CellBox("FECHA"));
                datos.AddCell(CellBox(ot.Fecha.ToString("dd/MM/yyyy")));

                doc.Add(datos);

                // ===== MEDIDAS =====
                Table medidas = new Table(4).UseAllAvailableWidth();

                medidas.AddCell(CellBox("CANTIDAD"));
                medidas.AddCell(CellBox(ot.Cantidad.ToString()));
                medidas.AddCell(CellBox("TOTAL IMPRESIONES"));
                medidas.AddCell(CellBox(ot.TotalImpresion.ToString()));

                medidas.AddCell(CellBox("CORTAR A TAMAÑO"));
                medidas.AddCell(CellBox($"{ot.CortarTamanioLargo} x {ot.CortarTamanion} cm"));
                medidas.AddCell(CellBox("TAMAÑO FINAL"));
                medidas.AddCell(CellBox($"{ot.TamanioFinalAncho} x {ot.TamanioFinalLargo} cm"));

                doc.Add(medidas);

                // ===== CLIENTE PROPORCIONA =====
                Table proporciona = new Table(5).UseAllAvailableWidth();

                proporciona.AddCell(CellBox("CLIENTE PROPORCIONA", true, 5));
                proporciona.AddCell(Check("Nada", ot.ClienteProporcionanada == "1" ? true : false));
                proporciona.AddCell(Check("Original", ot.ClienteProporcionaOriginal == "1"  ? true : false));
                proporciona.AddCell(Check("Planchas", ot.ClienteProporcionaPlancha));
                proporciona.AddCell(Check("Papel", ot.ClienteProporcionaPapel));
                proporciona.AddCell(Check("Películas", ot.ClienteProporcionaPelicula));

                doc.Add(proporciona);

                // ===== IMPRESIÓN =====
                Table impresion = new Table(4).UseAllAvailableWidth();

                impresion.AddCell(CellBox("TIPO IMPRESIÓN"));
                impresion.AddCell(CellBox(ot.TipoImpresion));
                impresion.AddCell(CellBox("PINZA"));
                impresion.AddCell(CellBox(ot.Pin));

                impresion.AddCell(CellBox("MÁQUINA"));
                impresion.AddCell(CellBox(ot.Maquina1));
                impresion.AddCell(CellBox("PLANCHAS NUEVAS"));
                impresion.AddCell(CellBox(ot.Nva.ToString()));

                doc.Add(impresion);

                // ===== TINTAS =====
                Table tintas = new Table(4).UseAllAvailableWidth();
                tintas.AddCell(CellBox("TINTAS", true,4));
                tintas.AddCell(CellBox("1 " + ot.Tintas1));
                tintas.AddCell(CellBox("2 " + ot.Tintas2));
                tintas.AddCell(CellBox("3 " + ot.Tintas3));
                tintas.AddCell(CellBox("4 " + ot.Tintas4));

                doc.Add(tintas);

                // ===== OBSERVACIONES =====
                Table obs = new Table(1).UseAllAvailableWidth();
                obs.AddCell(
                    new Cell().Add(new Paragraph("OBSERVACIONES\n\n\n\n"))
                    .SetHeight(120)
                    .SetBorder(new SolidBorder(1))
                );

                doc.Add(obs);

                // ===== FIRMA =====
                Table firma = new Table(2).UseAllAvailableWidth();
                firma.AddCell(CellBox("FIRMA CLIENTE\n\n\n", true));
                firma.AddCell(CellBox("RUT\n\n\n", true));

                doc.Add(firma);
            });
        }
        public static string GenerarListadoOTPendientes(List<OrdenTrabajo> lista)
        {
            return GeneratePdf("reporte_ordenes_trabajo.pdf", doc =>
            {
                var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                doc.SetFont(font).SetFontSize(7);

                // Reducir márgenes para que quepa más texto horizontal
                doc.GetPdfDocument().SetDefaultPageSize(new PageSize(PageSize.A4));
                doc.SetMargins(20, 20, 20, 20);

                // Encabezado
                doc.Add(new Paragraph("IMPRENTA MORIS"));
                doc.Add(new Paragraph("ADMINISTRADOR DE SERVICIOS V1.0"));
                doc.Add(new Paragraph($"Emitido : {DateTime.Now:dd/MM/yyyy}                     Pagina : 1"));

                doc.Add(new Paragraph("\nORDENES DE TRABAJO PENDIENTES DE PRODUCCION POR MAQUINA")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                doc.Add(new Paragraph(new string('-', 120))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                // Rango fechas
                DateTime desde = lista.Min(x => x.Fecha);
                DateTime hasta = lista.Max(x => x.Fecha);

                doc.Add(new Paragraph(
                    $"Entre el {desde:dd/MM/yyyy} y el {hasta:dd/MM/yyyy}\n"
                ).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                doc.Add(new Paragraph(new string('-', 120)));

                // Ajustar ancho de columnas
                doc.Add(new Paragraph(
                    $"{Pad("FOLIO", 12)}" +
                    $"{Pad("FECHA", 12)}" +
                    $"{Pad("RAZON SOCIAL", 12)}" +
                    $"{Pad("R.U.T.", 14)}" +
                    $"{Pad("TRABAJO", 30)}" +   // ← más ancho
                    $"{Pad("MAQ.", 6)}" +
                    $"{Pad("DESCRIPCION", 22)}" + // ← más ancho
                    $"{Pad("F.P.T.", 6)}" +
                    $"{Pad("MONTO", 6)}"
                ));

                doc.Add(new Paragraph(new string('-', 120)));

                // Filas
                foreach (var ot in lista.OrderBy(o => o.Fecha))
                {
                    string linea =
                        Pad(ot.Folio.ToString(), 12) +
                        Pad(ot.Fecha.ToString("dd/MM/yyyy"), 12) +
                        Pad(ot.RazonSocial ?? "", 12) +
                        Pad(ot.RutCliente ?? "", 14) +
                        Pad(ot.Descripcion ?? "", 30) +     // ← más ancho
                        Pad(ot.Maquina1 ?? "", 6) +
                        Pad(ot.Maquina1descripcion ?? "", 22) +   // ← más ancho
                        Pad($"{(ot.Nva == 1 ? "N" : ot.Us == 1 ? "U" : "")}", 6) +
                        Pad("0", 6);

                    doc.Add(new Paragraph(linea));
                }

                doc.Add(new Paragraph(new string('-', 120)));
            });
        }

        private static string Pad(string text, int width)
        {
            if (text == null) text = "";
            return text.Length > width ? text.Substring(0, width) : text.PadRight(width);
        }
    


    static Cell CellBox(string text, bool bold = false, int colSpan = 1)
    {
        var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

        return new Cell(1, colSpan)
            .Add(new Paragraph(text)
                .SetFont(bold ? boldFont : font)
                .SetFontSize(8))
            .SetPadding(4)
            .SetBorder(new SolidBorder(1));
    }

    static Cell Check(string label, bool value)
    {
        return CellBox($"{(value ? "[X]" : "[ ]")} {label}");
    }

    static Cell HeaderDateCell(string label, string value, float height)
    {
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
        return new Cell()
            .Add(new Paragraph(label)
                .SetFontSize(6))
            .Add(new Paragraph(value)
                .SetFontSize(8)
                .SetFont(boldFont))
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE)
            .SetHeight(height)
            .SetBorderTop(new SolidBorder(1))     
            .SetBorderLeft(new SolidBorder(1))    
            .SetBorderRight(new SolidBorder(1))
            .SetBorderBottom(Border.NO_BORDER);
    }
    }
}