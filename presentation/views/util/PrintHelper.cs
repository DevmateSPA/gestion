using System.IO;
using System.Windows;
using Gestion.core.model;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.util;

public static class PrintHelper
{

    public static void ImprimirOrdenTrabajo(Window owner, OrdenTrabajo orden)
    {
        var errores = ValidationHelper.GetValidationErrors(owner);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }

        string impresora = LocalConfig.ObtenerImpresora();

        if (string.IsNullOrWhiteSpace(impresora) || impresora == "No seleccionada")
        {
            var modal = new ImpresoraModal
            {
                Owner = owner
            };

            if (modal.ShowDialog() != true)
                return; // Usuario canceló

            impresora = modal.ImpresoraSeleccionada;

            if (string.IsNullOrWhiteSpace(impresora))
                return;
        }

        // 3️⃣ Continuar impresión
        string pdfPath = PrintUtils.GenerarOrdenTrabajoPrint(orden);

        string sumatra = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SumatraPDF.exe"
        );

        PrintUtils.PrintFile(pdfPath, impresora, sumatra);
    }
}
