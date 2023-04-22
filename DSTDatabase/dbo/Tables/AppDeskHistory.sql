CREATE TABLE [dbo].[AppDeskHistory] (
    [HistoryID] INT           IDENTITY (1, 1) NOT NULL,
    [AppID]     INT           NOT NULL,
    [ActionFrom]   VARCHAR(500)           NOT NULL,
    [Status]    VARCHAR (20)  NOT NULL,
    [Comment]   VARCHAR (MAX) NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [ActionTo] VARCHAR(500) NULL, 
    CONSTRAINT [PK_AppDeskHistory] PRIMARY KEY CLUSTERED ([HistoryID] ASC)
);

