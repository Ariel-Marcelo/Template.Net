-- Get all users
CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt
    FROM Users;
END
GO

-- Get user by ID
CREATE OR ALTER PROCEDURE sp_GetUserById
    @Id uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt
    FROM Users
    WHERE Id = @Id;
END
GO

-- Get user by username
CREATE OR ALTER PROCEDURE sp_GetUserByUsername
    @Username nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt
    FROM Users
    WHERE Username = @Username;
END
GO

-- Get user by email
CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt
    FROM Users
    WHERE Email = @Email;
END
GO

-- Create user
CREATE OR ALTER PROCEDURE sp_CreateUser
    @Id uniqueidentifier,
    @Username nvarchar(100),
    @Email nvarchar(100),
    @PasswordHash nvarchar(max),
    @FirstName nvarchar(100),
    @LastName nvarchar(100),
    @CreatedAt datetime2,
    @UpdatedAt datetime2
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Users (Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt)
    VALUES (@Id, @Username, @Email, @PasswordHash, @FirstName, @LastName, @CreatedAt, @UpdatedAt);
    
    SELECT Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt
    FROM Users
    WHERE Id = @Id;
END
GO

-- Update user
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @Id uniqueidentifier,
    @Username nvarchar(100),
    @Email nvarchar(100),
    @PasswordHash nvarchar(max),
    @FirstName nvarchar(100),
    @LastName nvarchar(100),
    @UpdatedAt datetime2
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Users
    SET Username = @Username,
        Email = @Email,
        PasswordHash = @PasswordHash,
        FirstName = @FirstName,
        LastName = @LastName,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
    
    SELECT Id, Username, Email, PasswordHash, FirstName, LastName, CreatedAt, UpdatedAt
    FROM Users
    WHERE Id = @Id;
END
GO

-- Delete user
CREATE OR ALTER PROCEDURE sp_DeleteUser
    @Id uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Success bit = 0;
    
    IF EXISTS (SELECT 1 FROM Users WHERE Id = @Id)
    BEGIN
        DELETE FROM Users WHERE Id = @Id;
        SET @Success = 1;
    END
    
    SELECT @Success as Success;
END
GO 