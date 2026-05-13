
CREATE TABLE Employees
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    Name VARCHAR(100) NOT NULL,

    Email VARCHAR(150) NOT NULL UNIQUE,

    Department VARCHAR(100) NULL,

    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE Transactions
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    UserId UNIQUEIDENTIFIER NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    Type VARCHAR(20) NOT NULL, -- Credit / Debit

    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Transactions_Employees
    FOREIGN KEY(UserId)
    REFERENCES Employees(Id)
);
GO

CREATE OR ALTER PROCEDURE sp_AddEmployee
(
    @Name VARCHAR(100),
    @Email VARCHAR(150),
    @Department VARCHAR(100) = NULL,

    @Message VARCHAR(500) OUTPUT,
    @Code INT OUTPUT
)
AS
BEGIN

    IF EXISTS
    (
        SELECT 1
        FROM Employees
        WHERE Email = @Email
    )
    BEGIN
        SET @Code = 1;
        SET @Message = 'Email already exists';

        RETURN;
    END

    INSERT INTO Employees
    (
        Name,
        Email,
        Department
    )
    VALUES
    (
        @Name,
        @Email,
        @Department
    );

    SET @Code = 0;
    SET @Message = 'Employee added successfully';

END
GO

CREATE OR ALTER PROCEDURE sp_UpdateEmployee
(
    @Id UNIQUEIDENTIFIER,
    @Name VARCHAR(100),
    @Email VARCHAR(150),
    @Department VARCHAR(100) = NULL,

    @Message VARCHAR(500) OUTPUT,
    @Code INT OUTPUT
)
AS
BEGIN

    IF NOT EXISTS
    (
        SELECT 1
        FROM Employees
        WHERE Id = @Id
    )
    BEGIN
        SET @Code = 1;
        SET @Message = 'Employee not found';

        RETURN;
    END

    IF EXISTS
    (
        SELECT 1
        FROM Employees
        WHERE Email = @Email
        AND Id <> @Id
    )
    BEGIN
        SET @Code = 1;
        SET @Message = 'Email already exists';

        RETURN;
    END

    UPDATE Employees
    SET
        Name = @Name,
        Email = @Email,
        Department = @Department
    WHERE Id = @Id;

    SET @Code = 0;
    SET @Message = 'Employee updated successfully';

END
GO


CREATE OR ALTER PROCEDURE sp_DeleteEmployee
(
    @Id UNIQUEIDENTIFIER,
    @Message VARCHAR(500) OUTPUT,
    @Code INT OUTPUT
)
AS
BEGIN

    IF NOT EXISTS
    (
        SELECT 1
        FROM Employees
        WHERE Id = @Id
    )
    BEGIN
        SET @Code = 1;
        SET @Message = 'Employee not found';

        RETURN;
    END

    DELETE FROM Transactions
    WHERE UserId = @Id;

    DELETE FROM Employees
    WHERE Id = @Id;

    SET @Code = 0;
    SET @Message = 'Employee deleted successfully';

END
GO


CREATE OR ALTER PROCEDURE sp_AddTransaction
(
    @UserId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18,2),
    @Type VARCHAR(20),

    @Message VARCHAR(500) OUTPUT,
    @Code INT OUTPUT
)
AS
BEGIN

    DECLARE @Balance DECIMAL(18,2);
	
    IF NOT EXISTS
    (
        SELECT 1
        FROM Employees
        WHERE Id = @UserId
    )
    BEGIN
        SET @Code = 1;
        SET @Message = 'Employee does not exist';

        RETURN;
    END
	
    IF @Amount <= 0
    BEGIN
        SET @Code = 1;
        SET @Message = 'Amount must be greater than 0';

        RETURN;
    END

    IF @Type NOT IN ('Credit','Debit')
    BEGIN
        SET @Code = 1;
        SET @Message = @Type;

        RETURN;
    END

    SELECT
        @Balance =
        ISNULL
        (
            SUM
            (
                CASE
                    WHEN Type = 'Credit' THEN Amount
                    WHEN Type = 'Debit' THEN -Amount
                END
            ),
            0
        )
    FROM Transactions
    WHERE UserId = @UserId;

    IF @Type = 'Debit'
    AND @Amount > @Balance
    BEGIN
        SET @Code = 1;
        SET @Message = 'Insufficient balance';

        RETURN;
    END

    INSERT INTO Transactions
    (
        UserId,
        Amount,
        Type
    )
    VALUES
    (
        @UserId,
        @Amount,
        @Type
    );

    SET @Code = 0;
    SET @Message = 'Transaction added successfully';

END
GO
CREATE OR ALTER PROCEDURE sp_GetEmployees
(
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search VARCHAR(100) = NULL
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Email,
        Department,
        CreatedDate
    FROM Employees
    WHERE
    (
        @Search IS NULL
        OR Name LIKE '%' + @Search + '%'
        OR Department LIKE '%' + @Search + '%'
    )
    ORDER BY CreatedDate DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

END
GO

CREATE OR ALTER PROCEDURE sp_GetEmployeeById
(
    @Id UNIQUEIDENTIFIER
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Email,
        Department,
        CreatedDate
    FROM Employees
    WHERE Id = @Id;

END
GO

CREATE OR ALTER PROCEDURE sp_GetTransactionsByUser
(
    @UserId UNIQUEIDENTIFIER,
    @PageNumber INT = 1,
    @PageSize INT = 10
)
AS
BEGIN

    SET NOCOUNT ON;

    ;WITH TransactionData AS
    (
        SELECT
            Id,
            UserId,
            Amount,
            Type,
            CreatedDate,

            SUM
            (
                CASE
                    WHEN Type = 'Credit'
                    THEN Amount

                    WHEN Type = 'Debit'
                    THEN -Amount
                END
            )
            OVER
            (
                PARTITION BY UserId
                ORDER BY CreatedDate, Id
            ) AS RunningBalance

        FROM Transactions
        WHERE UserId = @UserId
    )

    SELECT
        Id,
        UserId,
        Amount,
        Type,
        CreatedDate,
        RunningBalance

    FROM TransactionData

    ORDER BY CreatedDate DESC

    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

END
GO

select * from Employees
select * from Transactions