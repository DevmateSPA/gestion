using Gestion.presentation.views.util;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows;

namespace Gestion.presentation.views.windows
{
    public partial class PdfPreviewWindow : Window
    {
        private readonly string _pdfPath;

        public PdfPreviewWindow(string pdfPath)
        {
            InitializeComponent();
            _pdfPath = pdfPath;
            Loaded += PdfPreviewWindow_Loaded;
        }

        private async void PdfPreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await PdfViewer.EnsureCoreWebView2Async();

            // Cargar PDF local
            PdfViewer.Source = new Uri(_pdfPath);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintHelper.ImprimirOrdenTrabajo(this, _pdfPath);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
