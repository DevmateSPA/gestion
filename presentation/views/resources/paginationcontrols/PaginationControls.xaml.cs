using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestion.presentation.views.resources.paginationcontrols;

public partial class PaginationControls : UserControl
{
    public event Action<int> PageChanged;

    public int CurrentPage { get; private set; } = 1;
    public int TotalPages { get; private set; } = 1;

    public PaginationControls()
    {
        InitializeComponent();
        UpdateUI();
    }

    public void SetTotalPages(int total)
    {
        TotalPages = Math.Max(total, 1);
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        UpdateUI();
    }

    private void UpdateUI()
    {
        txtPage.Text = CurrentPage.ToString();
        lblTotalPages.Text = TotalPages.ToString();
    }

    private void NavigateTo(int page)
    {
        if (page < 1 || page > TotalPages)
            return;

        CurrentPage = page;
        UpdateUI();
        PageChanged?.Invoke(CurrentPage);
    }

    private void FirstPage_Click(object sender, RoutedEventArgs e) => NavigateTo(1);
    private void PrevPage_Click(object sender, RoutedEventArgs e) => NavigateTo(CurrentPage - 1);
    private void NextPage_Click(object sender, RoutedEventArgs e) => NavigateTo(CurrentPage + 1);
    private void LastPage_Click(object sender, RoutedEventArgs e) => NavigateTo(TotalPages);

    private void txtPage_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (int.TryParse(txtPage.Text, out int p))
                NavigateTo(p);

            e.Handled = true;
        }
    }
}
