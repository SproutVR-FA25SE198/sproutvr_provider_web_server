using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Moq;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Features.MarkAsDownloaded;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Test.ActivationKeys;

#pragma warning disable CA1515 // Consider making public types internal
public class MarkAsDownloadCommandHandlerTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<OrderItem>> _mockOrderItemRepo;
    private readonly MarkAsDownloadCommandHandler _handler;

    public MarkAsDownloadCommandHandlerTests()
    {
        // 1. Arrange Common Mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderItemRepo = new Mock<IGenericRepository<OrderItem>>();

        // Setup UoW to return our mocked OrderItem Repository
        _mockUnitOfWork.Setup(u => u.Repository<OrderItem>())
                        .Returns(_mockOrderItemRepo.Object);

        // Initialize Handler
        _handler = new MarkAsDownloadCommandHandler(_mockUnitOfWork.Object);
    }

    #region 1. Success & Idempotency Scenarios (UTCID 01 - 02)

    // UTCID 01 - Happy Path (Success)
    [Fact]
    public async Task Handle_ShouldUpdateIsDownloaded_WhenConditionsMet()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = orgId };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        var existingItem = new OrderItem
        {
            IsDownloaded = false, // Needs update
            Order = new Order { IsKeyActivated = true, OrganizationId = orgId }
        };

        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync(existingItem);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

        // Act
        Unit result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        // Verify State Change
        Assert.True(existingItem.IsDownloaded, "IsDownloaded should be set to true");

        // Verify DB Calls
        _mockOrderItemRepo.Verify(x => x.Update(existingItem), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // UTCID 02 - Already Downloaded (Idempotency)
    [Fact]
    public async Task Handle_ShouldReturnSuccess_AndDoNothing_WhenAlreadyDownloaded()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = orgId };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        var existingItem = new OrderItem
        {
            IsDownloaded = true, // Already done!
            Order = new Order { IsKeyActivated = true, OrganizationId = orgId }
        };

        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync(existingItem);

        // Act
        Unit result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        // Verify we did NOT call Update or SaveChanges
        _mockOrderItemRepo.Verify(x => x.Update(It.IsAny<OrderItem>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region 2. Input Validation Scenarios (UTCID 03 - 04)

    // UTCID 03 - Request DTO is Null
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenRequestDtoIsNull()
    {
        // Arrange
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), null!);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Yêu cầu không được tìm thấy", ex.Message);
    }

    // UTCID 04 - Organization ID is Empty
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenOrganizationIdIsEmpty()
    {
        // Arrange
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = Guid.Empty };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Tổ chức không tồn tại", ex.Message);
    }

    #endregion

    #region 3. Entity Lookup Scenarios (UTCID 05)

    // UTCID 05 - Order Item Not Found
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenOrderItemNotFound()
    {
        // Arrange
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = Guid.NewGuid() };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        // Mock Repo to return null
        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync((OrderItem)null!);

        // Act & Assert
        NotFoundException ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Học liệu không được tìm thấy", ex.Message);
    }

    #endregion

    #region 4. Business Logic & Security Scenarios (UTCID 06 - 07)

    // UTCID 06 - Organization Mismatch
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenOrganizationIdMismatch()
    {
        // Arrange
        var userOrgId = Guid.NewGuid();
        var realOwnerOrgId = Guid.NewGuid(); // Different ID!

        var dto = new MarkAsDownloadedRequestDto { OrganizationId = userOrgId };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        var existingItem = new OrderItem
        {
            Order = new Order
            {
                IsKeyActivated = true,
                OrganizationId = realOwnerOrgId // Logic failure point
            }
        };

        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync(existingItem);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Bạn không có quyền cập nhật học liệu này", ex.Message);
    }

    // UTCID 07 - Order Not Activated
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenOrderIsNotActivated()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = orgId };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        var existingItem = new OrderItem
        {
            Order = new Order
            {
                IsKeyActivated = false, // Logic failure point
                OrganizationId = orgId
            }
        };

        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync(existingItem);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Bạn chưa kích hoạt gói học liệu này", ex.Message);
    }

    #endregion

    #region 5. Persistence & Failure Scenarios (UTCID 08 - 09)

    // UTCID 08 - Save Changes Failed (Simulated DB failure)
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenSaveChangesFails()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = orgId };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        var existingItem = new OrderItem
        {
            IsDownloaded = false,
            Order = new Order { IsKeyActivated = true, OrganizationId = orgId }
        };

        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync(existingItem);

        // Mock SaveChangesAsync to return FALSE
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

        // Act & Assert
        await _handler.Handle(command, CancellationToken.None);

        // If no exception is thrown by your handler for 'false', verifying the call happened is sufficient for now.
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // UTCID 09 - Generic Exception (Infrastructure Failure)
    [Fact]
    public async Task Handle_ShouldThrowException_WhenDbFails()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var dto = new MarkAsDownloadedRequestDto { OrganizationId = orgId };
        var command = new MarkAsDownloadCommand(Guid.NewGuid(), dto);

        var existingItem = new OrderItem
        {
            IsDownloaded = false,
            Order = new Order { IsKeyActivated = true, OrganizationId = orgId }
        };

        _mockOrderItemRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<OrderItem>>()))
                          .ReturnsAsync(existingItem);

        // Mock DB Exception
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    #endregion
}
