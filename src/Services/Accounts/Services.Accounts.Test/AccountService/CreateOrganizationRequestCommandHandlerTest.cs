using System.Linq.Expressions;
using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Accounts;
using Common.Domain.Exceptions;
using MassTransit;
using Moq;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;
using Services.Accounts.Domain.Entities.Organizations;

namespace Services.Accounts.Test.AccountService;

public class CreateOrganizationRequestCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitMock;
    private readonly Mock<IGenericRepository<OrganizationRegisterRequest>> _reqRepoMock;
    private readonly Mock<IOrganizationRepository> _orgRepoMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly CreateOrganizationRequestCommandHandler _handler;

    public CreateOrganizationRequestCommandHandlerTests()
    {
        _unitMock = new Mock<IUnitOfWork>();
        _reqRepoMock = new Mock<IGenericRepository<OrganizationRegisterRequest>>();
        _orgRepoMock = new Mock<IOrganizationRepository>();
        _publishMock = new Mock<IPublishEndpoint>();

        _unitMock.Setup(u => u.Repository<OrganizationRegisterRequest>())
            .Returns(_reqRepoMock.Object);

        _handler = new CreateOrganizationRequestCommandHandler(
            _unitMock.Object,
            _orgRepoMock.Object,
            _publishMock.Object
        );
    }

    private CreateOrganizationRequestCommand FakeCreateOrganizationRequestCommand() =>
        new()
        {
            ContactEmail = "testorg@email.com",
            ContactPhone = "0123456789",
            OrganizationName = "Test Org",
            Address = "123 St"
        };

    [Fact]
    public async Task Handle_ShouldThrowException_WhenOrganizationAlreadyExists()
    {
        // Arrange
        CreateOrganizationRequestCommand command = FakeCreateOrganizationRequestCommand();

        // Simulate Organization already exists in the system
        _orgRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppCts.Errors.OrganizationRegisterRequests.Duplicated, exception.Message);

        // Verify to have never checked the Request table or tried to save
        _reqRepoMock.Verify(r => r.GetEntityWithSpec(It.IsAny<OrganizationRequestSpecification>()), Times.Never);
        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRequestAlreadyExists()
    {
        // Arrange
        CreateOrganizationRequestCommand command = FakeCreateOrganizationRequestCommand();

        // Organization does not exist
        _orgRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(false);

        // Request for this email/phone already exists
        var existingRequest = new OrganizationRegisterRequest();
        _reqRepoMock.Setup(r => r.GetEntityWithSpec(It.IsAny<OrganizationRequestSpecification>()))
            .ReturnsAsync(existingRequest);

        // Act & Assert
        OperationFailedException exception = await Assert.ThrowsAsync<OperationFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppCts.Errors.Organizations.Duplicated, exception.Message);

        // Verify never tried to add or publish
        _reqRepoMock.Verify(r => r.Add(It.IsAny<OrganizationRegisterRequest>()), Times.Never);
        _publishMock.Verify(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateRequestAndPublishMessage_WhenRequestIsValid()
    {
        // Arrange
        CreateOrganizationRequestCommand command = FakeCreateOrganizationRequestCommand();

        // Organization does not exist
        _orgRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(false);

        // Request does not exist
        _reqRepoMock.Setup(r => r.GetEntityWithSpec(It.IsAny<OrganizationRequestSpecification>()))
            .ReturnsAsync((OrganizationRegisterRequest)null!);

        // Mock SaveChanges
        _unitMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); 

        // Act
        OrganizationRegisterRequestDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.OrganizationName, result.OrganizationName);
        Assert.Equal("Unverified", result.ApprovalStatus); 

        // Verify Entity was Added with correct generated values
        _reqRepoMock.Verify(r => r.Add(It.Is<OrganizationRegisterRequest>(x =>
            x.OrganizationName == command.OrganizationName &&
            x.ApprovalStatus == ApprovalStatus.Unverified &&
            !x.IsEmailVerified &&
            !string.IsNullOrEmpty(x.EmailVerificationToken) && // Token was generated
            x.EmailVerificationTokenExpiry > DateTime.UtcNow // Expiry is in future
        )), Times.Once);

        // Verify Message was Published to RabbitMQ
        _publishMock.Verify(p => p.Publish(It.Is<OrganizationRequestCreatedMessage>(msg =>
            msg.Email == command.ContactEmail &&
            msg.OrganizationName == command.OrganizationName &&
            !string.IsNullOrEmpty(msg.VerificationToken) // Token is included in message
        ), It.IsAny<CancellationToken>()), Times.Once);

        // Verify DB Commit
        _unitMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
