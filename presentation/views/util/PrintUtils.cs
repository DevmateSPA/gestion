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
using iText.Layout.Properties;

namespace Gestion.presentation.views.util;

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

    public static string GenerarOrdenTrabajoPrint(OrdenTrabajo ot)
    {
        return GeneratePdf($"orden_trabajo_{ot.Folio}.pdf", doc =>
        {
            var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            var bold = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

            doc.SetFont(font).SetFontSize(8);

            var logo = new Image(ImageDataFactory.Create("C:\\Users\\elthe\\Desktop\\devmate\\gestion\\resources\\devmate-logo.jpeg")).ScaleToFit(60, 40);
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
                    .SetBorderTop(new SolidBorder(0.5f))
                    .SetBorderLeft(new SolidBorder(0.5f))
                    .SetBorderRight(new SolidBorder(0.5f))
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
                    .SetBorderTop(new SolidBorder(0.5f))
                    .SetBorderLeft(new SolidBorder(0.5f))
                    .SetBorderRight(new SolidBorder(0.5f))
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

            datos.AddCell(CellBox("CLIENTE", rightBorder: false));
            datos.AddCell(CellBox(ot.RazonSocial, leftBorder: false, rightBorder: false));
            datos.AddCell(CellBox("RUT", leftBorder: false, rightBorder: false));
            datos.AddCell(CellBox(ot.RutCliente, leftBorder: false));

            datos.AddCell(CellBox("TRABAJO", rightBorder: false));
            datos.AddCell(CellBox(ot.Descripcion, leftBorder: false, colSpan: 3));


            doc.Add(datos);

            // ===== MEDIDAS =====
            Table medidas = new Table(8).UseAllAvailableWidth();

            medidas.AddCell(CellBox("CANTIDAD", rightBorder: false));
            medidas.AddCell(CellBox(ot.Cantidad.ToString(), leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("TOTAL IMPRESIONES", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox(ot.TotalImpresion.ToString(), leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("FOLIO DEL N°", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox(ot.FolioDesde, leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("AL N°", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox(ot.FolioHasta, leftBorder: false));

            medidas.AddCell(CellBox("CORTAR A TAMAÑO", rightBorder: false));
            medidas.AddCell(CellBox($"{ot.CortarTamanio}", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("DE", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox($"{ot.CortarTamanion}", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("x", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox($"{ot.CortarTamanioLargo}", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("CM.", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("", leftBorder: false));

            medidas.AddCell(CellBox("MONTAR DE:", rightBorder: false));
            medidas.AddCell(CellBox($"{ot.Montar}", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("MOLDES POR TAMAÑO", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox($"{ot.MoldeTamanio}", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("TAMAÑO FINAL:", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox($"{ot.TamanioFinalAncho}", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox("X", leftBorder: false, rightBorder: false));
            medidas.AddCell(CellBox($"{ot.TamanioFinalAncho} CM.", leftBorder: false));

            doc.Add(medidas);

            // ===== CLIENTE PROPORCIONA =====
            Table proporciona = new Table(7).UseAllAvailableWidth();

            proporciona.AddCell(CellBox("CLIENTE PROPORCIONA", colSpan: 2, rightBorder: false, subheader: true));
            proporciona.AddCell(Check("Nada", ot.ClienteProporcionanada, leftBorder: false, rightBorder: false, subheader: true));
            proporciona.AddCell(Check("Original", ot.ClienteProporcionaOriginal, leftBorder: false, rightBorder: false, subheader: true));
            proporciona.AddCell(Check("Planchas", ot.ClienteProporcionaPlancha, leftBorder: false, rightBorder: false, subheader: true));
            proporciona.AddCell(Check("Papel", ot.ClienteProporcionaPapel, leftBorder: false, rightBorder: false, subheader: true));
            proporciona.AddCell(Check("Películas", ot.ClienteProporcionaPelicula, leftBorder: false, subheader: true));

            doc.Add(proporciona);

            // ===== IMPRESIÓN =====
            Table impresion = new Table(4).UseAllAvailableWidth();

            impresion.AddCell(CellBox("IMPRESIÓN", rightBorder: false));
            impresion.AddCell(Check("SOLO TIRO", ot.TipoImpresion == "1" ? true : false, rightBorder: false));
            impresion.AddCell(Check("T/R M.P.", ot.TipoImpresion == "2" ? true : false, leftBorder: false, rightBorder: false));
            impresion.AddCell(Check("T/R P.D.", ot.TipoImpresion == "3" ? true : false, leftBorder: false));

            impresion.AddCell(CellBox("MÁQUINA1 ", rightBorder: false));
            impresion.AddCell(CellBox(ot.Maquina1, leftBorder: false, rightBorder: false));
            impresion.AddCell(CellBox("MÁQUINA 2", leftBorder: false, rightBorder: false));
            impresion.AddCell(CellBox(ot.Maquina2, leftBorder: false));

            doc.Add(impresion);

            // ===== DETALLE =====

            Table detalle = new Table(6).UseAllAvailableWidth();
            detalle.AddCell(CellBox("TIPO DE PAPEL", header: true));
            detalle.AddCell(CellBox("CANTIDAD", header: true));
            detalle.AddCell(CellBox("SOBRANTE", header: true));
            detalle.AddCell(CellBox("TOTAL", header: true));
            detalle.AddCell(CellBox("TAMAÑO", header: true));
            detalle.AddCell(CellBox("CANT. PLIEGOS", header: true));
            //detalle.AddCell(CellBox("V° B° IMP"));

            for (int i = 0; i < ot.Detalles.Count; i++)
            {
                detalle.AddCell(CellBox(ot.Detalles[i].TipoPapel));
                detalle.AddCell(CellBox(ot.Detalles[i].Cantidad + ""));
                detalle.AddCell(CellBox(ot.Detalles[i].Sobr + ""));
                detalle.AddCell(CellBox(ot.Detalles[i].Total + ""));
                detalle.AddCell(CellBox(ot.Detalles[i].Tamanio + ""));
                detalle.AddCell(CellBox(ot.Detalles[i].Cplie + ""));

            }

            detalle.AddCell(CellBox(" "));
            detalle.AddCell(CellBox(" "));
            detalle.AddCell(CellBox(" "));
            detalle.AddCell(CellBox(" "));
            detalle.AddCell(CellBox(" "));
            detalle.AddCell(CellBox(" "));

            doc.Add(detalle);

            Table sobre = new Table(12).UseAllAvailableWidth();
            sobre.AddCell(CellBox("SOBRES", rightBorder: false));
            sobre.AddCell(Check("CTE", ot.Sobres == "1" ? true : false, leftBorder: false, rightBorder: false));
            sobre.AddCell(Check("AMERICANO", ot.Sobres == "2" ? true : false, leftBorder: false, rightBorder: false));
            sobre.AddCell(Check("AMERICANO ESPECIAL", ot.Sobres == "3" ? true : false, leftBorder: false, rightBorder: false));
            sobre.AddCell(Check("1/2 OFICIO", ot.Sobres == "4" ? true : false, leftBorder: false, rightBorder: false));
            sobre.AddCell(Check("OFICIO", ot.Sobres == "5" ? true : false, leftBorder: false, rightBorder: false));
            sobre.AddCell(Check("OTRO", ot.Sobres == "6" ? true : false, leftBorder: false));

            doc.Add(sobre);

            Table saco = new Table(12).UseAllAvailableWidth();
            saco.AddCell(CellBox("SACOS", rightBorder: false));
            saco.AddCell(Check("CTE", ot.Sacos == "1" ? true : false, leftBorder: false, rightBorder: false));
            saco.AddCell(Check("AMERICANO", ot.Sacos == "2" ? true : false, leftBorder: false, rightBorder: false));
            saco.AddCell(Check("AMERICANO ESPECIAL", ot.Sacos == "3" ? true : false, leftBorder: false, rightBorder: false));
            saco.AddCell(Check("1/2 OFICIO", ot.Sacos == "4" ? true : false, leftBorder: false, rightBorder: false));
            saco.AddCell(Check("OFICIO", ot.Sacos == "5" ? true : false, leftBorder: false, rightBorder: false));
            saco.AddCell(Check("OTRO", ot.Sacos == "6" ? true : false, leftBorder: false));

            doc.Add(saco);

            Table tinta1 = new Table(4).UseAllAvailableWidth();
            tinta1.AddCell(CellBox("TINTA", rightBorder: false));
            tinta1.AddCell(Check("TRICOMÍA", ot.Tintas1 == "1" ? true : false, leftBorder: false, rightBorder: false));
            tinta1.AddCell(Check("PANTONE", ot.Tintas1 == "2" ? true : false, leftBorder: false, rightBorder: false));
            tinta1.AddCell(Check("S/MUESTRA", ot.Tintas1 == "3" ? true : false, leftBorder: false));

            doc.Add(tinta1);

            Table tinta2 = new Table(4).UseAllAvailableWidth();
            tinta2.AddCell(CellBox("TINTA", rightBorder: false));
            tinta2.AddCell(Check("TRICOMÍA", ot.Tintas2 == "1" ? true : false, leftBorder: false, rightBorder: false));
            tinta2.AddCell(Check("PANTONE", ot.Tintas2 == "2" ? true : false, leftBorder: false, rightBorder: false));
            tinta2.AddCell(Check("S/MUESTRA", ot.Tintas2 == "3" ? true : false, leftBorder: false));

            doc.Add(tinta2);

            Table tinta3 = new Table(4).UseAllAvailableWidth();
            tinta3.AddCell(CellBox("TINTA", rightBorder: false));
            tinta3.AddCell(Check("TRICOMÍA", ot.Tintas3 == "1" ? true : false, leftBorder: false, rightBorder: false));
            tinta3.AddCell(Check("PANTONE", ot.Tintas3 == "2" ? true : false, leftBorder: false, rightBorder: false));
            tinta3.AddCell(Check("S/MUESTRA", ot.Tintas3 == "3" ? true : false, leftBorder: false));

            doc.Add(tinta3);

            Table tinta4 = new Table(4).UseAllAvailableWidth();
            tinta4.AddCell(CellBox("TINTA", rightBorder: false));
            tinta4.AddCell(Check("TRICOMÍA", ot.Tintas4 == "1" ? true : false, leftBorder: false, rightBorder: false));
            tinta4.AddCell(Check("PANTONE", ot.Tintas4 == "2" ? true : false, leftBorder: false, rightBorder: false));
            tinta4.AddCell(Check("S/MUESTRA", ot.Tintas4 == "3" ? true : false, leftBorder: false));

            doc.Add(tinta4);

            Table block = new Table(UnitValue.CreatePercentArray([1, 2, 0.5f, 0.5f, 1, 2, 5])).UseAllAvailableWidth();
            block.AddCell(CellBox(ot.Blockde + "", rightBorder: false));
            block.AddCell(CellBox("BLOCK DE", leftBorder: false, rightBorder: false));
            block.AddCell(CellBox(ot.BlockAncho + "", leftBorder: false, rightBorder: false));
            block.AddCell(CellBox("X", leftBorder: false, rightBorder: false));
            block.AddCell(CellBox(ot.BlockLargo + "", leftBorder: false, rightBorder: false));
            block.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            block.AddCell(CellBox(ot.Observacion1 + "", leftBorder: false));

            doc.Add(block);

            Table talonario = new Table(UnitValue.CreatePercentArray([1, 2, 0.5f, 0.5f, 1, 2, 5])).UseAllAvailableWidth();
            talonario.AddCell(CellBox(ot.Talonariode + "", rightBorder: false));
            talonario.AddCell(CellBox("TALONARIO DE", leftBorder: false, rightBorder: false));
            talonario.AddCell(CellBox(ot.TalonarioAncho + "", leftBorder: false, rightBorder: false));
            talonario.AddCell(CellBox("X", leftBorder: false, rightBorder: false));
            talonario.AddCell(CellBox(ot.TalonarioLargo + "", leftBorder: false, rightBorder: false));
            talonario.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            talonario.AddCell(CellBox(ot.Observacion2 + "", leftBorder: false));

            doc.Add(talonario);

            Table paquete = new Table(UnitValue.CreatePercentArray([1, 2, 1, 1, 5])).UseAllAvailableWidth();
            paquete.AddCell(CellBox(ot.Paquetede + "", rightBorder: false));
            paquete.AddCell(CellBox("PAQUETES DE", leftBorder: false, rightBorder: false));
            paquete.AddCell(CellBox(ot.Paqueteca + "", leftBorder: false, rightBorder: false));
            paquete.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            paquete.AddCell(CellBox(ot.Observacion3 + "", leftBorder: false));

            doc.Add(paquete);

            Table resma = new Table(UnitValue.CreatePercentArray([1, 2, 1, 1, 5])).UseAllAvailableWidth();
            resma.AddCell(CellBox(ot.Resmade + "", rightBorder: false));
            resma.AddCell(CellBox("RESMAS DE", leftBorder: false, rightBorder: false));
            resma.AddCell(CellBox(ot.Resmaca + "", leftBorder: false, rightBorder: false));
            resma.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            resma.AddCell(CellBox(ot.Observacion4 + "", leftBorder: false));

            doc.Add(resma);

            Table fajado = new Table(UnitValue.CreatePercentArray([1, 2, 1, 1, 5])).UseAllAvailableWidth();
            fajado.AddCell(CellBox(ot.Fajadode + "", rightBorder: false));
            fajado.AddCell(CellBox("FAJADOS DE", leftBorder: false, rightBorder: false));
            fajado.AddCell(CellBox(ot.Fajadoca + "", leftBorder: false, rightBorder: false));
            fajado.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            fajado.AddCell(CellBox(ot.Observacion5 + "", leftBorder: false));

            doc.Add(fajado);

            Table fechaT = new Table(UnitValue.CreatePercentArray([2, 2, 1, 5])).UseAllAvailableWidth();
            fechaT.AddCell(CellBox("TRABAJO TERMINADO:", rightBorder: false));
            fechaT.AddCell(CellBox(ot.FechaTermino.ToString("dd/MM/yyyy"), leftBorder: false, rightBorder: false));
            fechaT.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            fechaT.AddCell(CellBox(ot.Observacion6 + "", leftBorder: false));
            doc.Add(fechaT);

            Table fechaE = new Table(UnitValue.CreatePercentArray([2, 2, 1, 5])).UseAllAvailableWidth();
            fechaE.AddCell(CellBox("TRABAJO ENTREGADO:", rightBorder: false));
            fechaE.AddCell(CellBox(ot.FechaEntrega.ToString("dd/MM/yyyy"), leftBorder: false, rightBorder: false));
            fechaE.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            fechaE.AddCell(CellBox(ot.Observacion7 + "", leftBorder: false));
            doc.Add(fechaE);

            Table guia = new Table(UnitValue.CreatePercentArray([3, 1, 0.5f, 1, 1, 6])).UseAllAvailableWidth();
            guia.AddCell(CellBox("GUÍA DE DESPACHO N°", rightBorder: false));
            guia.AddCell(CellBox(ot.GuiaDesde, leftBorder: false, rightBorder: false));
            guia.AddCell(CellBox("AL", leftBorder: false, rightBorder: false));
            guia.AddCell(CellBox(ot.GuiaHasta, leftBorder: false, rightBorder: false));
            guia.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            guia.AddCell(CellBox(ot.Observacion8 + "", leftBorder: false));
            doc.Add(guia);

            Table factura = new Table(UnitValue.CreatePercentArray([3, 1, 0.5f, 1, 1, 6])).UseAllAvailableWidth();
            factura.AddCell(CellBox("FACTURA N°", rightBorder: false));
            factura.AddCell(CellBox(ot.FacturaDesde, leftBorder: false, rightBorder: false));
            factura.AddCell(CellBox("AL", leftBorder: false, rightBorder: false));
            factura.AddCell(CellBox(ot.FacturaHasta, leftBorder: false, rightBorder: false));
            factura.AddCell(CellBox("OBS : ", leftBorder: false, rightBorder: false));
            factura.AddCell(CellBox(ot.Observacion9 + "", leftBorder: false));
            doc.Add(factura);


            // ===== FIRMA =====
            Table firma = new Table(2).UseAllAvailableWidth();
            firma.AddCell(CellBox("FIRMA CLIENTE\n\n\n", true, bottomBorder: true));
            firma.AddCell(CellBox("RUT\n\n\n", true, bottomBorder: true));

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



    static Cell CellBox(
    string text,
    bool bold = false,
    int colSpan = 1,
    bool leftBorder = true,
    bool rightBorder = true,
    bool bottomBorder = false,
    bool header = false,
    bool subheader = false
    )
    {
        var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

        var paragraph = new Paragraph(text)
            .SetFont((bold || header || subheader) ? boldFont : font)
            .SetFontSize(8);

        var cell = new Cell(1, colSpan)
            .Add(paragraph)
            .SetPadding(5)
            .SetMinHeight(12);

        // === HEADER (negro) ===
        if (header)
        {
            paragraph.SetFontColor(ColorConstants.WHITE);
            cell.SetBackgroundColor(ColorConstants.BLACK);
        }
        // === SUBHEADER (plomo) ===
        else if (subheader)
        {
            paragraph.SetFontColor(ColorConstants.BLACK);
            cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
        }

        // Bordes base
        cell.SetBorderTop(new SolidBorder(0.5f));
        cell.SetBorderBottom(bottomBorder ? new SolidBorder(0.5f) : Border.NO_BORDER);

        // Borde izquierdo
        cell.SetBorderLeft(leftBorder ? new SolidBorder(0.5f) : Border.NO_BORDER);

        // Borde derecho
        cell.SetBorderRight(rightBorder ? new SolidBorder(0.5f) : Border.NO_BORDER);

        return cell;
    }
    static Cell Check(string label, bool value, bool leftBorder = true, bool rightBorder = true, bool subheader = false)
    {
        return CellBox($"{(value ? "[X]" : "[ ]")} {label}", leftBorder: leftBorder, rightBorder: rightBorder, subheader: subheader);
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
            .SetBorderTop(new SolidBorder(0.5f))
            .SetBorderLeft(new SolidBorder(0.5f))
            .SetBorderRight(new SolidBorder(0.5f))
            .SetBorderBottom(Border.NO_BORDER);
    }
}