using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using Moq;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.VerifyEmail;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Test.AccountService;

public class VerifyEmailCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitMock;
    private readonly Mock<IGenericRepository<OrganizationRegisterRequest>> _repoMock;
    private readonly VerifyEmailCommandHandler _handler;

    public VerifyEmailCommandHandlerTests()
    {
        _unitMock = new Mock<IUnitOfWork>();
        _repoMock = new Mock<IGenericRepository<OrganizationRegisterRequest>>();

        _unitMock.Setup(u => u.Repository<OrganizationRegisterRequest>())
            .Returns(_repoMock.Object);

        _handler = new VerifyEmailCommandHandler(_unitMock.Object);
    }

    private OrganizationRegisterRequest FakeRequest() =>
        new()
        {
            Id = Guid.NewGuid(),
            ApprovalStatus = ApprovalStatus.Unverified,
            IsEmailVerified = false,
            EmailVerificationToken = "VALID_TOKEN",
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRequestDoesNotExist()
    {
        // Arrange
        var command = new VerifyEmailCommand { OrganizationRegisterRequestId = Guid.NewGuid() };

        _repoMock.Setup(r => r.GetByIdAsync(command.OrganizationRegisterRequestId))
            .ReturnsAsync((OrganizationRegisterRequest)null!);

        // Act & Assert
        NotFoundException ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppCts.Errors.OrganizationRegisterRequests.NotFound, ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEmailAlreadyVerified()
    {
        // Arrange
        OrganizationRegisterRequest orgRequest = FakeRequest();
        orgRequest.IsEmailVerified = true;

        var command = new VerifyEmailCommand { 
            OrganizationRegisterRequestId = orgRequest.Id,
            Token = orgRequest.EmailVerificationToken!
        };

        _repoMock.Setup(r => r.GetByIdAsync(orgRequest.Id)).ReturnsAsync(orgRequest);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Email đã được xác nhận trước đó.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenTokenDoesNotMatch()
    {
        // Arrange
        OrganizationRegisterRequest orgRequest = FakeRequest();

        // Command sends wrong token
        var command = new VerifyEmailCommand
        {
            OrganizationRegisterRequestId = orgRequest.Id,
            Token = "WRONG_TOKEN"
        };

        _repoMock.Setup(r => r.GetByIdAsync(orgRequest.Id)).ReturnsAsync(orgRequest);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Mã xác nhận không hợp lệ.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenTokenIsExpired()
    {
        // Arrange
        OrganizationRegisterRequest orgRequest = FakeRequest();
        orgRequest.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(-24);

        var command = new VerifyEmailCommand
        {
            OrganizationRegisterRequestId = orgRequest.Id,
            Token = orgRequest.EmailVerificationToken!
        };

        _repoMock.Setup(r => r.GetByIdAsync(orgRequest.Id)).ReturnsAsync(orgRequest);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Mã xác nhận đã hết hạn. Vui lòng yêu cầu gửi lại email xác nhận.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSaveChangesFails()
    {
        // Arrange: Valid Request
        OrganizationRegisterRequest orgRequest = FakeRequest();

        var command = new VerifyEmailCommand
        {
            OrganizationRegisterRequestId = orgRequest.Id,
            Token = orgRequest.EmailVerificationToken!
        };

        _repoMock.Setup(r => r.GetByIdAsync(orgRequest.Id)).ReturnsAsync(orgRequest);

        // Setup SaveChanges to return false 
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Không thể xác nhận email. Vui lòng thử lại.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldVerifyEmail_WhenRequestIsValid()
    {
        // Arrange: Valid Request
        OrganizationRegisterRequest orgRequest = FakeRequest();

        var command = new VerifyEmailCommand
        {
            OrganizationRegisterRequestId = orgRequest.Id,
            Token = orgRequest.EmailVerificationToken!
        };

        _repoMock.Setup(r => r.GetByIdAsync(orgRequest.Id)).ReturnsAsync(orgRequest);

        // Setup SaveChanges to return true 
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        bool result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        // Verify Entity Update Logic
        _repoMock.Verify(r => r.Update(It.Is<OrganizationRegisterRequest>(x =>
            x.IsEmailVerified &&
            x.EmailVerificationToken == null &&         // Token cleared
            x.EmailVerificationTokenExpiry == null &&   // Expiry cleared
            x.ApprovalStatus == ApprovalStatus.Approval_Pending // Status updated
        )), Times.Once);

        // Verify SaveChanges was called
        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
