CREATE TABLE [dbo].[AppStageDocuments] (
    [StageDocID]   INT      IDENTITY (1, 1) NOT NULL,
    [AppStageID]   INT      NOT NULL,
    [AppDocID]     INT      NOT NULL,
    [CreatedAt]    DATETIME NOT NULL,
    [UpdatedAt]    DATETIME NULL,
    [DeleteStatus] BIT      NOT NULL,
    [DeletedBy]    INT      NULL,
    [DeletedAt]    DATETIME NULL,
    CONSTRAINT [PK_AppStageDocuments] PRIMARY KEY CLUSTERED ([StageDocID] ASC)
);

