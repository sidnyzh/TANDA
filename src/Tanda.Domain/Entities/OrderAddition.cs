namespace Tanda.Domain.Entities;

public sealed class OrderAddition
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public Guid AdditionId { get; set; }

    public Addition Addition { get; set; } = null!;

    /// <summary>
    /// Precio vigente del adicional al momento de cotizar.
    /// Se almacena para que cambios posteriores en el catálogo no alteren
    /// cotizaciones ya emitidas.
    /// </summary>
    public decimal Price { get; set; }
}