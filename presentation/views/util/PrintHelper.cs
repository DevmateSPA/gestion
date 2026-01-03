using System.IO;
using System.Windows;
using Gestion.core.model;
using Gestion.presentation.utils;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.util;

public static class PrintHelper
{
    public static void ImprimirOrdenTrabajo(Window owner, OrdenTrabajo orden)
    {
        if (orden == null)
            throw new ArgumentNullException(nameof(orden));

        var modal = new ImpresoraModal
        {
            Owner = owner
        };

        if (modal.ShowDialog() != true)
            return;

        string impresora = modal.ImpresoraSeleccionada;

        string pdfPath = PrintUtils.GenerarOrdenTrabajoPrint(orden);

        string sumatra = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SumatraPDF.exe");

        PrintUtils.PrintFile(pdfPath, impresora, sumatra);
    }
}
