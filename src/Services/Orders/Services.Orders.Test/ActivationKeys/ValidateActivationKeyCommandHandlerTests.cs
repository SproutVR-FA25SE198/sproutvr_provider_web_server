using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using Moq;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Features.ValidateActivationKey;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Test.ActivationKeys;

#pragma warning disable CA1515 // Consider making public types internal
public class ValidateActivationKeyCommandHandlerTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
    private readonly ValidateActivationKeyCommandHandler _handler;

    public ValidateActivationKeyCommandHandlerTests()
    {
        // 1. Arrange Common Mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepo = new Mock<IGenericRepository<Order>>();

        // Setup UoW to return our mocked Order Repository
        _mockUnitOfWork.Setup(u => u.Repository<Order>())
                        .Returns(_mockOrderRepo.Object);

        // Initialize Handler
        _handler = new ValidateActivationKeyCommandHandler(_mockUnitOfWork.Object);
    }

    #region 1. Input Validation Scenarios

    // UTCID01
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenRequestDtoIsNull()
    {
        // Arrange
        var command = new ValidateActivationKeyCommand(null!);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Thiếu dữ liệu kích hoạt", exception.Message);
    }

    // UTCID02
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenActivationKeyIsNull()
    {
        // Arrange
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = Guid.NewGuid(),
            ActivationKey = null!
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Mã kích hoạt đang để trống", exception.Message);
    }

    // UTCID03
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenActivationKeyIsEmpty()
    {
        // Arrange
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = Guid.NewGuid(),
            ActivationKey = ""
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Mã kích hoạt đang để trống", exception.Message);
    }

    #endregion

    #region 2. Entity Lookup Scenarios

    // UTCID04
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenOrderNotFound()
    {
        // Arrange
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = Guid.NewGuid(),
            ActivationKey = "INVALID-KEY"
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        // Mock Repo to return null
        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Order>>()))
                      .ReturnsAsync((Order)null!);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Mã kích hoạt không hợp lệ", exception.Message);
    }

    #endregion

    #region 3. Business Logic & Security Scenarios

    // UTCID05
    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenOrganizationIdDoesNotMatch()
    {
        // Arrange
        var userOrgId = Guid.NewGuid();
        var actualOwnerOrgId = Guid.NewGuid(); // Different ID

        var requestDto = new ActivationRequestDto
        {
            OrganizationId = userOrgId,
            ActivationKey = "VALID-KEY"
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        var existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            OrganizationId = actualOwnerOrgId, // Matches Owner, not User
            IsKeyActivated = false
        };

        // Mock repo to return a valid order
        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Order>>()))
                      .ReturnsAsync(existingOrder);

        // Act & Assert
        UnauthorizedAccessException exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Bạn không có quyền kích hoạt mã này", exception.Message);
    }

    // UTCID06
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenKeyAlreadyActivated()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = orgId,
            ActivationKey = "VALID-KEY"
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        var existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            IsKeyActivated = true // Already used
        };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Order>>()))
                      .ReturnsAsync(existingOrder);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Mã này đã được kích hoạt", exception.Message);
    }

    #endregion

    #region 4. Persistence & Success Scenarios

    // UTCID07
    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenSaveChangesFails()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = orgId,
            ActivationKey = "VALID-KEY"
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        var existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            IsKeyActivated = false
        };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Order>>()))
                      .ReturnsAsync(existingOrder);

        // Mock SaveChangesAsync to return FALSE (Failure)
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Đã xảy ra lỗi, vui lòng thử lại sau", exception.Message);

        // Verification: Ensure we at least TRIED to set the flag
        Assert.True(existingOrder.IsKeyActivated);
    }

    // UTCID08
    [Fact]
    public async Task Handle_ShouldReturnPayload_WhenActivationIsSuccessful()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = orgId,
            ActivationKey = "VALID-KEY"
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        var existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            IsKeyActivated = false
        };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Order>>()))
                      .ReturnsAsync(existingOrder);

        // Mock SaveChangesAsync to return TRUE (Success)
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

        // Act
        ValidateActivationKeyPayloadDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingOrder.Id, result.OrderId);
        Assert.Equal(existingOrder.OrganizationId, result.OrganizationId);

        // Verify State Change
        Assert.True(existingOrder.IsKeyActivated, "Order.IsKeyActivated should be set to true");

        // Verify Interaction: SaveChangesAsync called exactly once
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // UTCID09
    [Fact]
    public async Task Handle_ShouldThrowGenericException_WhenUnexpectedErrorOccursDuringSave()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var requestDto = new ActivationRequestDto
        {
            OrganizationId = orgId,
            ActivationKey = "VALID-KEY"
        };
        var command = new ValidateActivationKeyCommand(requestDto);

        var existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            IsKeyActivated = false
        };

        _mockOrderRepo.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Order>>()))
                      .ReturnsAsync(existingOrder);

        // Mock SaveChangesAsync to THROW a generic system exception (e.g., DB connection lost)
        string expectedError = "Critical Database Failure";
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception(expectedError));

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(expectedError, exception.Message);
    }

    #endregion
}
