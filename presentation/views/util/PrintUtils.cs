using System.Diagnostics;
using System.IO;
using System.Windows;
using Gestion.core.model;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
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

        public static string GenerarOrdenTrabajoPrint(OrdenTrabajo ot)
        {
            return GeneratePdf("orden_trabajo_"+ot.Folio+".pdf", doc =>
            {
                var font = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
                doc.SetFont(font).SetFontSize(10);

                string T(object? v) =>
                    string.IsNullOrWhiteSpace(v?.ToString()) ? "---" : v!.ToString();

                doc.Add(new Paragraph("IMPRENTA MORIS").SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Left));
                doc.Add(new Paragraph("ADMINISTRADOR DE SERVICIOS V1.0"));
                doc.Add(new Paragraph($"Emitido : {DateTime.Now:dd/MM/yyyy}                Pagina : 1"));
                doc.Add(new Paragraph("\nORDEN DE TRABAJO\n===============")
                    .SetTextAlignment((iText.Layout.Properties.TextAlignment?)TextAlignment.Center));

                doc.Add(new Paragraph(
                    $"Folio O.T. [{T(ot.Folio)}]                                          Cliente [{T(ot.RutCliente)}] {T(ot.Descripcion)}"
                ));

                doc.Add(new Paragraph($"Fecha [{ot.Fecha:dd/MM/yyyy}]"));
                doc.Add(new Paragraph(new string('-', 120)));

                // SECCIÓN TRABAJO
                doc.Add(new Paragraph($"Trabajo           [{T(ot.Descripcion)}]"));
                doc.Add(new Paragraph($"Cantidad          [{ot.Cantidad}]"));
                doc.Add(new Paragraph($"Total impresiones [{ot.TotalImpresion}]"));
                doc.Add(new Paragraph($"Folio del         [{T(ot.FolioDesde)}]  al [{T(ot.FolioHasta)}]"));
                doc.Add(new Paragraph($"Cortar a tamaño   [{ot.CortarTamanio}]  [{T(ot.CortarTamanioLargo)}] x [{T(ot.CortarTamanion)}] cms"));
                doc.Add(new Paragraph($"Montar            [{ot.Montar}]"));
                doc.Add(new Paragraph($"Molde x tamaño    [{ot.MoldeTamanio}]"));
                doc.Add(new Paragraph($"Tamaño final      [{T(ot.TamanioFinalAncho)}] x [{T(ot.TamanioFinalLargo)}] cms"));

                doc.Add(new Paragraph(new string('-', 120)));

                // CLIENTE PROPORCIONA
                doc.Add(new Paragraph(
                    $"Cliente Proporciona:   Nada [{T(ot.ClienteProporcionanada)}]  Original [{T(ot.ClienteProporcionaOriginal)}]  Películas [{(ot.ClienteProporcionaPelicula ? "X" : " ")}]  Planchas [{(ot.ClienteProporcionaPlancha ? "X" : " ")}]  Papel [{(ot.ClienteProporcionaPapel ? "X" : " ")}]"
                ));

                // IMPRESIÓN Y MÁQUINAS
                doc.Add(new Paragraph($"Tipo impresión   [{T(ot.TipoImpresion)}]"));
                doc.Add(new Paragraph($"Máquina 1        [{T(ot.Maquina1)}]"));
                doc.Add(new Paragraph($"Máquina 2        [{T(ot.Maquina2)}]"));
                doc.Add(new Paragraph($"Pinza           [{T(ot.Pin)}]"));
                doc.Add(new Paragraph($"P.Nueva         [{ot.Nva}]   Usada [{ot.Us}]"));
                doc.Add(new Paragraph($"P.Nueva CTP     [{ot.CtpNva}]   Usada CTP [{ot.U}]"));

                doc.Add(new Paragraph(new string('-', 120)));

                // TINTAS
                doc.Add(new Paragraph($"Tintas:   1[{T(ot.Tintas1)}]   2[{T(ot.Tintas2)}]   3[{T(ot.Tintas3)}]   4[{T(ot.Tintas4)}]"));

                // MATERIALES
                doc.Add(new Paragraph($"Sobres: [{T(ot.Sobres)}]"));
                doc.Add(new Paragraph($"Sacos:   [{T(ot.Sacos)}]"));

                doc.Add(new Paragraph(new string('-', 120)));

                doc.Add(new Paragraph("Películas y Planchas   $____________"));
                doc.Add(new Paragraph("Impresión               $____________"));
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
                        Pad(ot.Razon_social ?? "", 12) +
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
    }
}