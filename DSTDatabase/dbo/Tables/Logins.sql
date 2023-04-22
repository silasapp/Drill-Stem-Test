﻿CREATE TABLE [dbo].[Logins] (
    [LoginID]     INT           IDENTITY (1, 1) NOT NULL,
    [UserID]      INT           NOT NULL,
    [RoleID]      INT           NOT NULL,
    [HostName]    VARCHAR (30)  NULL,
    [MacAddress]  VARCHAR (30)  NULL,
    [Local_Ip]    VARCHAR (30)  NULL,
    [Remote_Ip]   VARCHAR (30)  NULL,
    [UserAgent]   VARCHAR (200) NULL,
    [LoginTime]   DATETIME      NOT NULL,
    [LogoutTime]  DATETIME      NULL,
    [LoginStatus] VARCHAR (12)  NOT NULL,
    CONSTRAINT [PK_Logins] PRIMARY KEY CLUSTERED ([LoginID] ASC)
);

