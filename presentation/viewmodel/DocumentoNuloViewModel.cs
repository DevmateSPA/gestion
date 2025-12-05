using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class DocumentoNuloViewModel : EntidadViewModel<DocumentoNulo>, INotifyPropertyChanged
{
    public ObservableCollection<DocumentoNulo> DocumentosNulos => Entidades;

    private ObservableCollection<DocumentoNulo> _documentosFiltrados = new();
    public ObservableCollection<DocumentoNulo> DocumentosNulosFiltrados
    {
        get => _documentosFiltrados;
        set { _documentosFiltrados = value; OnPropertyChanged(); }
    }

    public DocumentoNuloViewModel(IDocumentoNuloService documentoNuloService, IDialogService dialogService)
        : base(documentoNuloService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await base.LoadAll();
        DocumentosNulosFiltrados = new ObservableCollection<DocumentoNulo>(DocumentosNulos);
    }
    
    public override async Task LoadAllByEmpresa()
    {
        await base.LoadAllByEmpresa();
        DocumentosNulosFiltrados = new ObservableCollection<DocumentoNulo>(DocumentosNulos);
    }
}
