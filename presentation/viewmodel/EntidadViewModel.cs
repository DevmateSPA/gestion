using System.Collections.ObjectModel;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.service;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public abstract class EntidadViewModel<T> where T : IModel
{
    protected readonly IDialogService _dialogService;
    protected readonly IBaseService<T> _service;

    public ObservableCollection<T> Entidades { get; } = new();

    protected EntidadViewModel(IBaseService<T> baseService, IDialogService dialogService)
    {
        _dialogService = dialogService;
        _service = baseService;
    }

    public async Task LoadAll()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _service.FindAll();
            Entidades.Clear();
            foreach (var entidad in lista)
                Entidades.Add(entidad);
        }, _dialogService, $"Error al cargar {typeof(T).Name}");
    }

    public async Task Delete(int id)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await _service.DeleteById(id))
            {
                var entidadAEliminar = Entidades.FirstOrDefault(e => e.Id == id);
                if (entidadAEliminar != null)
                    Entidades.Remove(entidadAEliminar);
            }
        }, _dialogService, $"Error al eliminar {typeof(T).Name}");
    }

    public async Task Update(T entidad)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await _service.Update(entidad))
            {
                var existing = Entidades.FirstOrDefault(e => e.Id == entidad.Id);
                if (existing != null)
                {
                    var index = Entidades.IndexOf(existing);
                    if (index >= 0)
                        Entidades[index] = entidad;
                }
            }
        }, _dialogService, $"Error al actualizar {typeof(T).Name}");
    }

    public async Task Save(T entidad)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var entidadGuardada = await _service.Save(entidad);
            if (entidadGuardada)
                Entidades.Add(entidad);
        }, _dialogService, $"Error al guardar {typeof(T).Name}");
    }
}
