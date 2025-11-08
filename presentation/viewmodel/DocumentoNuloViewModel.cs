using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class DocumentoNuloViewModel : EntidadViewModel<DocumentoNulo>
{
    public ObservableCollection<DocumentoNulo> DocumentoNulo => Entidades;
    public DocumentoNuloViewModel(IDocumentoNuloService documentoNuloService, IDialogService dialogService)
        : base(documentoNuloService, dialogService) {}
}
