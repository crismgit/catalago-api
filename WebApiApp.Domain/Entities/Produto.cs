using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using WebApiApp.Domain.Validation;

namespace WebApiApp.Domain.Entities
{
    public class Produto : Entity
    {
        // Construtor vazio para inicialização e uso do EF Core
        public Produto() { }

        // Construtor customizado para testes unitários e criação limpa
        [SetsRequiredMembers] // <--- Informa ao compilador que este construtor satisfaz todos os 'required'
        public Produto(int id, string nome, string descricao, decimal preco, int estoque, DateTime dataCadastro, int categoriaId, string? imagemUrl = null)
        {
            Id = id; // Define o Id herdado da base (protected set)
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Estoque = estoque;
            DataCadastro = dataCadastro;
            CategoriaId = categoriaId;
            ImagemUrl = imagemUrl;
        }

        [Required(ErrorMessage = "Nome inválido. O nome é obrigatório")]
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Descrição inválida. A descrição é obrigatória")]
        [MinLength(5, ErrorMessage = "A descrição deve ter no mínimo 5 caracteres")]
        public required string Descricao { get; set; }

        [Required(ErrorMessage = "Valor do preço inválido")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Valor do preço inválido")]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Preco { get; set; }

        [MaxLength(250, ErrorMessage = "O nome da imagem não pode exceder 250 caracteres")]
        public string? ImagemUrl { get; set; } // Permitindo nulo se a validação original aceitava string vazia/nula abaixo de 250 chars

        [Required(ErrorMessage = "Estoque inválido")]
        [Range(0, int.MaxValue, ErrorMessage = "Estoque inválido")]
        public required int Estoque { get; set; }

        [Required(ErrorMessage = "A data de cadastro é obrigatória")]
        public required DateTime DataCadastro { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public required int CategoriaId { get; set; }

        // Propriedade de navegação do EF Core (não usamos required para evitar validação circular na API)
        public Categoria? Categoria { get; set; }

        // Método Update adaptado para a nova estrutura de propriedades abertas
        public void Update(string nome, string descricao, decimal preco, string? imagemUrl,
            int estoque, DateTime dataCadastro, int categoriaId)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            ImagemUrl = imagemUrl;
            Estoque = estoque;
            DataCadastro = dataCadastro;
            CategoriaId = categoriaId;
        }
    }
}
