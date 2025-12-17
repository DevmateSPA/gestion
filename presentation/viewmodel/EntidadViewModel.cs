using System.Collections.ObjectModel;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.service;
using Gestion.helpers;
using System.Reflection;
using Gestion.core.session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public abstract class EntidadViewModel<T> : INotifyPropertyChanged where T : IEmpresa
{
    protected readonly IDialogService _dialogService;
    protected readonly IBaseService<T> _service;

    public ObservableCollection<T> Entidades { get; set; } = new ObservableCollection<T>();
    public ICollectionView EntidadesView { get; }
    private string _filtro = "";
    public string Filtro
    {
        get => _filtro;
        set { _filtro = value; OnPropertyChanged(); }
    }
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Atributos para paginacion
    private int _pageNumber = 1;
    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            _pageNumber = value;
            OnPropertyChanged(nameof(PageNumber));
        }
    }
    private int _pageSize = 29;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value;
            OnPropertyChanged(nameof(PageSize));
        }
    }

    private int _totalRegistros;
    public int TotalRegistros
    {
        get => _totalRegistros;
        set
        {
            _totalRegistros = value;
            OnPropertyChanged(nameof(TotalRegistros));
        }
    }

    // Constructor

    protected EntidadViewModel(IBaseService<T> baseService, IDialogService dialogService)
    {
        _dialogService = dialogService;
        _service = baseService;
        EntidadesView = CollectionViewSource.GetDefaultView(Entidades);
    }

    private readonly PropertyInfo[] _stringProps =
        typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

    public virtual void Buscar(string filtro)
    {
        if (string.IsNullOrWhiteSpace(filtro))
        {
            EntidadesView.Filter = null;
            EntidadesView.Refresh();
            return;
        }

        var lower = filtro.ToLower();

        EntidadesView.Filter = item =>
        {
            if (item is not T entidad) 
                return false;

            foreach (var prop in _stringProps)
            {
                var value = prop.GetValue(entidad) as string;
                if (value?.ToLower().Contains(lower) == true)
                    return true;
            }

            return false;
        };

        EntidadesView.Refresh();
    }

    private protected void RemoveEntityById(long id)
    {
        var entidad = Entidades.FirstOrDefault(e => e.Id == id);
        if (entidad != null)
            Entidades.Remove(entidad);
    }

    private protected void ReplaceEntity(T entidad)
    {
        var existing = Entidades.FirstOrDefault(e => e.Id == entidad.Id);
        if (existing != null)
        {
            var index = Entidades.IndexOf(existing);
            if (index >= 0)
                Entidades[index] = entidad;
        }
    }

    private protected void AddEntity(T entidad)
    {
        if (entidad != null)
            Entidades.Insert(0, entidad);
    }

    protected async Task RunWithLoading(
        Func<Task<List<T>>> action,
        string errorMessage,
        Action? onSuccess = null,
        Action? onEmpty = null)
    {
        this.IsLoading = true;
        try
        {
            // Variante con retorno
            var lista = await SafeExecutor.RunAsync(action, _dialogService, errorMessage);

            if (lista.Count == 0)
                onEmpty?.Invoke();

            LoadEntitiesIntoCollection(lista);
            onSuccess?.Invoke();
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    protected static PropertyInfo? GetDateProperty(Type t)
    {
        return t.GetProperty("Fecha", 
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }

    protected void LoadEntitiesIntoCollection(List<T> lista)
    {
        var dateProp = GetDateProperty(typeof(T));
        if (dateProp != null)
            lista = [.. lista.OrderByDescending(x => dateProp.GetValue(x))];

        Entidades.Clear();
        foreach (var entidad in lista)
            AddEntity(entidad);
    }

    public virtual async Task LoadAll()
    {
        await RunWithLoading(
            action: async () => await _service.FindAll(),
            errorMessage: $"Error al cargar {typeof(T).Name}",
            onEmpty: () => _dialogService.ShowMessage($"No hay {typeof(T).Name} cargadas."));
    }

    public virtual async Task LoadAllByEmpresa()
    {
        await RunWithLoading(
            action: async () => await _service.FindAllByEmpresa(SesionApp.IdEmpresa),
            errorMessage: $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}",
            onEmpty: () => _dialogService.ShowMessage($"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}"));
    }

    protected async Task LoadPagedEntities(
        Func<int, Task<List<T>>> serviceCall,
        int page,
        string emptyMessage,
        string errorMessage,
        Func<Task<List<T>>>? allItemsCall = null)
    {
        if (PageSize == 0)
        {
            if (allItemsCall == null)
                await LoadAllByEmpresa();
            else
                await allItemsCall.Invoke();

            PageNumber = 1;
            TotalRegistros = 1;
            return;
        }

        PageNumber = page;

        long total = await _service.ContarPorEmpresa(SesionApp.IdEmpresa);
        TotalRegistros = (int)Math.Ceiling(total / (double)PageSize);

        await RunWithLoading(
            action: () => serviceCall(PageNumber),
            errorMessage: errorMessage,
            onEmpty: () => _dialogService.ShowMessage(emptyMessage));
    }

    public virtual async Task LoadPageByEmpresa(int page)
    {
        await LoadPagedEntities(
            serviceCall: (p) => _service.FindPageByEmpresa(SesionApp.IdEmpresa, p, PageSize),
            page: page,
            emptyMessage: $"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}",
            errorMessage: $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}");
    }
    private protected async Task RunServiceAction(Func<Task<bool>> serviceAction, Action? onSuccess, string mensajeError)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await serviceAction())
                onSuccess?.Invoke();
        }, _dialogService, mensajeError);
    }
    public virtual async Task Delete(long id) =>
        await RunServiceAction(
            serviceAction: async () => await _service.DeleteById(id), 
            onSuccess: () => RemoveEntityById(id), 
            mensajeError: $"Error al eliminar {typeof(T).Name}");

    public async Task Update(T entidad) =>
        await RunServiceAction(
        serviceAction: async () => await _service.Update(entidad), 
        onSuccess: () => ReplaceEntity(entidad), 
        mensajeError: $"Error al actualizar {typeof(T).Name}");

    public async Task Save(T entidad)
    {
        entidad.Empresa = SesionApp.IdEmpresa;
        await RunServiceAction(serviceAction: async () => await _service.Save(entidad), 
        onSuccess: () => AddEntity(entidad), 
        mensajeError: $"Error al guardar {typeof(T).Name}");
    }
}
