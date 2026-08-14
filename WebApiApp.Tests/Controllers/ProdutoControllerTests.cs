using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiApp.Application.Interfaces;
using WebApiApp.API.Controllers;
using WebApiApp.Application.DTOs;

public class ProdutosControllerTests
{
    private readonly Mock<IProdutoService> _serviceMock;
    private readonly ProdutosController _controller;

    public ProdutosControllerTests()
    {
        _serviceMock = new Mock<IProdutoService>();
        _controller = new ProdutosController(_serviceMock.Object);
    }

    [Fact]
    public async Task Get_DeveRetornarOkComListaDeProdutos()
    {
        // Arrange
        var produtosFakes = new List<ProdutoDTO> { new() { Id = 1, Nome = "Prod 1" } };
        _serviceMock.Setup(s => s.GetProdutos()).ReturnsAsync(produtosFakes);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(produtosFakes);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_QuandoProdutoExistir()
    {
        // Arrange
        var produtoFake = new ProdutoDTO { Id = 1, Nome = "Prod 1" };
        _serviceMock.Setup(s => s.GetById(1)).ReturnsAsync(produtoFake);

        // Act
        var result = await _controller.Get(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(produtoFake);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoProdutoNaoExistir()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetById(1)).ReturnsAsync((ProdutoDTO)null);

        // Act
        var result = await _controller.Get(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Post_DeveRetornarCreatedAtRoute_QuandoValido()
    {
        // Arrange
        var dto = new ProdutoDTO { Id = 5, Nome = "Novo" };

        // Act
        var result = await _controller.Post(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
        createdResult.RouteName.Should().Be("GetProduto");
        createdResult.RouteValues["id"].Should().Be(dto.Id);
        createdResult.Value.Should().Be(dto);
        _serviceMock.Verify(s => s.Add(dto), Times.Once);
    }

    [Fact]
    public async Task Post_DeveRetornarBadRequest_QuandoModelStateInvalido()
    {
        // Arrange
        var dto = new ProdutoDTO();
        _controller.ModelState.AddModelError("Nome", "Obrigatório");

        // Act
        var result = await _controller.Post(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _serviceMock.Verify(s => s.Add(It.IsAny<ProdutoDTO>()), Times.Never);
    }

    [Fact]
    public async Task Put_DeveRetornarOk_QuandoIdsForemIguais()
    {
        // Arrange
        var dto = new ProdutoDTO { Id = 2, Nome = "Editado" };

        // Act
        var result = await _controller.Put(2, dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
        _serviceMock.Verify(s => s.Update(dto), Times.Once);
    }

    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoIdsForemDiferentes()
    {
        // Arrange
        var dto = new ProdutoDTO { Id = 3 };

        // Act
        var result = await _controller.Put(2, dto);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _serviceMock.Verify(s => s.Update(It.IsAny<ProdutoDTO>()), Times.Never);
    }

    [Fact]
    public async Task Delete_DeveRetornarOk_QuandoProdutoExistir()
    {
        // Arrange
        var dto = new ProdutoDTO { Id = 1 };
        _serviceMock.Setup(s => s.GetById(1)).ReturnsAsync(dto);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(dto);
        _serviceMock.Verify(s => s.Remove(1), Times.Once);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoProdutoNaoExistir()
    {

        // Arrange
        _serviceMock.Setup(s => s.GetById(1)).ReturnsAsync((ProdutoDTO)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _serviceMock.Verify(s => s.Remove(It.IsAny<int>()), Times.Never);
    }
}

