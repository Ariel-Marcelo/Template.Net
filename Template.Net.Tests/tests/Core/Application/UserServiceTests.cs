using Moq;
using NUnit.Framework;
using Template.Core.Application.DTOs;
using Template.Core.Application.Services;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Interfaces;
using Template.Core.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace Template.Net.Tests.Core.Application;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Username = "user1", Email = "user1@test.com" },
            new() { Id = Guid.NewGuid(), Username = "user2", Email = "user2@test.com" }
        };

        _userRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        _userRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Email = "test@test.com" };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(userId));
        Assert.That(result.Username, Is.EqualTo("testuser"));
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_WithValidData_ShouldCreateAndReturnUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto(
            "newuser",
            "new@test.com",
            "password123",
            "John",
            "Doe"
        );

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(createUserDto.Username))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(createUserDto.Email))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _userService.CreateUserAsync(createUserDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo(createUserDto.Username));
        Assert.That(result.Email, Is.EqualTo(createUserDto.Email));
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public void CreateUserAsync_WithExistingUsername_ShouldThrowException()
    {
        // Arrange
        var createUserDto = new CreateUserDto(
            "existinguser",
            "new@test.com",
            "password123",
            "John",
            "Doe"
        );

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(createUserDto.Username))
            .ReturnsAsync(new User { Username = createUserDto.Username });

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.CreateUserAsync(createUserDto)
        );
        Assert.That(ex.Message, Is.EqualTo("Username already exists"));
    }

    [Test]
    public async Task UpdateUserAsync_WithValidData_ShouldUpdateAndReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            Username = "existinguser",
            Email = "existing@test.com",
            FirstName = "John",
            LastName = "Doe"
        };

        var updateUserDto = new UpdateUserDto(
            "new@test.com",
            "newpassword",
            "Jane",
            "Smith",
            true,
            "password123"
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(updateUserDto.Email))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateUserDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(updateUserDto.Email));
        Assert.That(result.FirstName, Is.EqualTo(updateUserDto.FirstName));
        Assert.That(result.LastName, Is.EqualTo(updateUserDto.LastName));
        Assert.That(result.IsActive, Is.EqualTo(updateUserDto.IsActive));
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public void UpdateUserAsync_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto(
            "new@test.com",
            "newpassword",
            "Jane",
            "Smith",
            true,
            "Pasiljiv"
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.UpdateUserAsync(userId, updateUserDto)
        );
        Assert.That(ex.Message, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task DeleteUserAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.DeleteAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.That(result, Is.True);
        _userRepositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
    }
} 