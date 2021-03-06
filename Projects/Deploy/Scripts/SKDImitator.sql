USE [master]
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'SKDImirator')
BEGIN
	ALTER DATABASE [SKDImirator] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [SKDImirator]
END
GO
CREATE DATABASE [SKDImirator]
GO
USE [SKDImirator]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Devices](
	[UID] [uniqueidentifier] NOT NULL,
	[Port] [int],
	[LastJournalNo] [int],
	[BytesHash] BINARY(32),
	[TimeIntervalHash] [nvarchar](max) NULL,
CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED 
(
	[UID]
)
)
GO
