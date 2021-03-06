USE master
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'Journal_1')
BEGIN
	SET NOEXEC ON
END
GO
CREATE DATABASE Journal_1
GO
USE Journal_1

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TYPE SqlTime FROM DATETIME

CREATE TYPE SqlDate FROM DATETIME
GO
CREATE RULE TimeOnlyRule AS
	datediff(dd, 0, @DateTime) = 0
GO
CREATE RULE DateOnlyRule AS
	dateAdd(dd, datediff(dd,0,@DateTime), 0) = @DateTime
GO
EXEC sp_bindrule 'TimeOnlyRule', 'SqlTime'
EXEC sp_bindrule 'DateOnlyRule', 'SqlDate'

CREATE TABLE [dbo].[Journal](
	[UID] [uniqueidentifier] NOT NULL,
	[SystemDate] [datetime] NOT NULL,
	[DeviceDate] [datetime] NULL,
	[Subsystem] [int] NOT NULL,
	[Name] [int] NOT NULL,
	[Description] [int] NOT NULL,
	[NameText] [nvarchar](max) NULL,
	[DescriptionText] [nvarchar](max) NULL,
	[ObjectType] [int] NOT NULL,
	[ObjectName] [nvarchar](50) NULL,
	[ObjectUID] [uniqueidentifier] NOT NULL,
	[Detalisation] [text] NULL,
	[UserName] [nvarchar](50) NULL,
	[EmployeeUID] [uniqueidentifier] NULL,
	[VideoUID] [uniqueidentifier] NULL,
	[CameraUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE INDEX JournalUIDIndex ON Journal([UID])
CREATE INDEX JournalDeviceDateIndex ON Journal([DeviceDate])
CREATE INDEX JournalSystemDateIndex ON Journal([SystemDate])
CREATE INDEX JournalNameIndex ON Journal([Name])
CREATE INDEX JournalDescriptionIndex ON Journal([Description])
CREATE INDEX JournalObjectUIDIndex ON Journal([ObjectUID])

CREATE TABLE Patches (Id nvarchar(100) not null)

INSERT INTO Patches (Id) VALUES
	('ExternalKeys')
INSERT INTO Patches (Id) VALUES
	('VideoUID')
INSERT INTO Patches (Id) VALUES
	('CameraUID')

SET NOEXEC OFF
