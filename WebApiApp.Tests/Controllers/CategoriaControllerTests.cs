using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using WebApiApp.API.Controllers;
using WebApiApp.Application.DTOs;
using WebApiApp.Application.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace WebApiApp.Tests.Controllers
{
    public class CategoriasControllerTests
    {
        private readonly Mock<ICategoriaService> _serviceMock;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _serviceMock = new Mock<ICategoriaService>();
            _controller = new CategoriasController(_serviceMock.Object);
        }

        [Fact]
        public async Task Get_DeveRetornarOkComListaDeCategorias()
        {
            // Arrange
            var listaFake = new List<CategoriaDTO> { new() { Id = 1, Nome = "TI" } };
            _serviceMock.Setup(s => s.GetCategorias()).ReturnsAsync(listaFake);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(listaFake);
        }

        [Fact]
        public async Task GetById_DeveRetornarOk_QuandoCategoriaExistir()
        {
            // Arrange
            var dtoFake = new CategoriaDTO { Id = 1, Nome = "TI" };
            _serviceMock.Setup(s => s.GetById(1)).ReturnsAsync(dtoFake);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(dtoFake);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoCategoriaNaoExistir()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetById(1)).ReturnsAsync((CategoriaDTO)null);

            // Act
            var result = await _controller.Get(1);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Post_DeveRetornarCreatedAtRoute_QuandoValido()
        {
            // Arrange
            var dto = new CategoriaDTO { Id = 2, Nome = "Livros" };

            // Act
            var result = await _controller.Post(dto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteName.Should().Be("GetCategoria");
            createdResult.Value.Should().Be(dto);
            _serviceMock.Verify(s => s.Add(dto), Times.Once);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoModelStateInvalido()
        {
            // Arrange
            var dto = new CategoriaDTO();
            _controller.ModelState.AddModelError("Nome", "Obrigatório");

            // Act
            var result = await _controller.Post(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _serviceMock.Verify(s => s.Add(It.IsAny<CategoriaDTO>()), Times.Never);
        }

        [Fact]
        public async Task Put_DeveRetornarOk_QuandoDadosValidos()
        {
            // Arrange
            var dto = new CategoriaDTO { Id = 1, Nome = "Atualizada" };

            // Act
            var result = await _controller.Put(1, dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(dto);
            _serviceMock.Verify(s => s.Update(dto), Times.Once);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequest_QuandoIdsForemDiferentes()
        {
            // Arrange
            var dto = new CategoriaDTO { Id = 2 };

            // Act
            var result = await _controller.Put(1, dto);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            _serviceMock.Verify(s => s.Update(It.IsAny<CategoriaDTO>()), Times.Never);
        }

        [Fact]
        public async Task Delete_DeveRetornarOk_QuandoCategoriaExistir()
        {
            // Arrange
            var dtoFake = new CategoriaDTO { Id = 4 };
            _serviceMock.Setup(s => s.GetById(4)).ReturnsAsync(dtoFake);

            // Act
            var result = await _controller.Delete(4);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(dtoFake);
            _serviceMock.Verify(s => s.Remove(4), Times.Once);
        }
    }
}
