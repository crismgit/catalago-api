using System.ComponentModel.DataAnnotations;

namespace WebApiApp.Domain.Entities
{
    public abstract class Entity
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "Valor de Id inválido.")]
        public int Id { get; protected set; }
    }
}
