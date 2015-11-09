CREATE TABLE [dbo].[DataAccessLog]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[UserName]             NVARCHAR (256) NOT NULL,
	[PatientId] INT NULL,
	[ActionDescription]             NVARCHAR (256) NOT NULL,
	[AccessDate]     DATETIME        NULL,
    CONSTRAINT [PK_dbo.DataAccessLog] PRIMARY KEY CLUSTERED ([Id] ASC)
)
