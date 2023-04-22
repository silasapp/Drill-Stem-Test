CREATE TABLE [dbo].[Facilities] (
    [FacilityID]      INT           IDENTITY (1, 1) NOT NULL,
    [ElpsFacilityID]  INT           NULL,
    [CompanyID]       INT           NOT NULL,
    [FacilityName]    VARCHAR (80)  NOT NULL,
    [FacilityAddress] VARCHAR (300) NOT NULL,
    [State]           INT           NOT NULL,
    [Lga]             VARCHAR (30)  NULL,
    [City]            VARCHAR (15)  NULL,
    [LandMeters]      INT           NULL,
    [isPipeLine]      BIT           NULL,
    [isHighTention]   BIT           NULL,
    [isHighWay]       BIT           NULL,
    [CreatedAt]       DATETIME      NOT NULL,
    [UpdatedAt]       DATETIME      NULL,
    [DeletedAt]       DATETIME      NULL,
    [DeletedBy]       INT           NULL,
    [DeleteStatus]    BIT           NOT NULL,
    [ContactName]     VARCHAR (30)  NULL,
    [ContactPhone]    VARCHAR (17)  NULL,
    CONSTRAINT [PK_Facility] PRIMARY KEY CLUSTERED ([FacilityID] ASC)
);

