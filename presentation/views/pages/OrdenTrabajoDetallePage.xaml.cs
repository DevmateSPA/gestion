using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util;
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
        Owner = Window.GetWindow(padre);

        _entidadOriginal = entidad;

        // Clonado simple de la entidad
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        DataContext = EntidadEditada;

        Loaded += (_, _) =>
        {
            BindearCamposDinamico();
        };

        // ESC para cerrar
        PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
                DialogResult = false;
        };
    }

    // =========================
    // GUARDAR
    // =========================
    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var errores = ValidationHelper.GetValidationErrors(this);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }

        EntidadEditada.GetType()
            .GetProperty("Empresa")?
            .SetValue(EntidadEditada, SesionApp.IdEmpresa);

        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // =========================
    // IMPRIMIR
    // =========================
    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        var errores = ValidationHelper.GetValidationErrors(this);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }

        GenerarOrdenTrabajoPdf((OrdenTrabajo)EntidadEditada);
        ImprimirPDF();
    }

    // =========================
    // VALIDACIÓN
    // =========================
    private void BindearCamposDinamico()
    {
        if (DataContext == null)
            return;

        var tipo = DataContext.GetType();

        foreach (var ctrl in FindVisualChildren<Control>(this))
        {
            string? propiedad = ctrl.Name switch
            {
                var n when n.StartsWith("txt") => n.Substring(3),
                var n when n.StartsWith("dp")  => n.Substring(2),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(propiedad))
                continue;

            var propInfo = tipo.GetProperty(propiedad);
            if (propInfo == null || !propInfo.CanWrite)
                continue;

            // TextBox
            if (ctrl is TextBox tb)
            {
                var binding = BindingFactory.CreateValidateBinding(propInfo, DataContext);
                tb.SetBinding(TextBox.TextProperty, binding);
            }
            // DatePicker
            else if (ctrl is DatePicker dp)
            {
                var binding = BindingFactory.CreateValidateBinding(
                    propInfo,
                    DataContext,
                    "dd/MM/yyyy"
                );

                dp.SetBinding(DatePicker.SelectedDateProperty, binding);
            }
        }

        // 🔥 Fuerza validación inicial
        ValidationHelper.ForceValidation(this);
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        if (parent == null)
            yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T t)
                yield return t;

            foreach (var sub in FindVisualChildren<T>(child))
                yield return sub;
        }
    }

    // =========================
    // PDF
    // =========================
    public static void GenerarOrdenTrabajoPdf(OrdenTrabajo ot)
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "orden_trabajo.pdf");

        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf);

        PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
        doc.SetFont(font).SetFontSize(10);

        string T(object? v) =>
            string.IsNullOrWhiteSpace(v?.ToString()) ? "---" : v!.ToString()!;

        doc.Add(new Paragraph("IMPRENTA MORIS"));
        doc.Add(new Paragraph("ADMINISTRADOR DE SERVICIOS V1.0"));
        doc.Add(new Paragraph($"Emitido : {DateTime.Now:dd/MM/yyyy}                Pagina : 1"));
        doc.Add(new Paragraph("\nORDEN DE TRABAJO\n===============").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

        doc.Add(new Paragraph(
            $"Folio O.T. [{T(ot.Folio)}]                                          Cliente [{T(ot.RutCliente)}] {T(ot.Descripcion)}"
        ));

        doc.Add(new Paragraph($"Fecha [{ot.Fecha:dd/MM/yyyy}]"));
        doc.Add(new Paragraph(new string('-', 120)));

        doc.Add(new Paragraph($"Trabajo           [{T(ot.Descripcion)}]"));
        doc.Add(new Paragraph($"Cantidad          [{ot.Cantidad}]"));
        doc.Add(new Paragraph($"Total impresiones [{ot.TotalImpresion}]"));
        doc.Add(new Paragraph($"Folio del         [{T(ot.FolioDesde)}]  al [{T(ot.FolioHasta)}]"));
        doc.Add(new Paragraph($"Cortar a tamaño   [{ot.CortarTamanio}]  [{T(ot.CortarTamanioLargo)}] x [{T(ot.CortarTamanion)}] cms"));
        doc.Add(new Paragraph($"Montar            [{ot.Montar}]"));
        doc.Add(new Paragraph($"Molde x tamaño    [{ot.MoldeTamanio}]"));
        doc.Add(new Paragraph($"Tamaño final      [{T(ot.TamanioFinalAncho)}] x [{T(ot.TamanioFinalLargo)}] cms"));

        doc.Add(new Paragraph(new string('-', 120)));

        doc.Add(new Paragraph(
            $"Cliente Proporciona:   Nada [{T(ot.ClienteProporcionanada)}]  Original [{T(ot.ClienteProporcionaOriginal)}]  " +
            $"Películas [{(ot.ClienteProporcionaPelicula ? "X" : " ")}]  Planchas [{(ot.ClienteProporcionaPlancha ? "X" : " ")}]  Papel [{(ot.ClienteProporcionaPapel ? "X" : " ")}]"
        ));

        doc.Add(new Paragraph($"Tipo impresión   [{T(ot.TipoImpresion)}]"));
        doc.Add(new Paragraph($"Máquina 1        [{T(ot.Maquina1)}]"));
        doc.Add(new Paragraph($"Máquina 2        [{T(ot.Maquina2)}]"));
        doc.Add(new Paragraph($"Pinza           [{T(ot.Pin)}]"));
        doc.Add(new Paragraph($"P.Nueva         [{ot.Nva}]   Usada [{ot.Us}]"));
        doc.Add(new Paragraph($"P.Nueva CTP     [{ot.CtpNva}]   Usada CTP [{ot.U}]"));

        doc.Add(new Paragraph(new string('-', 120)));

        doc.Add(new Paragraph($"Tintas:   1[{T(ot.Tintas1)}]   2[{T(ot.Tintas2)}]   3[{T(ot.Tintas3)}]   4[{T(ot.Tintas4)}]"));
        doc.Add(new Paragraph($"Sobres: [{T(ot.Sobres)}]"));
        doc.Add(new Paragraph($"Sacos:   [{T(ot.Sacos)}]"));

        doc.Add(new Paragraph(new string('-', 120)));
        doc.Add(new Paragraph("Películas y Planchas   $____________"));
        doc.Add(new Paragraph("Impresión               $____________"));
    }

    // =========================
    // IMPRESIÓN
    // =========================
    public static void ImprimirPDF(string impresora = "Microsoft Print to PDF")
    {
        try
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "orden_trabajo.pdf");

            string sumatraPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "SumatraPDF.exe");

            if (!File.Exists(sumatraPath))
            {
                MessageBox.Show("No se encontró SumatraPDF.exe.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Process p = new();
            p.StartInfo.FileName = sumatraPath;
            p.StartInfo.Arguments = $"-print-to \"{impresora}\" \"{path}\"";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.Start();
            p.WaitForExit();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al imprimir PDF: " + ex.Message,
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}