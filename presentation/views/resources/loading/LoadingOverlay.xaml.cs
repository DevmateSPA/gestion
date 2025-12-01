namespace Gestion.presentation.views.resources.loading;

using System.Windows;
using System.Windows.Controls;

public partial class LoadingOverlay : UserControl
{
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingOverlay), new PropertyMetadata(false));

    public LoadingOverlay()
    {
        InitializeComponent();
    }
}