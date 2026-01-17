using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel;

public class DocumentoNuloViewModel : EntidadViewModel<DocumentoNulo>, INotifyPropertyChanged
{
    public DocumentoNuloViewModel(
        IDocumentoNuloService documentoNuloService, 
        IDialogService dialogService,
        IFacturaService facturaService)
        : base(documentoNuloService, dialogService)
    {
        _ = DataBootstrapper.LoadFoliosFacturaSearch(facturaService, SesionApp.IdEmpresa);
    }
}
