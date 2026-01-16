using System.Windows;

namespace Gestion.presentation.views.windows;

public partial class ToastWindow : Window
{
    public string Message { get; }

    public ToastWindow(string message)
    {
        InitializeComponent();
        Message = message;
        DataContext = this;
    }

    public void PositionOver(Window window)
    {
        if (window == null) return;

        Dispatcher.BeginInvoke(() =>
        {
            Left = window.Left + (window.ActualWidth - ActualWidth) / 2;
            Top  = window.Top  + (window.ActualHeight - ActualHeight) / 2;
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    public void AutoClose(int timer)
    {
        _ = CloseLater(timer);
    }

    private async Task CloseLater(int timer)
    {
        await Task.Delay(timer);
        Close();
    }
}