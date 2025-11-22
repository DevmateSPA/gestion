namespace Gestion.core.interfaces.model;

public interface IDetalle : IModel
{
    string Folio { get; set; }
    string Tipo { get; set; }
    string Producto { get; set; }
    long Entrada { get; set; }
    long Salida { get; set; }
    string Maquina { get; set; }
    string Operario { get; set; }
    DateTime Fecha { get; set; }
}