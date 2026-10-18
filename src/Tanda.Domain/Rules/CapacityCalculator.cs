using System;

namespace Tanda.Domain.Rules
{
    /// <summary>
    /// Calculadora simple de capacidad.
    /// </summary>
    public static class CapacityCalculator
    {
        /// <summary>
        /// Devuelve la capacidad disponible como <c>total - committed</c>.
        /// Un resultado negativo es información válida: indica que la fecha está sobrecomprometida
        /// (es un resultado, no una excepción).
        /// </summary>
        public static int GetAvailable(int total, int committed) => total - committed;

        /// <summary>
        /// Indica si hay espacio para el esfuerzo requerido.
        /// La comparación es inclusiva (<c>&gt;=</c>): si lo disponible es igual a lo requerido,
        /// la fecha admite el pedido. Reutiliza <see cref="GetAvailable(int,int)"/>.
        /// </summary>
        public static bool HasRoomFor(int total, int committed, int required) =>
            GetAvailable(total, committed) >= required;
    }
}
