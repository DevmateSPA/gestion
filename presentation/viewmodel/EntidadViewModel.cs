using System.Collections.ObjectModel;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.service;
using Gestion.helpers;
using System.Reflection;
using Gestion.core.session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Gestion.presentation.viewmodel;

public abstract class EntidadViewModel<T> : INotifyPropertyChanged where T : IEmpresa
{
    protected readonly IDialogService _dialogService;
    protected readonly IBaseService<T> _service;
    protected readonly string _emptyMessage;
    protected readonly string _errorMessage;

    private ObservableCollection<T> _entidades = [];
    public ObservableCollection<T> Entidades
    {
        get => _entidades;
        private set
        {
            _entidades = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EntidadesView));
        }
    }
    public ICollectionView EntidadesView => CollectionViewSource.GetDefaultView(Entidades);
    private string _filtro = "";
    public string Filtro
    {
        get => _filtro;
        set { _filtro = value; OnPropertyChanged(); }
    }

    private readonly Dictionary<T, string> _searchCache = new();

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

    private bool _isBusy;
    
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
    private int _pageSize = 50;
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

    protected EntidadViewModel(
        IBaseService<T> baseService, 
        IDialogService dialogService, 
        string? emptyMessage = null, 
        string? errorMessage = null)
    {
        _dialogService = dialogService;
        _service = baseService;
        _emptyMessage = emptyMessage ?? $"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}";
        _errorMessage = errorMessage ?? $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}";
    }

    private readonly PropertyInfo[] _stringProps =
        typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

    private string BuildSearchText(T entidad)
    {
        return string.Join(" ",
            _stringProps
                .Select(p => p.GetValue(entidad) as string)
                .Where(s => !string.IsNullOrWhiteSpace(s))
        ).ToLowerInvariant();
    }

    public virtual void Buscar(string filtro)
    {
        if (string.IsNullOrWhiteSpace(filtro))
        {
            EntidadesView.Filter = null;
            return;
        }

        var lower = filtro.ToLowerInvariant();

        EntidadesView.Filter = item =>
        {
            if (item is not T entidad)
                return false;

            return _searchCache.TryGetValue(entidad, out var text)
                && text.Contains(lower);
        };
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
        if (_isBusy)
            return;

        _isBusy = true;
        IsLoading = true;

        try
        {
            var lista = await SafeExecutor.RunAsyncList(
                action,
                _dialogService,
                errorMessage);

            if (lista.Count == 0)
                onEmpty?.Invoke();

            LoadEntitiesIntoCollection(lista);
            onSuccess?.Invoke();
        }
        finally
        {
            IsLoading = false;
            _isBusy = false;
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
        _searchCache.Clear();

        foreach (var entidad in lista)
        {
            _searchCache[entidad] = BuildSearchText(entidad);
            Entidades.Add(entidad);
        }
    }

    public virtual async Task LoadAll()
    {
        await RunWithLoading(
            action: async () => await _service.FindAll(),
            errorMessage: _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
    }

    public virtual async Task LoadAllByEmpresa()
    {
        await RunWithLoading(
            action: async () => await _service.FindAllByEmpresa(SesionApp.IdEmpresa),
            errorMessage: _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
    }

    protected async Task LoadPagedEntities(
        Func<int, Task<List<T>>> serviceCall,
        int page,
        string emptyMessage,
        string errorMessage,
        Func<Task<long>>? totalCountCall = null,
        Func<Task<List<T>>>? allItemsCall = null)
    {
        if (PageSize == 0)
        {
            await RunWithLoading(
                action: async () =>
                {
                    if (allItemsCall != null)
                        return await allItemsCall();
                    else
                        return await _service.FindAllByEmpresa(SesionApp.IdEmpresa);
                },
                errorMessage: errorMessage,
                onEmpty: () => _dialogService.ShowMessage(emptyMessage)
            );

            PageNumber = 1;
            TotalRegistros = 1;
            return;
        }

        // Validación defensiva
        if (page < 1)
            page = 1;

        PageNumber = page;

        long total = totalCountCall != null
            ? await totalCountCall()
            : await _service.ContarPorEmpresa(SesionApp.IdEmpresa);

        TotalRegistros = Math.Max(1,
            (int)Math.Ceiling(total / (double)PageSize));

        await RunWithLoading(
            action: () => serviceCall(PageNumber),
            errorMessage: errorMessage,
            onEmpty: () => _dialogService.ShowMessage(emptyMessage));
    }

    public virtual async Task LoadPageByEmpresa(int page)
    {
        await LoadPagedEntities(
            serviceCall: async (p) => await _service.FindPageByEmpresa(SesionApp.IdEmpresa, p, PageSize),
            page: page,
            emptyMessage: _emptyMessage,
            errorMessage: _errorMessage);
    }
    private protected async Task<bool> RunServiceAction(
        Func<Task<bool>> serviceAction,
        Action? onSuccess,
        string mensajeError)
    {
        return await SafeExecutor.RunAsyncValue(
        action: async () =>
        {
            if (await serviceAction())
            {
                onSuccess?.Invoke();
                return true;
            }

            return false;
        }, dialogService: _dialogService, 
        mensajeError: mensajeError);
    }
    public virtual async Task Delete(long id) =>
        await RunServiceAction(
            serviceAction: async () => await _service.DeleteById(id), 
            onSuccess: () => RemoveEntityById(id), 
            mensajeError: $"Error al eliminar {typeof(T).Name}");

    public async Task<bool> Update(T entidad) =>
        await RunServiceAction(
        serviceAction: async () => await _service.Update(entidad), 
        onSuccess: () => ReplaceEntity(entidad), 
        mensajeError: $"Error al actualizar {typeof(T).Name}");

    public async Task<bool> Save(T entidad)
    {
        entidad.Empresa = SesionApp.IdEmpresa;

        return await RunServiceAction(
        serviceAction: async () => await _service.Save(entidad), 
        onSuccess: () => AddEntity(entidad), 
        mensajeError: $"Error al guardar {typeof(T).Name}");
    }
}
