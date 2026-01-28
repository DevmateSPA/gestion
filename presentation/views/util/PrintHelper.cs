using System.IO;
using System.Windows;
using Gestion.core.model;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.util;

public static class PrintHelper
{

    private static string pdfPath = "";

    public async static void ImprimirOrdenTrabajo(Window owner, string pdfPath)
    {

        string impresora = LocalConfig.ObtenerImpresora();

        if (string.IsNullOrWhiteSpace(impresora) || impresora == "")
        {
            var modal = new ImpresoraModal
            {
                Owner = owner
            };

            if (modal.ShowDialog() != true)
                return; 

            impresora = modal.ImpresoraSeleccionada;

            if (string.IsNullOrWhiteSpace(impresora))
                return;
        }

        await Task.Run(() =>
        {

            string sumatra = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "SumatraPDF.exe"
            );

            PrintUtils.PrintFile(pdfPath, impresora, sumatra);
        });

        MessageBox.Show(owner, "Impresión completada", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public async static void PrevisualizarOrdenTrabajo(Window owner, OrdenTrabajo orden)
    {
        var errores = ValidationHelper.GetValidationErrors(owner);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }

        string pdfPath = await Task.Run(() =>
            PrintUtils.GenerarOrdenTrabajoPrint(orden)
        );

        var preview = new PdfPreviewWindow(pdfPath)
        {
            Owner = owner
        };

        preview.ShowDialog();
    }
}
