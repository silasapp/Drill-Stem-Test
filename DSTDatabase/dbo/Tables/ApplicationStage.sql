CREATE TABLE [dbo].[ApplicationStage] (
    [AppStageID]    INT          IDENTITY (1, 1) NOT NULL,
    [StageName]     VARCHAR (50) NOT NULL,
    [ShortName]     VARCHAR (10)  NULL,
    [Amount]        INT          NULL,
    [ServiceCharge] INT          NULL,
    [CreatedAt]     DATETIME     NOT NULL,
    [UpdatedAt]     DATETIME     NULL,
    [DeleteStatus]  BIT          NOT NULL,
    [DeletedBy]     INT          NULL,
    [DeletedAt]     DATETIME     NULL,
    CONSTRAINT [PK_ApplicationStage] PRIMARY KEY CLUSTERED ([AppStageID] ASC)
);

