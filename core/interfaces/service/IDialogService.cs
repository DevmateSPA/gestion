namespace Gestion.core.interfaces.service;

public interface IDialogService
{
    void ShowMessage(string message, string title = "Información");
    void ShowError(string message, string title = "Error");
}