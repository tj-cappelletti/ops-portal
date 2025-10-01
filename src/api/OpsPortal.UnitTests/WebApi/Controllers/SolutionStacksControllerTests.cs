using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OpsPortal.Application.Features.SolutionStacks.Queries;
using OpsPortal.Contracts.Responses;
using OpsPortal.UnitTests.Helpers;
using OpsPortal.WebApi.Controllers;

namespace OpsPortal.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class SolutionStacksControllerTests
{
    private static SolutionStacksController CreateControllerWithMediatorMock(IMediator mediator, HeaderDictionary responseHeaders)
    {
        var httpRequestMock = new Mock<HttpRequest>();
        httpRequestMock.Setup(httpRequest => httpRequest.Scheme).Returns("https");
        httpRequestMock.Setup(httpRequest => httpRequest.Host).Returns(new HostString("localhost"));
        httpRequestMock.Setup(httpRequest => httpRequest.Path).Returns("/api/solution-stacks");
        httpRequestMock.Setup(httpRequest => httpRequest.QueryString).Returns(QueryString.Empty);

        var httpResponseMock = new Mock<HttpResponse>();
        httpResponseMock.Setup(httpResponse => httpResponse.Headers).Returns(responseHeaders);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);
        httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

        return new SolutionStacksController(mediator)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            }
        };
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public void Controller_Public_Methods_Should_Have_Http_Method_Attribute()
    {
        var solutionStacksControllerType = typeof(SolutionStacksController);

        var publicMethods = solutionStacksControllerType
            .GetMethods(BindingFlags.DeclaredOnly)
            .Where(m => m is { IsPublic: true, IsSpecialName: false }); // Exclude property getters/setters

        foreach (var method in publicMethods)
        {
            Type? attributeType = null;

            switch (method.Name)
            {
                case "GetAll":
                    attributeType = typeof(HttpGetAttribute);
                    break;
                case "GetById":
                    attributeType = typeof(HttpGetAttribute);
                    break;
                default:
                    Assert.Fail("Unsupported public HTTP operation");
                    break;
            }

            var attributes = method.GetCustomAttributes(attributeType, false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);
        }
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public void Controller_Should_Have_ApiController_Attribute()
    {
        var solutionStacksControllerType = typeof(SolutionStacksController);

        var attributes = solutionStacksControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public void Controller_Should_Have_Route_Attribute()
    {
        var solutionStacksControllerType = typeof(SolutionStacksController);

        var attributes = solutionStacksControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/solution-stacks", routeAttribute.Template);
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public async void GetAll_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var query = new GetAllSolutionStacks
        {
            PageNumber = 1,
            PageSize = 10
        };

        var items = new List<SolutionStackResponse>();

        var paginatedResponse = new PaginatedResponse<SolutionStackResponse>(items, 1, 10, 0, 0, false, false);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.Is(query, new GetAllSolutionStacksEqualityComparer()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResponse)
            .Verifiable(Times.Once);

        var responseHeaders = new HeaderDictionary();

        var controller = CreateControllerWithMediatorMock(mediatorMock.Object, responseHeaders);

        // Act
        var result = await controller.GetAll(query);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var returnedResponse = okResult.Value as PaginatedResponse<SolutionStackResponse>;
        Assert.NotNull(returnedResponse);
        Assert.Same(paginatedResponse, returnedResponse);

        // Verify pagination headers
        Assert.True(responseHeaders.ContainsKey("X-Pagination"));
        Assert.False(responseHeaders.ContainsKey("Link")); // No next/prev links for empty result

        // Verify mediator interactions
        mediatorMock.VerifyAll();
        mediatorMock.VerifyNoOtherCalls();
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public async void GetAll_Should_Return_Ok_When_NonEmpty()
    {
        // Arrange
        var items = new List<SolutionStackResponse>
        {
            new(
                Guid.NewGuid(),
                "Stack 1",
                "stack-1",
                "Description 1",
                "Category 1",
                "Active",
                "Owner 1",
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow.AddDays(-5)),
            new(
                Guid.NewGuid(),
                "Stack 2",
                "stack-2",
                "Description 2",
                "Category 2",
                "Inactive",
                "Owner 2",
                DateTime.UtcNow.AddDays(-8),
                DateTime.UtcNow.AddDays(-3))
        };

        var pageNumber = 1;
        var pageSize = items.Count;
        var totalPages = 5;

        var query = new GetAllSolutionStacks
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var paginatedResponse = new PaginatedResponse<SolutionStackResponse>(
            items,
            pageNumber,
            pageSize,
            totalPages * pageSize,
            totalPages,
            false,
            true);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.Is(query, new GetAllSolutionStacksEqualityComparer()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResponse)
            .Verifiable(Times.Once);

        var responseHeaders = new HeaderDictionary();

        var controller = CreateControllerWithMediatorMock(mediatorMock.Object, responseHeaders);

        // Act
        var result = await controller.GetAll(query);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var returnedResponse = okResult.Value as PaginatedResponse<SolutionStackResponse>;
        Assert.NotNull(returnedResponse);
        Assert.Same(paginatedResponse, returnedResponse);

        // Verify pagination headers
        Assert.True(responseHeaders.ContainsKey("X-Pagination"));
        Assert.True(responseHeaders.ContainsKey("Link")); // Next/prev links should be present

        // Verify mediator interactions
        mediatorMock.VerifyAll();
        mediatorMock.VerifyNoOtherCalls();
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public async void GetById_Should_Return_NotFound_When_Not_Found()
    {
        // Arrange
        var id = Guid.NewGuid();

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.Is<GetSolutionStackById>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SolutionStackResponse?)null)
            .Verifiable(Times.Once);

        var responseHeaders = new HeaderDictionary();

        var controller = CreateControllerWithMediatorMock(mediatorMock.Object, responseHeaders);

        // Act
        var result = await controller.GetById(id);

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);

        // Verify no pagination headers added
        Assert.False(responseHeaders.ContainsKey("X-Pagination"));
        Assert.False(responseHeaders.ContainsKey("Link"));

        // Verify mediator interactions
        mediatorMock.VerifyAll();
        mediatorMock.VerifyNoOtherCalls();
    }

    [Fact]
    [Trait(TestAnnotations.TraitNames.Category, TestAnnotations.Categories.Unit)]
    public async void GetById_Should_Return_Ok_When_Found()
    {
        // Arrange
        var id = Guid.NewGuid();

        var solutionStack = new SolutionStackResponse(
            id,
            "Stack 1",
            "stack-1",
            "Description 1",
            "Category 1",
            "Active",
            "Owner 1",
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(-5));

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.Is<GetSolutionStackById>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(solutionStack)
            .Verifiable(Times.Once);

        var responseHeaders = new HeaderDictionary();

        var controller = CreateControllerWithMediatorMock(mediatorMock.Object, responseHeaders);

        // Act
        var result = await controller.GetById(id);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var returnedResponse = okResult.Value as SolutionStackResponse;
        Assert.NotNull(returnedResponse);
        Assert.Same(solutionStack, returnedResponse);

        // Verify no pagination headers added
        Assert.False(responseHeaders.ContainsKey("X-Pagination"));
        Assert.False(responseHeaders.ContainsKey("Link"));

        // Verify mediator interactions
        mediatorMock.VerifyAll();
        mediatorMock.VerifyNoOtherCalls();
    }
}
