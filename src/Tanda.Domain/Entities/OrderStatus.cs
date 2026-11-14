namespace Tanda.Domain.Entities;

/// <summary>
/// Estados del ciclo de vida de un pedido.
/// </summary>
public enum OrderStatus
{
    /// <summary>Cotizado.</summary>
    Quoted = 1,

    /// <summary>Confirmado.</summary>
    Confirmed = 2,

    /// <summary>En producción.</summary>
    InProduction = 3,

    /// <summary>Listo.</summary>
    Ready = 4,

    /// <summary>Entregado.</summary>
    Delivered = 5,

    /// <summary>Cancelado.</summary>
    Cancelled = 6
}