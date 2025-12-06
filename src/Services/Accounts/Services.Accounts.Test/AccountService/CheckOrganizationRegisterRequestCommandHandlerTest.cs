using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Accounts;
using Common.Domain.Exceptions;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Moq;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CheckOrganizationRegisterRequest;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Test.AccountService;

public class CheckOrganizationRegisterRequestCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitMock;
    private readonly Mock<IGenericRepository<OrganizationRegisterRequest>> _repoMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly CheckOrganizationRegisterRequestCommandHandler _handler;

    public CheckOrganizationRegisterRequestCommandHandlerTests()
    {
        _unitMock = new Mock<IUnitOfWork>();
        _repoMock = new Mock<IGenericRepository<OrganizationRegisterRequest>>();
        _userManagerMock = MockUserManager();
        _publishMock = new Mock<IPublishEndpoint>();

        _unitMock.Setup(x => x.Repository<OrganizationRegisterRequest>())
            .Returns(_repoMock.Object);

        _handler = new CheckOrganizationRegisterRequestCommandHandler(
            _unitMock.Object,
            _userManagerMock.Object,
            _publishMock.Object
        );
    }

    // Helper to mock the concrete UserManager class
    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    // Helper to create a valid entity for tests
    private OrganizationRegisterRequest FakeRequestUnverified() =>
        new()
        {
            Id = Guid.NewGuid(),
            OrganizationName = "TestOrg",
            ContactEmail = "email@test.com",
            ContactPhone = "123456789",
            Address = "123 Test St",
            ApprovalStatus = ApprovalStatus.Unverified
        };

    private OrganizationRegisterRequest FakeRequestApprovalPending() =>
        new()
        {
            Id = Guid.NewGuid(),
            OrganizationName = "TestOrg",
            ContactEmail = "email@test.com",
            ContactPhone = "123456789",
            Address = "123 Test St",
            ApprovalStatus = ApprovalStatus.Approval_Pending
        };

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenApprovalStatusIsInvalidEnum()
    {
        // Arrange
        OrganizationRegisterRequest req = FakeRequestApprovalPending();
        _repoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync(req);

        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = req.Id,
            ApprovalStatus = "Invalid_Status_String"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenRequestDoesNotExist()
    {
        // Arrange
        // Mock repo returning null
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((OrganizationRegisterRequest?)null!);

        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = Guid.NewGuid(),
            ApprovalStatus = "Approved"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldApprove_CreateUser_AndPublish_WhenSuccessful()
    {
        // Arrange
        OrganizationRegisterRequest req = FakeRequestApprovalPending();
        _repoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync(req);

        // Mock Identity Success
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<Organization>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<Organization>(), AppCts.Roles.Organization))
            .ReturnsAsync(IdentityResult.Success);

        // Mock Save Changes to return true
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Setup Command
        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = req.Id,
            ApprovalStatus = "Approved"
        };

        // Act
        bool result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result);

        // Verify User Created with correct properties
        _userManagerMock.Verify(u => u.CreateAsync(
            It.Is<Organization>(o => o.Name == req.OrganizationName && o.Email == req.ContactEmail),
            It.IsAny<string>()), Times.Once);

        // Verify Role Assignment
        _userManagerMock.Verify(u => u.AddToRoleAsync(
            It.IsAny<Organization>(),
            AppCts.Roles.Organization), Times.Once);

        // Verify RabbitMQ Message
        _publishMock.Verify(p => p.Publish(
            It.Is<OrganizationRegisterRequestApprovedMessage>(msg =>
                msg.Email == req.ContactEmail &&
                msg.OrganizationId != null),
            It.IsAny<CancellationToken>()), Times.Once);

        // Verify DB Update
        _repoMock.Verify(r => r.Update(
            It.Is<OrganizationRegisterRequest>(x => x.ApprovalStatus == ApprovalStatus.Approved)), Times.Once);

        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationFailedException_WhenUserCreationFails()
    {
        // Arrange
        OrganizationRegisterRequest req = FakeRequestApprovalPending();
        _repoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync(req);

        // Mock Identity Failure
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<Organization>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password weak" }));

        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = req.Id,
            ApprovalStatus = "Approved"
        };

        // Act & Assert
        OperationFailedException ex = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(cmd, CancellationToken.None));

        Assert.Equal("Failed to add new organization!", ex.Message);

        // Verify never published success message
        _publishMock.Verify(p => p.Publish(It.IsAny<OrganizationRegisterRequestApprovedMessage>(), It.IsAny<CancellationToken>()), Times.Never);

        // Verify never saved changes to DB 
        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReject_AndPublishRejectMessage_WhenStatusIsRejected()
    {
        // Arrange
        OrganizationRegisterRequest req = FakeRequestUnverified();
        _repoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync(req);

        // Mock Save Changes to return true
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = req.Id,
            ApprovalStatus = "Rejected",
            RejectReason = "Documents unclear"
        };

        // Act
        bool result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result);

        // Verify no User created
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<Organization>(), It.IsAny<string>()), Times.Never);

        // Verify RabbitMQ Reject Message
        _publishMock.Verify(p => p.Publish(
            It.Is<OrganizationRegisterRequestRejectedMessage>(msg =>
                msg.Email == req.ContactEmail &&
                msg.Reason == "Documents unclear"),
            It.IsAny<CancellationToken>()), Times.Once);

        // Verify DB Update
        _repoMock.Verify(r => r.Update(
            It.Is<OrganizationRegisterRequest>(x => x.ApprovalStatus == ApprovalStatus.Rejected)), Times.Once);

        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("Approval_Pending")]
    public async Task Handle_ShouldOnlyUpdateStatus_WhenStatusIsNotApprovedOrRejected(string status)
    {
        // Arrange
        OrganizationRegisterRequest req = FakeRequestUnverified();
        _repoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync(req);

        // Mock Save Changes to return true
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        ApprovalStatus expectedEnum = Enum.Parse<ApprovalStatus>(status);

        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = req.Id,
            ApprovalStatus = status
        };

        // Act
        await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        // Verify no Publish calls (neither Approved nor Rejected)
        _publishMock.Verify(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);

        // Verify no User creation
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);

        // Verify DB Update
        _repoMock.Verify(r => r.Update(
            It.Is<OrganizationRegisterRequest>(x => x.ApprovalStatus == expectedEnum)), Times.Once);

        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdate_WhenApprovingUnverifiedRequest()
    {
        // Arrange
        OrganizationRegisterRequest req = FakeRequestUnverified();
        _repoMock.Setup(r => r.GetByIdAsync(req.Id)).ReturnsAsync(req);

        // Mock Identity Success
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<Organization>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<Organization>(), AppCts.Roles.Organization))
            .ReturnsAsync(IdentityResult.Success);

        // Mock Save Changes to return true
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Setup Command
        var cmd = new CheckOrganizationRegisterRequestCommand
        {
            OrganizationRegisterRequestId = req.Id,
            ApprovalStatus = "Approved"
        };

        // Act
        bool result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        // Expect the handler to return 'false' because the transition is invalid
        Assert.False(result, "Handler should return false when approving an Unverified request.");
    }
}
