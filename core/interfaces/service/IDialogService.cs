namespace Gestion.core.interfaces.service;

public interface IDialogService
{
    void ShowMessage(string message, string title = "Información");
    void ShowError(string message, string title = "Error");
    void ShowWarning(string message, string title = "Advertencia");
    bool Confirm(string message, string title = "Confirmar acción");
}