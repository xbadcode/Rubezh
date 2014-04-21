USE [master]
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'SKUD')
BEGIN
	ALTER DATABASE [SKUD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [SKUD]
END
GO
CREATE DATABASE [SKUD] 
GO
USE [SKUD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TYPE SqlTime FROM DATETIME
GO
CREATE TYPE SqlDate FROM DATETIME
GO
CREATE RULE TimeOnlyRule AS
  datediff(dd,0,@DateTime) = 0
GO
CREATE RULE DateOnlyRule AS
  dateAdd(dd,datediff(dd,0,@DateTime),0) = @DateTime
GO
EXEC sp_bindrule 'TimeOnlyRule', 'SqlTime'
GO
EXEC sp_bindrule 'DateOnlyRule', 'SqlDate'
GO
CREATE TABLE [dbo].[Photo](
	[UID] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RemovalDate] [datetime] NOT NULL,	
	[Data] [varbinary](MAX) NULL,
CONSTRAINT [PK_Photo] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE TABLE [dbo].[AdditionalColumnType](
	[UID] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RemovalDate] [datetime] NOT NULL,	
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[DataType] [int] NULL,
	[PersonType] [int],
	[OrganizationUID] [uniqueidentifier] NULL,
	[IsInGrid] [bit] NOT NULL, 
CONSTRAINT [PK_AdditionalColumnType] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE TABLE [dbo].[AdditionalColumn](
	[UID] [uniqueidentifier] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NULL,
	[AdditionalColumnTypeUID] [uniqueidentifier] NULL,
	[TextData] [text] NULL,
	[PhotoUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_AdditionalColumn] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Day](
	[UID] [uniqueidentifier] NOT NULL,
	[NamedIntervalUID] [uniqueidentifier] NULL,
	[ScheduleSchemeUID] [uniqueidentifier] NOT NULL,
	[Number] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL 
 CONSTRAINT [PK_Day] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Department](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[PhotoUID] [uniqueidentifier] NULL,
	[ParentDepartmentUID] [uniqueidentifier] NULL,
	[ContactEmployeeUID] [uniqueidentifier] NULL,
	[AttendantUID] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Department_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Employee](
	[UID] uniqueidentifier NOT NULL,
	FirstName nvarchar(50) NULL,
	SecondName nvarchar(50) NULL,
	LastName nvarchar(50) NULL,
	PhotoUID uniqueidentifier NULL, 
	PositionUID uniqueidentifier NULL,
	DepartmentUID uniqueidentifier NULL,
	ScheduleUID uniqueidentifier NULL,
	DocumentUID uniqueidentifier NULL,
	Appointed datetime NOT NULL,
	Dismissed datetime NOT NULL,
	[Type] int NULL,
	TabelNo int NOT NULL,
	CredentialsStartDate datetime NOT NULL,
	EscortUID uniqueidentifier NULL,
	IsDeleted bit NOT NULL ,
	RemovalDate datetime NOT NULL ,
	OrganizationUID uniqueidentifier NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[EmployeeDocument](
	[UID] uniqueidentifier NOT NULL,
	Number nvarchar(50) NOT NULL,
	BirthDate datetime NOT NULL,
	BirthPlace nvarchar(MAX) NOT NULL,
	GivenDate datetime NOT NULL,
	GivenBy nvarchar(MAX) NOT NULL,
	ValidTo datetime NOT NULL,
	Gender int NOT NULL,
	DepartmentCode nvarchar(50) NOT NULL,
	Citizenship nvarchar(MAX) NOT NULL,
	[Type] int NOT NULL,
	IsDeleted bit NOT NULL ,
	RemovalDate datetime NOT NULL ,
CONSTRAINT [PK_EmployeeDocument_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[EmployeeReplacement](
	[UID] [uniqueidentifier] NOT NULL,
	[BeginDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NULL,
	[DepartmentUID] [uniqueidentifier] NULL,
	[ScheduleUID] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_EmployeeReplacement] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Holiday](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[TransferDate] [datetime] NULL,
	[Reduction] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Interval](
	[BeginTime] [int] NOT NULL,
	[EndTime] [int] NOT NULL,
	[UID] [uniqueidentifier] NOT NULL,
	[NamedIntervalUID] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
CONSTRAINT [PK_Interval] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[NamedInterval](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[SlideTime] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_NamedInterval_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Phone](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[NumberString] [nvarchar](50) NULL,
	[DepartmentUID] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Phone] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Position](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
	[PhotoUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Schedule](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ScheduleSchemeUID] [uniqueidentifier] NULL,
	[IsIgnoreHoliday] [bit] NOT NULL  DEFAULT 0,
	[IsOnlyFirstEnter] [bit] NOT NULL  DEFAULT 0,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleScheme](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ScheduleScheme] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Document](
	[UID] [uniqueidentifier] NOT NULL,
	[No] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IssueDate] [datetime] NOT NULL,
	[LaunchDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Journal](
	[UID] [uniqueidentifier] NOT NULL,
	[SysemDate] [datetime] NOT NULL,
	[DeviceDate] [datetime] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[DeviceNo] [int] NOT NULL,
	[IpPort] [nvarchar](50) NULL,
	[CardUID] [uniqueidentifier] NULL,
	[CardSeries] [int] NOT NULL,
	[CardNo] [int] NOT NULL,
CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Card](
	[UID] [uniqueidentifier] NOT NULL,
	[Series] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NULL,
	[AccessTemplateUID] [uniqueidentifier] NULL,
	[CardType] [int] NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsInStopList] [bit] NOT NULL,
	[StopReason] [text] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	CardTemplateUID [uniqueidentifier] NULL,
CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[CardZoneLink](
	[UID] [uniqueidentifier] NOT NULL,
	[ZoneUID] [uniqueidentifier] NOT NULL,
	[ParentUID] [uniqueidentifier] NULL,
	[ParentType] [int] NULL,
	[IsWithEscort] [bit] NOT NULL,
	[IsAntipass] [bit] NOT NULL,
	[IntervalUID] [uniqueidentifier] NULL,
	[IntervalType] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_CardZoneLink] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[AccessTemplate](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_AccessTemplate] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleZoneLink](
	[UID] [uniqueidentifier] NOT NULL,
	[ZoneUID] [uniqueidentifier] NULL,
	[ScheduleUID] [uniqueidentifier] NULL,
	[IsControl] [bit] NOT NULL DEFAULT 0,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_ScheduleZoneLink] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Organization](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[PhotoUID] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[OrganizationZone](
	[UID] [uniqueidentifier] NOT NULL,
	[ZoneUID] [uniqueidentifier] NOT NULL,
	[OrganizationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganizationZone] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[PassJournal](
	[UID] [uniqueidentifier] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[ZoneUID] [uniqueidentifier] NULL,
	[EntryTime] [datetime] NULL,
	[ExitTime] [datetime] NULL,
 CONSTRAINT [PK_PassJournal] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE INDEX IntervalUIDIndex ON [dbo].[Interval]([UID])
CREATE INDEX NamedIntervalUIDIndex ON NamedInterval([UID])
CREATE INDEX DayUIDIndex ON [Day]([UID])
CREATE INDEX ScheduleSchemeUIDIndex ON ScheduleScheme([UID])
CREATE INDEX ScheduleUIDIndex ON Schedule([UID])
CREATE INDEX AdditionalColumnUIDIndex ON AdditionalColumn([UID])
CREATE INDEX AdditionalColumnTypeUIDIndex ON AdditionalColumnType([UID])
CREATE INDEX PositionUIDIndex ON Position([UID])
CREATE INDEX EmployeeUIDIndex ON Employee([UID])
CREATE INDEX EmployeeReplacementUIDIndex ON EmployeeReplacement([UID])
CREATE INDEX PhoneUIDIndex ON Phone([UID])
CREATE INDEX DepartmentUIDIndex ON Department([UID])
CREATE INDEX DocumentUIDIndex ON [Document]([UID])
CREATE INDEX HolidayUIDIndex ON Holiday([UID])
CREATE INDEX JournalUIDIndex ON Journal([UID])
CREATE INDEX CardUIDIndex ON Card([UID])
CREATE INDEX CardZoneLinkUIDIndex ON CardZoneLink([UID])
CREATE INDEX ScheduleZoneLinkUIDIndex ON ScheduleZoneLink([UID])
CREATE INDEX OrganizationUIDIndex ON Organization([UID])
CREATE INDEX PhotoUIDIndex ON Photo([UID])
CREATE INDEX PassJournalUIDIndex ON PassJournal([UID])