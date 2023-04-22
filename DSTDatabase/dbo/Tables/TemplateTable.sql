CREATE TABLE [dbo].[TemplateTable] (
    [TempId]         INT           IDENTITY (1, 1) NOT NULL,
    [AppId]          INT           NULL,
    [OmlOpl]         VARCHAR (500) NOT NULL,
    [FieldName]      VARCHAR (500) NOT NULL,
    [Reservior]      VARCHAR (500) NOT NULL,
    [WellName]       VARCHAR (500) NOT NULL,
    [String]         VARCHAR (500) NULL,
    [Terrian]        VARCHAR (500) NOT NULL,
    [StartDate]      DATETIME      NULL,
    [EndDate]        DATETIME      NULL,
    [FluidType]      VARCHAR (500) NULL,
    [CreatedAt]      DATETIME      NOT NULL,
    [DriveMechanism] VARCHAR (500) NULL,
    CONSTRAINT [PK__Template__06C703C17DD7C56F] PRIMARY KEY CLUSTERED ([TempId] ASC)
);




