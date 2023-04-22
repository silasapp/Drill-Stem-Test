CREATE TABLE [dbo].[AppTypeStage]
(
	[TypeStageID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AppTypeID] INT NOT NULL, 
    [AppStageID] INT NOT NULL, 
    [Counter] INT NOT NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [UpdatedAt] DATETIME NULL, 
    [DeleteStatus] BIT NOT NULL, 
    [DeletedBy] INT NULL, 
    [DeletedAt] DATETIME NULL
)
