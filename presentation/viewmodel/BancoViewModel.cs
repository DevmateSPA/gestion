using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class BancoViewModel
{
    private readonly IBancoService _bancoService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Banco> Bancos { get; set; } = new ObservableCollection<Banco>();
    public BancoViewModel(IBancoService bancoService, IDialogService dialogService)
    {
        _bancoService = bancoService;
        _dialogService = dialogService;
    }

    public async Task delete(int id)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await _bancoService.DeleteById(id))
            {
                var bancoAEliminar = Bancos.FirstOrDefault(b => b.Id == id);
                if (bancoAEliminar != null)
                    Bancos.Remove(bancoAEliminar);
            }
        }, _dialogService, "Error al eliminar el banco");
    }

    public async Task update(Banco banco)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await _bancoService.Update(banco))
            {
                var existing = Bancos.FirstOrDefault(b => b.Id == banco.Id);
                if (existing != null)
                {
                    var index = Bancos.IndexOf(existing);
                    if (index >= 0)
                        Bancos[index] = banco;
                }
            }
        }, _dialogService, "Error al actualizar el banco");
    }

    public async Task LoadBancos()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _bancoService.FindAll();
            Bancos.Clear();
            foreach (var banco in lista)
                Bancos.Add(banco);
        }, _dialogService, "Error al cargar los bancos");
    }
}
