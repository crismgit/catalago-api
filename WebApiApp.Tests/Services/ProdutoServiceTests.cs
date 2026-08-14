using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WebApiApp.Application.DTOs;
using WebApiApp.Application.Mappings;
using WebApiApp.Application.Services;
using WebApiApp.Domain.Entities;
using WebApiApp.Domain.Interfaces;

namespace WebApiApp.Tests.Services
{
    public class ProdutoServiceTests
    {
        private readonly Mock<IProdutoRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly ProdutoService _service;

        public ProdutoServiceTests()
        {
            _repositoryMock = new Mock<IProdutoRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                // Recomendo usar o seu Profile real criado no projeto
                cfg.AddProfile<DomainToDTOMappingProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = mapperConfig.CreateMapper();

            _service = new ProdutoService(_mapper, _repositoryMock.Object);
        }

        [Fact]
        public async Task Construtor_DeveLancarArgumentNullException_QuandoRepositorioForNulo()
        {
            // Act
            Action act = () => new ProdutoService(_mapper, null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("productRepository");
        }

        [Fact]
        public async Task GetProdutos_DeveRetornarListaDeProdutosDTO()
        {
            // Arrange - Garanta que os namespaces de 'Produto' e 'ProdutoDTO' estejam importados no topo
            var produtosEntity = new List<Produto>
            {
                new Produto
                { 
                    Nome = "Mouse Wireless",
                    Preco = 99.90m,
                    Descricao = "Qualquer texto aqui",
                    Estoque = 10,                 // <--- Adicionado campo obrigatório
                    DataCadastro = DateTime.Now,// <--- Adicione as exigidas pela classe
                    CategoriaId = 1                    // <--- Exemplo de outra propriedade exigida
                }
            };
            _repositoryMock.Setup(r => r.GetProdutosAsync()).ReturnsAsync(produtosEntity);

            // Act
            var result = await _service.GetProdutos();

            // Assert (FluentAssertions)
            result.Should().NotBeNullOrEmpty();

            // Como os tipos são diferentes (Produto vs ProdutoDTO), validamos se a lista retornada tem o mesmo tamanho
            result.Should().HaveCount(produtosEntity.Count());

            // Valida se as propriedades mapeadas batem com a origem
            var primeiroDto = result.First();
            var primeiroEntity = produtosEntity.First();
            primeiroDto.Id.Should().Be(primeiroEntity.Id);
            primeiroDto.Nome.Should().Be(primeiroEntity.Nome);
        }

        [Fact]
        public async Task GetById_DeveRetornarProdutoDTO_QuandoIdExistir()
        {
            
            // Arrange - Criação direta em uma única linha usando o novo construtor
            var produtoEntity = new Produto(10, "Mouse Wireless", "Qualquer texto aqui", 99.90m, 10, DateTime.Now, 1);

            _repositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(produtoEntity);


            // Act
            var result = await _service.GetById(10);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(10); // Agora o ID inserido via reflection será mapeado para o DTO
            result.Nome.Should().Be(produtoEntity.Nome);
        }

        [Fact]
        public async Task Add_DeveChamarCriacaoNoRepositorio()
        {
            // Arrange - Preenchendo todas as propriedades essenciais para o ProdutoDTO
            var dto = new ProdutoDTO
            {
                Id = 0, // 0 pois é um registro novo que será gerado no banco
                Nome = "Monitor",
                Descricao = "Monitor LED Full HD de 24 polegadas",
                Preco = 899.90m,
                Estoque = 15,
                DataCadastro = DateTime.Now,
                CategoriaId = 1,
                ImagemUrl = "monitor.png"
            };

            // Act
            await _service.Add(dto);

            // Assert - Valida os campos de negócio enviados ao repositório (ignorando o ID que o banco vai gerar)
            _repositoryMock.Verify(r => r.CreateAsync(It.Is<Produto>(p =>
                p.Nome == dto.Nome &&
                p.Descricao == dto.Descricao &&
                p.Preco == dto.Preco &&
                p.Estoque == dto.Estoque &&
                p.CategoriaId == dto.CategoriaId &&
                p.ImagemUrl == dto.ImagemUrl
            )), Times.Once);
        }

        [Fact]
        public async Task Update_DeveChamarAtualizacaoNoRepositorio()
        {
            var dto = new ProdutoDTO
            {
                Id = 1, // ID existente que será atualizado
                Nome = "Monitor Ultrawide",
                Descricao = "Monitor Ultrawide IPS de 29 polegadas",
                Preco = 1299.90m,
                Estoque = 10,
                DataCadastro = DateTime.Now,
                CategoriaId = 1,
                ImagemUrl = "monitor_ultrawide.png"
            };

            // Act
            await _service.Update(dto);

            // Assert - Valida se o ID correto e os novos dados mapeados foram enviados para a atualização
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Produto>(p =>
                p.Id == dto.Id &&
                p.Nome == dto.Nome &&
                p.Descricao == dto.Descricao &&
                p.Preco == dto.Preco &&
                p.Estoque == dto.Estoque &&
                p.CategoriaId == dto.CategoriaId &&
                p.ImagemUrl == dto.ImagemUrl
            )), Times.Once);
        }

        [Fact]
        public async Task Remove_DeveBuscarEExcluirProduto()
        {
            // Arrange - Utilizando o novo construtor para definir o Id e os campos required em uma única linha
            var produtoEntity = new Produto(
                id: 3,
                nome: "Notebook",
                descricao: "Notebook potente para trabalho",
                preco: 4500.00m,
                estoque: 5,
                dataCadastro: DateTime.Now,
                categoriaId: 2
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(produtoEntity);

            // Act
            await _service.Remove(3);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(3), Times.Once);
            _repositoryMock.Verify(r => r.RemoveAsync(produtoEntity), Times.Once);
        }
    }
}
