using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestion.presentation.utils
{
    public static class PageUtils
    {
        public static void GridFocus(DataGrid dataGrid, EventHandler handler)
        {
            if (dataGrid.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                if (dataGrid.Items.Count > 0)
                {
                    dataGrid.SelectedIndex = 0;
                    dataGrid.Focus();

                    var firstRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                    if (firstRow != null)
                    {
                        firstRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }

                    dataGrid.ItemContainerGenerator.StatusChanged -= handler;
                }
            }
        }
    }
}