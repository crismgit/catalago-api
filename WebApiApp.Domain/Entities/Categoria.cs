using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApiApp.Domain.Validation;

namespace WebApiApp.Domain.Entities
{
    public sealed class Categoria : Entity
    {
        // Opcional: O EF Core ainda pode precisar de um construtor vazio, 
        // mas o 'required' obriga o preenchimento no código C# corporativo.
        public Categoria() { }

        // Construtor exclusivo para o Seed (HasData) ou situações específicas
        [SetsRequiredMembers]
        public Categoria(int id, string nome, string imagemUrl)
        {
            Id = id; // Funciona porque Categoria herda de Entity e acessa o 'protected set'
            Nome = nome;
            ImagemUrl = imagemUrl;
        }

        [Required(ErrorMessage = "Nome inválido. O nome é obrigatório")]
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres")]
        public required string Nome { get; set; } // Mudado para set/init para permitir o required

        [Required(ErrorMessage = "Nome da imagem inválido. O nome é obrigatório")]
        [MinLength(5, ErrorMessage = "Nome da imagem deve ter no mínimo 5 caracteres")]
        public required string ImagemUrl { get; set; }

        // Inicializado como uma lista vazia para evitar NullReferenceException
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
