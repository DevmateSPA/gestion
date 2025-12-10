using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.core.session;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoDetallePage : Window
{
    private readonly object _entidadOriginal;

    public object EntidadEditada { get; private set; }

    public OrdenTrabajoDetallePage(Page padre, object entidad)
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);

        _entidadOriginal = entidad;

        // Crear copia profunda simple
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        // Esc para cerrar
        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
                this.DialogResult = false;
        };

        DataContext = EntidadEditada;
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        EntidadEditada.GetType()
            .GetProperty("Empresa")?
            .SetValue(EntidadEditada, SesionApp.IdEmpresa);

        this.DialogResult = true;
    }

    

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }

    public static void GenerarOrdenTrabajoPdf(OrdenTrabajo ot, string outputPath)
    {
        var writer = new PdfWriter(outputPath);
        var pdf = new PdfDocument(writer);
        var doc = new Document(pdf);

        PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);

        doc.SetFont(font).SetFontSize(10);

        // Función para evitar nulls
        string T(object? v) => string.IsNullOrWhiteSpace(v?.ToString()) ? "---" : v.ToString();

        // CABECERA
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
        doc.Add(new Paragraph($"Cliente Proporciona:   Nada [{T(ot.ClienteProporcionanada)}]  Original [{T(ot.ClienteProporcionaOriginal)}]  Películas [{(ot.ClienteProporcionaPelicula ? "X" : " ")}]  Planchas [{(ot.ClienteProporcionaPlancha ? "X" : " ")}]  Papel [{(ot.ClienteProporcionaPapel ? "X" : " ")}]"));

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

        // PIE FINAL
        doc.Add(new Paragraph("Películas y Planchas   $____________"));
        doc.Add(new Paragraph("Impresión               $____________"));

        doc.Close();
    }
}