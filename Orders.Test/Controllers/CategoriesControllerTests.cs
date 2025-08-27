using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.Backend.Controllers;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Test.Controllers
{
    [TestClass]
    public class CategoriesControllerTests
    {
        private Mock<IGenericUnitOfWork<Category>> _mockGenericUnitOfWork = null!;
        private Mock<ICategoriesUnitOfWork> _mockCategoriesUnitOfWork = null!;
        private CategoriesController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockGenericUnitOfWork = new Mock<IGenericUnitOfWork<Category>>();
            _mockCategoriesUnitOfWork = new Mock<ICategoriesUnitOfWork>();
            _controller = new CategoriesController(_mockGenericUnitOfWork.Object,
                _mockCategoriesUnitOfWork.Object);
        }

        [TestMethod]
        public async Task GetComboAsync_ReturnsOkObject()
        {
            // Arrange
            var comboData = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            _mockCategoriesUnitOfWork.Setup(uow => uow.GetComboAsync())
                .ReturnsAsync(comboData);
            // Act
            var result = await _controller.GetComboAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(comboData, okResult!.Value);
            _mockCategoriesUnitOfWork.Verify(uow => uow.GetComboAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOkObjectResult_WhenWasSuccessIsTrue()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new ActionResponse<IEnumerable<Category>> { WasSuccess = true };
            _mockCategoriesUnitOfWork.Setup(uow => uow.GetAsync(pagination))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(response.Result, okResult!.Value);
            _mockCategoriesUnitOfWork.Verify(uow => uow.GetAsync(pagination), Times.Once);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsOkObjectResult_WhenWasSuccessIsFalse()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new ActionResponse<IEnumerable<Category>> { WasSuccess = false };
            _mockCategoriesUnitOfWork.Setup(uow => uow.GetAsync(pagination))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockCategoriesUnitOfWork.Verify(uow => uow.GetAsync(pagination), Times.Once);
        }

        [TestMethod]
        public async Task GetPagesAsync_ReturnsOkObjectResult_WhenWasSuccessIsTrue()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new ActionResponse<int> { WasSuccess = true, Result = 30 };
            _mockCategoriesUnitOfWork.Setup(uow => uow.GetTotalPagesAsync(pagination))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPagesAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(response.Result, okResult!.Value);
            _mockCategoriesUnitOfWork.Verify(uow => uow.GetTotalPagesAsync(pagination), Times.Once);
        }

        [TestMethod]
        public async Task GetPagesAsync_ReturnsOkObjectResult_WhenWasSuccessIsFalse()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new ActionResponse<int> { WasSuccess = false };
            _mockCategoriesUnitOfWork.Setup(uow => uow.GetTotalPagesAsync(pagination))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPagesAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockCategoriesUnitOfWork.Verify(uow => uow.GetTotalPagesAsync(pagination), Times.Once);
        }
    }
}
