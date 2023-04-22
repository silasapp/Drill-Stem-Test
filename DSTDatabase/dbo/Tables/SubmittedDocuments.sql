CREATE TABLE [dbo].[SubmittedDocuments] (
    [SubDocID]      INT           IDENTITY (1, 1) NOT NULL,
    [AppID]         INT           NOT NULL,
    [AppDocID]      INT           NOT NULL,
    [CompElpsDocID] INT           NULL,
    [CreatedAt]     DATETIME      NOT NULL,
    [UpdatedAt]     DATETIME      NULL,
    [DeleteStatus]  BIT           NOT NULL,
    [DeletedBy]     INT           NULL,
    [DeletedAt]     DATETIME      NULL,
    [DocSource]     VARCHAR (MAX) NULL,
    [isAddictional] BIT           NULL,
    CONSTRAINT [PK_SubmittedDocuments] PRIMARY KEY CLUSTERED ([SubDocID] ASC)
);

