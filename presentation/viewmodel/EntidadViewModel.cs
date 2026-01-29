using System.Collections.ObjectModel;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.service;
using Gestion.helpers;
using System.Reflection;
using Gestion.core.session;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Diagnostics;

namespace Gestion.presentation.viewmodel;

/// <summary>
/// ViewModel base genérico para entidades asociadas a una empresa.
///
/// Proporciona:
/// - Carga asincrónica con control de cancelación
/// - Manejo de estado de carga (IsLoading)
/// - Búsqueda textual optimizada con caché
/// - Paginación
/// - Operaciones CRUD sincronizadas con la UI
///
/// Está pensado para ser heredado por ViewModels concretos
/// (Ej: ClienteViewModel, FacturaViewModel, etc.).
/// </summary>
/// <typeparam name="T">
/// Tipo de entidad manejada, debe implementar <see cref="IEmpresa"/>.
/// </typeparam>
public abstract class EntidadViewModel<T> : INotifyPropertyChanged
    where T : IEmpresa
{
    protected bool DebugEnabled = false;

    [Conditional("DEBUG")]
    protected void Log(string message)
    {
        if (DebugEnabled)
            Debug.WriteLine(message);
    }
    #region Servicios y mensajes

    /// <summary>
    /// Servicio de diálogo para mostrar mensajes de error o información.
    /// </summary>
    protected readonly IDialogService _dialogService;

    /// <summary>
    /// Servicio base de acceso a datos para la entidad.
    /// </summary>
    protected readonly IBaseService<T> _service;

    /// <summary>
    /// Mensaje mostrado cuando no existen registros.
    /// </summary>
    protected readonly string _emptyMessage;

    /// <summary>
    /// Mensaje mostrado cuando ocurre un error al cargar datos.
    /// </summary>
    protected readonly string _errorMessage;

    #endregion

    #region Colección principal y vista filtrada

    private ObservableCollection<T> _entidades = [];

    /// <summary>
    /// Colección observable de entidades cargadas.
    /// </summary>
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

    /// <summary>
    /// Vista de colección utilizada para filtrado y binding en la UI.
    /// </summary>
    public ICollectionView EntidadesView =>
        CollectionViewSource.GetDefaultView(Entidades);

    #endregion

    #region Búsqueda

    private string _filtro = "";

    /// <summary>
    /// Texto de búsqueda actual.
    /// </summary>
    public string Filtro
    {
        get => _filtro;
        set
        {
            _filtro = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Caché de texto de búsqueda por entidad,
    /// usado para evitar reflexión repetida.
    /// </summary>
    private readonly Dictionary<T, string> _searchCache = new();

    #endregion

    #region Estado de carga

    private bool _isLoading;

    /// <summary>
    /// Indica si el ViewModel se encuentra cargando datos.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Token de cancelación para cargas concurrentes.
    /// </summary>
    private CancellationTokenSource? _loadCts;

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Paginación

    private int _pageNumber = 1;

    /// <summary>
    /// Página actual (1-based).
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            _pageNumber = value;
            OnPropertyChanged();
        }
    }

    private int _pageSize = 50;

    /// <summary>
    /// Cantidad de registros por página.
    /// Si es 0, se cargan todos los registros.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value;
            OnPropertyChanged();
        }
    }

    private int _totalRegistros;

    /// <summary>
    /// Total de páginas disponibles.
    /// </summary>
    public int TotalRegistros
    {
        get => _totalRegistros;
        set
        {
            _totalRegistros = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Constructor

    protected EntidadViewModel(
        IBaseService<T> baseService,
        IDialogService dialogService,
        string? emptyMessage = null,
        string? errorMessage = null)
    {
        _dialogService = dialogService;
        _service = baseService;

        _emptyMessage =
            emptyMessage ??
            $"No hay {typeof(T).Name} para la empresa {SesionApp.NombreEmpresa}";

        _errorMessage =
            errorMessage ??
            $"Error al cargar {typeof(T).Name} de la empresa {SesionApp.NombreEmpresa}";
    }

    #endregion

    #region Búsqueda interna por reflexión

    private readonly PropertyInfo[] _stringProps =
        typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

    /// <summary>
    /// Construye una cadena normalizada utilizada para búsquedas rápidas
    /// sobre una entidad.
    ///
    /// Combina todas las propiedades públicas de tipo <see cref="string"/>
    /// en un solo texto en minúsculas.
    /// </summary>
    /// <param name="entidad">Entidad desde la cual se extrae el texto.</param>
    /// <returns>
    /// Texto concatenado y normalizado para búsquedas.
    /// </returns>
    private string BuildSearchText(T entidad)
    {
        return string.Join(" ",
            _stringProps
                .Select(p => p.GetValue(entidad) as string)
                .Where(s => !string.IsNullOrWhiteSpace(s))
        ).ToLowerInvariant();
    }

    /// <summary>
    /// Aplica un filtro textual sobre la vista de entidades.
    /// </summary>
    public virtual void Buscar(string filtro)
    {
        Log($"Buscar → '{filtro}'");
        if (string.IsNullOrWhiteSpace(filtro))
        {
            Log("Buscar → filtro vacío, limpiando");
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

    #endregion

    #region Manipulación de colección (UI-safe)

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

    #endregion

    #region Carga con control de concurrencia

    /// <summary>
    /// Ejecuta una operación asincrónica de carga controlando:
    /// - Estado visual de carga
    /// - Cancelación de operaciones previas
    /// - Manejo centralizado de errores
    /// - Ejecución opcional de callbacks
    /// </summary>
    /// <param name="action">Función asincrónica que obtiene los datos.</param>
    /// <param name="errorMessage">Mensaje a mostrar si ocurre un error.</param>
    /// <param name="onSuccess">Acción opcional al completar exitosamente.</param>
    /// <param name="onEmpty">
    /// Acción opcional cuando la carga no devuelve resultados.
    /// </param>
    protected async Task RunWithLoading(
        Func<Task<List<T>>> action,
        string errorMessage,
        Action? onSuccess = null,
        Action? onEmpty = null)
    {
        IsLoading = true;
        var token = RenewLoadToken();
        Log("RunWithLoading → inicio");

        try
        {
            Log("RunWithLoading → ejecutando action");
            var lista = await SafeExecutor.RunAsyncList(
                async () =>
                {
                    token.ThrowIfCancellationRequested();

                    Log("RunWithLoading → action START");

                    var result = await action();

                    Log($"RunWithLoading → action END ({result.Count} registros)");

                    token.ThrowIfCancellationRequested();
                    return result;
                },
                _dialogService,
                errorMessage);

            if (token.IsCancellationRequested)
            {
                Log("RunWithLoading → cancelado después de action");
                return;
            }

            if (lista.Count == 0)
            {
                Log("RunWithLoading → lista vacía");
                onEmpty?.Invoke();
            }
            else
            {
                Log("RunWithLoading → cargando entidades en colección");
            }

            await LoadEntitiesIntoCollection(lista);

            Log("RunWithLoading → LoadEntitiesIntoCollection OK");

            onSuccess?.Invoke();
            Log("RunWithLoading → onSuccess ejecutado");
        }
        catch (OperationCanceledException)
        {
            Log("RunWithLoading → OperationCanceledException");
            // Cancelación silenciosa
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsLoading = false;
                Log("RunWithLoading → fin (IsLoading=false)");
            }
            else
            {
                Log("RunWithLoading → fin cancelado");
            }
        }
    }

    /// <summary>
    /// Cancela cualquier proceso de carga anterior y crea un nuevo
    /// <see cref="CancellationToken"/> para la operación actual.
    ///
    /// Se utiliza para evitar condiciones de carrera cuando se disparan
    /// múltiples cargas consecutivas (por ejemplo, búsquedas rápidas
    /// o cambios de página).
    /// </summary>
    /// <returns>
    /// Token de cancelación asociado a la operación de carga actual.
    /// </returns>
    private CancellationToken RenewLoadToken()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        return _loadCts.Token;
    }

    #endregion

    #region Carga y ordenamiento

    protected static PropertyInfo? GetDateProperty(Type t)
    {
        return t.GetProperty(
            "Fecha",
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.IgnoreCase);
    }

    /// <summary>
    /// Carga una lista de entidades en la colección observable,
    /// realizando previamente:
    /// - Ordenamiento por fecha (si existe propiedad "Fecha")
    /// - Construcción del caché de búsqueda
    ///
    /// La preparación se realiza en segundo plano y la actualización
    /// de la colección en el hilo de UI.
    /// </summary>
    /// <param name="lista">Lista de entidades obtenidas del servicio.</param>
    protected async Task LoadEntitiesIntoCollection(List<T> lista)
    {
        Log($"LoadEntitiesIntoCollection → inicio ({lista.Count} items)");

        var procesadas = await Task.Run(() =>
        {
            var dateProp = GetDateProperty(typeof(T));

            if (dateProp != null)
            {
                Log("LoadEntitiesIntoCollection → ordenando por Fecha");
                lista = [.. lista.OrderByDescending(x => dateProp.GetValue(x))];
            }
            else
            {
                Log("LoadEntitiesIntoCollection → sin propiedad Fecha");
            }

            var cache = new Dictionary<T, string>();
            foreach (var entidad in lista)
                cache[entidad] = BuildSearchText(entidad);
            
            Log($"LoadEntitiesIntoCollection → cache construido ({cache.Count})");

            return (lista, cache);
        });

        Log("LoadEntitiesIntoCollection → aplicando cambios a la UI");

        Entidades.Clear();
        _searchCache.Clear();

        foreach (var kv in procesadas.cache)
            _searchCache[kv.Key] = kv.Value;

        Entidades = new ObservableCollection<T>(procesadas.lista);

        Log("LoadEntitiesIntoCollection → fin");
    }

    #endregion

    #region Carga pública

    public virtual async Task LoadAll() =>
        await RunWithLoading(
            _service.FindAll,
            _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));

    public virtual async Task LoadAllByEmpresa() =>
        await RunWithLoading(
            () => _service.FindAllByEmpresa(SesionApp.IdEmpresa),
            _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));

    #endregion

    #region Paginación pública

    protected async Task LoadPagedEntities(
        Func<int, Task<List<T>>> serviceCall,
        int page,
        string emptyMessage,
        string errorMessage,
        Func<Task<long>>? totalCountCall = null,
        Func<Task<List<T>>>? allItemsCall = null)
    {
        Log($"LoadPagedEntities → solicitado page={page}, PageSize={PageSize}");
        if (PageSize == 0)
        {
            Log("LoadPagedEntities → PageSize=0, cargando todo");
            await RunWithLoading(
                async () =>
                    allItemsCall != null
                        ? await allItemsCall()
                        : await _service.FindAllByEmpresa(SesionApp.IdEmpresa),
                errorMessage,
                onEmpty: () => _dialogService.ShowMessage(emptyMessage));

            PageNumber = 1;
            TotalRegistros = 1;

            Log("LoadPagedEntities → sin paginación (1 página)");
            return;
        }

        PageNumber = Math.Max(1, page);
        Log($"LoadPagedEntities → PageNumber ajustado a {PageNumber}");

        long total = totalCountCall != null
            ? await totalCountCall()
            : await _service.ContarPorEmpresa(SesionApp.IdEmpresa);

        Log($"LoadPagedEntities → total registros={total}");

        TotalRegistros = Math.Max(
            1,
            (int)Math.Ceiling(total / (double)PageSize));

        Log($"LoadPagedEntities → TotalRegistros={TotalRegistros}");

        await RunWithLoading(
            () => serviceCall(PageNumber),
            errorMessage,
            onEmpty: () => _dialogService.ShowMessage(emptyMessage));
    }

    public virtual async Task LoadPageByEmpresa(int page) =>
        await LoadPagedEntities(
            p => _service.FindPageByEmpresa(SesionApp.IdEmpresa, p, PageSize),
            page,
            _emptyMessage,
            _errorMessage);

    #endregion

    #region Operaciones CRUD

    /// <summary>
    /// Ejecuta una acción de servicio que retorna un resultado booleano,
    /// manejando errores de forma centralizada y ejecutando una acción
    /// secundaria en caso de éxito.
    ///
    /// Usado para operaciones CRUD (Save, Update, Delete).
    /// </summary>
    /// <param name="serviceAction">Acción asincrónica del servicio.</param>
    /// <param name="onSuccess">Acción a ejecutar si el resultado es exitoso.</param>
    /// <param name="mensajeError">Mensaje mostrado si ocurre un error.</param>
    /// <returns>
    /// <c>true</c> si la operación fue exitosa; en caso contrario <c>false</c>.
    /// </returns>
    private protected async Task<bool> RunServiceAction(
        Func<Task<bool>> serviceAction,
        Action? onSuccess,
        string mensajeError)
    {
        Log("RunServiceAction → inicio");

        return await SafeExecutor.RunAsyncValue(
            async () =>
            {
                Log("RunServiceAction → ejecutando serviceAction");

                var result = await serviceAction();

                Log($"RunServiceAction → resultado = {result}");

                if (result)
                {
                    Log("RunServiceAction → ejecutando onSuccess");
                    onSuccess?.Invoke();
                    return true;
                }

                Log("RunServiceAction → operación fallida");
                return false;
            },
            _dialogService,
            mensajeError);
    }

    public virtual async Task Delete(long id) =>
        await RunServiceAction(
            () => _service.DeleteById(id),
            () => RemoveEntityById(id),
            $"Error al eliminar {typeof(T).Name}");

    public async Task<bool> Update(T entidad) =>
        await RunServiceAction(
            () => _service.Update(entidad),
            () => ReplaceEntity(entidad),
            $"Error al actualizar {typeof(T).Name}");

    public async Task<bool> Save(T entidad)
    {
        entidad.Empresa = SesionApp.IdEmpresa;

        return await RunServiceAction(
            () => _service.Save(entidad),
            () => AddEntity(entidad),
            $"Error al guardar {typeof(T).Name}");
    }

    #endregion
}