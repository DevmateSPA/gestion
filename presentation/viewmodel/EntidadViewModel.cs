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
    private int _pageSize = 20;
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
            Entidades.Add(entidad);
    }

    public virtual async Task LoadAll()
    {
        this.IsLoading = true;
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _service.FindAll();
            if (lista.Count == 0)
                _dialogService.ShowMessage($"No hay {typeof(T).Name} cargadas.");
            var dateProp = GetDateProperty(typeof(T));
            if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }
            Entidades.Clear();
            foreach (var entidad in lista)
                AddEntity(entidad);
        }, _dialogService, $"Error al cargar {typeof(T).Name}");
        this.IsLoading = false;
    }

    public virtual async Task LoadAllByEmpresa()
    {
        this.IsLoading = true;
        await SafeExecutor.RunAsync(async () =>
        {
           var lista = await _service.FindAllByEmpresa(SesionApp.IdEmpresa);
            if (lista.Count == 0)
                _dialogService.ShowMessage($"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}");

           var dateProp = GetDateProperty(typeof(T));
           if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }
            Entidades.Clear();
            foreach(var entidad in lista)
                AddEntity(entidad);
        }, _dialogService, $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}");
        this.IsLoading = false;
    }

    public virtual async Task LoadAllByEntrega()
    {
        this.IsLoading = true;
        await SafeExecutor.RunAsync(async () =>
        {
            var p = new MySqlParameter("@empresa", SesionApp.IdEmpresa);
            var where = "empresa = @empresa AND fechaentrega IS NULL";
            var lista = await _service.FindAllByParam("vw_ordentrabajo",p,where);
            if (!lista.Any())
                _dialogService.ShowMessage($"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}");

           var dateProp = GetDateProperty(typeof(T));
           if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }
            Entidades.Clear();
            foreach(var entidad in lista)
                AddEntity(entidad);
        }, _dialogService, $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}");
        this.IsLoading = false;
    }

    public virtual async Task LoadPageByEmpresa(int page)
    {
        if (PageSize == 0)
        {
            await LoadAllByEmpresa();
            PageNumber = 1;
            TotalRegistros = 1; // solo 1 página
            return;
        }

        PageNumber = page;

        long total = await _service.ContarPorEmpresa(SesionApp.IdEmpresa);
        TotalRegistros = (int)Math.Ceiling(total / (double)PageSize);

        this.IsLoading = true;

        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _service.FindPageByEmpresa(SesionApp.IdEmpresa, PageNumber, PageSize);

            var dateProp = GetDateProperty(typeof(T));
            if (dateProp != null)
                lista = [.. lista.OrderByDescending(x => dateProp.GetValue(x))];

            Entidades.Clear();
            foreach (var entidad in lista)
                AddEntity(entidad);

        }, _dialogService, $"Error al cargar {typeof(T).Name}");
        this.IsLoading = false;
    }

    public static PropertyInfo? GetDateProperty(Type t)
    {
        return t.GetProperty("Fecha", 
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
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
        await RunServiceAction(async () => await _service.DeleteById(id), () => RemoveEntityById(id), $"Error al eliminar {typeof(T).Name}");

    public async Task Update(T entidad) =>
        await RunServiceAction(async () => await _service.Update(entidad), () => ReplaceEntity(entidad), $"Error al actualizar {typeof(T).Name}");

    public async Task Save(T entidad)
    {
        entidad.Empresa = SesionApp.IdEmpresa;
        await RunServiceAction(() => _service.Save(entidad), () => AddEntity(entidad), $"Error al guardar {typeof(T).Name}");
    }
}
