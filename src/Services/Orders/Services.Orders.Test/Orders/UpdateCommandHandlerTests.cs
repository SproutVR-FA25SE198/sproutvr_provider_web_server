using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Orders;
using MassTransit;
using Moq;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.Abstractions.Services;
using Services.Orders.Application.BusinessLogics.Orders.Features.AssignSystemAdmin;
using Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Test.Orders;

#pragma warning disable CA1515 // Consider making public types internal
public class UpdateOrderCommandHandlerTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly Mock<IGrpcAccountClient> _mockAccountClient;
    private readonly Mock<IActivationKeyGeneratorService> _mockKeyGenerator;
    private readonly UpdateOrderCommandHandler _handler;

    public UpdateOrderCommandHandlerTests()
    {
        // 1. Arrange Common Mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepo = new Mock<IGenericRepository<Order>>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _mockAccountClient = new Mock<IGrpcAccountClient>();
        _mockKeyGenerator = new Mock<IActivationKeyGeneratorService>();

        // Setup UoW to return our mocked Order Repository
        _mockUnitOfWork.Setup(u => u.Repository<Order>())
                        .Returns(_mockOrderRepo.Object);

        // Initialize Handler
        _handler = new UpdateOrderCommandHandler(
            _mockUnitOfWork.Object,
            _mockPublishEndpoint.Object,
            _mockAccountClient.Object,
            _mockKeyGenerator.Object
        );
    }

    #region 1. Validation & Logic Scenarios

    // UTCID 01 - Order Not Found
    // Condition: Order is null
    // Expectation: IsSuccess = false, OrderId = null
    [Fact]
    public async Task Handle_ShouldReturnFail_WhenOrderNotFound()
    {
        // Arrange
        var command = new UpdateOrderCommand { OrderCode = 112324324, Status = "Payment_Pending" };

        // Mock Repo returning null
        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>()))
                      .ReturnsAsync((Order?)null!);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.OrderId);

        // Verify Save is never called
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // UTCID 02 - Invalid Status
    // Condition: Status is empty string or invalid
    // Expectation: ArgumentException
    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenStatusIsInvalid()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10000, Status = "" }; // Invalid empty status
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10000, Status = OrderStatus.Payment_Pending };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>()))
                      .ReturnsAsync(existingOrder);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    // UTCID 03 - Happy Path (Simple Status Update)
    // Condition: Payment_Pending -> Canceled
    // Expectation: Success, OrderId = ORDER-B
    [Fact]
    public async Task Handle_ShouldUpdateStatus_WhenTransitionIsSimple()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10001, Status = "Canceled" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10001, Status = OrderStatus.Payment_Pending };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(existingOrder.Id, result.OrderId);
        Assert.Equal(OrderStatus.Canceled, existingOrder.Status);

        // Verify side effects did NOT happen
        _mockAccountClient.Verify(x => x.GetSystemAdminWithMinPendingOrdersAsync(), Times.Never);
        _mockPublishEndpoint.Verify(x => x.Publish(It.IsAny<OrderCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // UTCID 04 - Payment Success (Admin Assigned)
    // Condition: Payment_Pending -> Bundle_Pending, Admin = ADMIN-A
    // Expectation: Success, OrderId = ORDER-B, Admin assigned, Event Published
    [Fact]
    public async Task Handle_ShouldAssignAdminAndPublish_WhenStatusIsBundlePending()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10001, Status = "Bundle_Pending" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10001, Status = OrderStatus.Payment_Pending };
        var adminDto = new SystemAdminDto { SystemAdminId = Guid.NewGuid() };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);
        _mockAccountClient.Setup(x => x.GetSystemAdminWithMinPendingOrdersAsync()).ReturnsAsync(adminDto);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Bundle_Pending, existingOrder.Status);
        Assert.Equal(adminDto.SystemAdminId, existingOrder.AssignedSystemAdminId);

        // Verify Publish called
        _mockPublishEndpoint.Verify(x => x.Publish(It.IsAny<OrderCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // UTCID 05 - Payment Success (No Admin)
    // Condition: Payment_Pending -> Bundle_Pending, Admin = null
    // Expectation: Success, OrderId = ORDER-B, No Admin assigned, Event Published
    [Fact]
    public async Task Handle_ShouldPublishOnly_WhenStatusIsBundlePendingAndNoAdmin()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10001, Status = "Bundle_Pending" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10001, Status = OrderStatus.Payment_Pending };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);
        _mockAccountClient.Setup(x => x.GetSystemAdminWithMinPendingOrdersAsync()).ReturnsAsync((SystemAdminDto?)null); // No Admin
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(existingOrder.AssignedSystemAdminId);
        _mockPublishEndpoint.Verify(x => x.Publish(It.IsAny<OrderCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // UTCID 06 - Finished (Key Generation)
    // Condition: Bundle_Pending -> Finished
    // Expectation: Success, OrderId = ORDER-C, Key Generated
    [Fact]
    public async Task Handle_ShouldGenerateKey_WhenStatusIsFinished()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10002, Status = "Finished" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10002, Status = OrderStatus.Bundle_Pending };
        string fakeKey = "KEY-123";

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);
        _mockKeyGenerator.Setup(x => x.Generate()).Returns(fakeKey);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Finished, existingOrder.Status);
        Assert.Equal(fakeKey, existingOrder.ActivationKey);
    }

    // UTCID 07 - Idempotency
    // Condition: Bundle_Pending -> Bundle_Pending
    // Expectation: Success, No triggers
    [Fact]
    public async Task Handle_ShouldDoNothingExtra_WhenStatusIsUnchanged()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10002, Status = "Bundle_Pending" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10002, Status = OrderStatus.Bundle_Pending }; // Already Bundle Pending

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify no specific logic triggered because status didn't change
        _mockAccountClient.Verify(x => x.GetSystemAdminWithMinPendingOrdersAsync(), Times.Never);
        _mockPublishEndpoint.Verify(x => x.Publish(It.IsAny<OrderCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region 2. Persistence & Exception Scenarios

    // UTCID 08 - Save Changes Fails
    // Condition: SaveChanges returns FALSE
    // Expectation: IsSuccess = false
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenSaveFails()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10001, Status = "Finished" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10001, Status = OrderStatus.Bundle_Pending };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);
        // Mock Save Failure
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        UpdateOrderResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(existingOrder.Id, result.OrderId);
    }

    // UTCID 09 - Unexpected Exception
    // Condition: Server not started
    // Expectation: Throws Exception
    [Fact]
    public async Task Handle_ShouldThrowException_WhenDependencyFails()
    {
        // Arrange
        var command = new UpdateOrderCommand{ OrderCode = 10001, Status = "Bundle_Pending" };
        var existingOrder = new Order { Id = Guid.NewGuid(), OrderCode = 10001, Status = OrderStatus.Payment_Pending };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<OrderSpecification>())).ReturnsAsync(existingOrder);

        // Mock Exception in dependency
        _mockAccountClient.Setup(x => x.GetSystemAdminWithMinPendingOrdersAsync())
                          .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    #endregion
}
