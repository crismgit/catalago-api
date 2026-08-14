using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WebApiApp.Application.DTOs;
using WebApiApp.Application.Mappings;
using WebApiApp.Application.Services;
using WebApiApp.Domain.Entities;
using WebApiApp.Domain.Interfaces;

namespace WebApiApp.Tests
{
    public class CategoriaServiceTests
    {
        private readonly Mock<ICategoriaRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly CategoriaService _service;

        public CategoriaServiceTests()
        {
            _repositoryMock = new Mock<ICategoriaRepository>();

            // Configuração real do AutoMapper usando os seus Profiles
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                // Recomendo usar o seu Profile real criado no projeto
                cfg.AddProfile<DomainToDTOMappingProfile>();
            }, NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _service = new CategoriaService(_repositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetCategorias_DeveRetornarListaDeCategoriasDTO_QuandoExistiremDados()
        {
            // Arrange
            var categoriasEntity = new List<Categoria>
        {
            new() { Nome = "Eletrônicos", ImagemUrl = "teste.png" }
        };
            _repositoryMock.Setup(r => r.GetCategoriasAsync()).ReturnsAsync(categoriasEntity);

            // Act
            var result = await _service.GetCategorias();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(categoriasEntity, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetById_DeveRetornarCategoriaDTO_QuandoIdExistir()
        {
            // Arrange
            var categoriaEntity = new Categoria(1,"Livros", "teste.png");

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(categoriaEntity);

            // Act
            var result = await _service.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Nome.Should().Be(categoriaEntity.Nome);
        }

        [Fact]
        public async Task Add_DeveChamarCriacaoNoRepositorio_ComEntidadeMapeada()
        {
            // Arrange - Adicionada a propriedade ImagemUrl no DTO
            var dto = new CategoriaDTO
            {
                Id = 2, // Geralmente ignorado na criação, mas mantido se o DTO exigir
                Nome = "Games",
                ImagemUrl = "games.png" // <--- Campo adicionado
            };

            // Act
            await _service.Add(dto);

            // Assert - Ajustado para usar o mock de categoria e validando as propriedades enviadas
            _repositoryMock.Verify(r => r.CreateAsync(It.Is<Categoria>(c =>
                c.Nome == dto.Nome &&
                c.ImagemUrl == dto.ImagemUrl // <--- Verificação adicionada
            )), Times.Once);
        }

        [Fact]
        public async Task Update_DeveChamarAtualizacaoNoRepositorio_ComEntidadeMapeada()
        {
            // Arrange - Adicionada a propriedade ImagemUrl no DTO
            var dto = new CategoriaDTO
            {
                Id = 1,
                Nome = "Games Alterado",
                ImagemUrl = "games.png" // <--- Campo adicionado
            };

            // Act
            await _service.Update(dto);

            // Assert - Ajustado para usar o mock de categoria e validando a imagem também
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Categoria>(c =>
                c.Id == dto.Id &&
                c.Nome == dto.Nome &&
                c.ImagemUrl == dto.ImagemUrl // <--- Verificação adicionada
            )), Times.Once);
        }

        [Fact]
        public async Task Remove_DeveBuscarCategoriaERemover_QuandoIdForValido()
        {
            // Arrange - Agora correto: compila perfeitamente e o ID bate com a busca (5)
            var categoriaEntity = new Categoria(5, "Excluir", "teste.png");

            _repositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(categoriaEntity);

            // Act
            await _service.Remove(5);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(5), Times.Once);
            _repositoryMock.Verify(r => r.RemoveAsync(categoriaEntity), Times.Once);
        }
    }
}
