using System.Collections.ObjectModel;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.service;
using Gestion.helpers;
using System.Windows;
using System.IO;
using System.Reflection;
using Gestion.presentation.utils;
using Gestion.core.session;
using Org.BouncyCastle.Asn1.X509.Qualified;

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

    private protected void removeEntityById(long id)
    {
        var entidad = Entidades.FirstOrDefault(e => e.Id == id);
        if (entidad != null)
            Entidades.Remove(entidad);
    }

    private protected void replaceEntity(T entidad)
    {
        var existing = Entidades.FirstOrDefault(e => e.Id == entidad.Id);
        if (existing != null)
        {
            var index = Entidades.IndexOf(existing);
            if (index >= 0)
                Entidades[index] = entidad;
        }
    }

    private protected void addEntity(T entidad)
    {
        if (entidad != null)
            Entidades.Add(entidad);
    }

    public virtual async Task LoadAll()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _service.FindAll();
            var dateProp = GetDateProperty(typeof(T));
            if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }
            Entidades.Clear();
            foreach (var entidad in lista)
                addEntity(entidad);
        }, _dialogService, $"Error al cargar {typeof(T).Name}");
    }

    public virtual async Task LoadAllByEmpresa()
    {
        await SafeExecutor.RunAsync(async () =>
        {
           var lista = await _service.FindAllByEmpresa(SesionApp.IdEmpresa);
            if (!lista.Any())
                _dialogService.ShowMessage($"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}");

           var dateProp = GetDateProperty(typeof(T));
           if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }
            Entidades.Clear();
            foreach(var entidad in lista)
                addEntity(entidad);
        }, _dialogService, $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}");
    }

    public static PropertyInfo? GetDateProperty(Type t)
    {
        return t.GetProperty("Fecha", 
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }
    private protected async Task RunServiceAction(Func<Task<bool>> serviceAction, Action onSuccess, string mensajeError)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await serviceAction())
                onSuccess();
        }, _dialogService, mensajeError);
    }
    public Task Delete(long id) =>
        RunServiceAction(() => _service.DeleteById(id), () => removeEntityById(id), $"Error al eliminar {typeof(T).Name}");

    public Task Update(T entidad) =>
        RunServiceAction(() => _service.Update(entidad), () => replaceEntity(entidad), $"Error al actualizar {typeof(T).Name}");

    public Task Save(T entidad) =>
        RunServiceAction(() => _service.Save(entidad), () => addEntity(entidad), $"Error al guardar {typeof(T).Name}");
}
