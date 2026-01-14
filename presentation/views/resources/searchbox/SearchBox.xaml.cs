using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Gestion.presentation.views.resources.searchbox;

public partial class SearchBox : UserControl
{
    // ========================
    // Datos de prueba
    // ========================
    private readonly List<string> _data =
    [
        "Cliente A",
        "Cliente B",
        "Cliente C",
        "Factura 1001",
        "Factura 1002",
        "Orden 500",
        "Orden 501"
    ];

    public SearchBox()
    {
        InitializeComponent();

        PART_Input.TextChanged += OnTextChanged;
        PART_Input.PreviewKeyDown += OnPreviewKeyDown;
        PART_Input.GotKeyboardFocus += (_, _) => TryShow();
        PART_Input.LostKeyboardFocus += OnInputLostFocus;

        PART_List.MouseLeftButtonUp += (_, _) => CommitSelection();

        PART_List.ItemsSource = _data;
    }

    // ========================
    // Dependency Properties
    // ========================

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(SearchBox),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(SearchBox),
            new PropertyMetadata(null));

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty DisplayMemberProperty =
        DependencyProperty.Register(
            nameof(DisplayMember),
            typeof(string),
            typeof(SearchBox),
            new PropertyMetadata(string.Empty));

    public string DisplayMember
    {
        get => (string)GetValue(DisplayMemberProperty);
        set => SetValue(DisplayMemberProperty, value);
    }

    // ========================
    // Text Changed
    // ========================
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TryShow();
    }

    private void TryShow()
    {
        var text = PART_Input.Text;

        if (string.IsNullOrWhiteSpace(text))
        {
            Hide();
            return;
        }

        var source = ItemsSource ?? _data;

        var filtered = source
            .Cast<object>()
            .Select(x => GetItemText(x))
            .Where(s =>
                s.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Select(s => new HighlightItem(s, text))
            .ToList();

        // Si ya es match exacto único → cerrar
        if (filtered.Count == 1 &&
            string.Equals(
                GetItemText(filtered[0]),
                text,
                StringComparison.OrdinalIgnoreCase))
        {
            Hide();
            return;
        }

        PART_List.ItemsSource = filtered;

        if (filtered.Count > 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    // ========================
    // TECLADO (LA CLAVE)
    // ========================
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!PART_Popup.IsOpen || PART_List.Items.Count == 0)
            return;

        switch (e.Key)
        {
            case Key.Down:
                if (PART_List.SelectedIndex < PART_List.Items.Count - 1)
                    PART_List.SelectedIndex++;
                e.Handled = true;
                break;

            case Key.Up:
                if (PART_List.SelectedIndex > 0)
                    PART_List.SelectedIndex--;
                e.Handled = true;
                break;

            case Key.Enter:
                CommitSelection();
                e.Handled = true;
                break;

            case Key.Escape:
                Hide();
                e.Handled = true;
                break;
        }

        PART_List.ScrollIntoView(PART_List.SelectedItem);
    }

    // ========================
    // Focus Handling
    // ========================
    private void OnInputLostFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (PART_Popup.IsKeyboardFocusWithin)
            return;

        Hide();
    }

    // ========================
    // Selection
    // ========================
    private void CommitSelection()
    {
        if (PART_List.SelectedItem == null)
            return;

        if (PART_List.SelectedItem is HighlightItem item)
        {
            Text = item.Original;
        }

        Hide();
        PART_Input.CaretIndex = Text.Length;
    }

    private string GetItemText(object item)
    {
        if (item == null)
            return string.Empty;

        if (string.IsNullOrEmpty(DisplayMember))
            return item.ToString() ?? string.Empty;

        return item.GetType()
                   .GetProperty(DisplayMember)?
                   .GetValue(item)?
                   .ToString()
               ?? string.Empty;
    }

    // ========================
    // Popup helpers
    // ========================
    private void Show()
    {
        PART_Popup.IsOpen = true;

        if (PART_List.SelectedIndex < 0 && PART_List.Items.Count > 0)
            PART_List.SelectedIndex = 0;
    }

    private void Hide()
    {
        PART_Popup.IsOpen = false;
    }
}