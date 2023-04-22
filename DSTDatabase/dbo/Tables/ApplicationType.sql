CREATE TABLE [dbo].[ApplicationType]
(
	[AppTypeID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TypeName] VARCHAR(50) NOT NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [UpdatedAt] DATETIME NULL, 
    [DeleteStatus] BIT NOT NULL, 
    [DeletedBy] INT NULL, 
    [DeletedAt] DATETIME NULL
)
