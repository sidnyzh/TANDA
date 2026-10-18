namespace Tanda.Domain.Rules
{
    internal static class CapacityCalculator
    {
        /// <summary>
        /// Devuelve la capacidad disponible como <c>total - committed</c>.
        /// Un resultado negativo es información válida: indica que la fecha está sobrecomprometida.
        /// </summary>
        public static int GetAvailable(int total, int committed) => total - committed;

        /// <summary>
        /// Indica si todavía hay espacio para el pedido.
        /// Usa comparación inclusiva (<c>&gt;=</c>), así que si lo disponible es igual a lo requerido, la fecha sí admite el pedido.
        /// Reutiliza <see cref="GetAvailable(int, int)"/>; un valor negativo sigue siendo un resultado válido y no se trata como error.
        /// </summary>
        public static bool HasRoomFor(int total, int committed, int required) => GetAvailable(total, committed) >= required;
    }
}
