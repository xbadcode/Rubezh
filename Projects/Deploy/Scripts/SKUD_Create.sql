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
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Day]') AND type in (N'U'))
DROP TABLE [dbo].[Day]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Holiday]') AND type in (N'U'))
DROP TABLE [dbo].[Holiday]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Interval]') AND type in (N'U'))
DROP TABLE [dbo].[Interval]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NamedInterval]') AND type in (N'U'))
DROP TABLE [dbo].[NamedInterval]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Schedule]') AND type in (N'U'))
DROP TABLE [dbo].[Schedule]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleScheme]') AND type in (N'U'))
DROP TABLE [dbo].[ScheduleScheme]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisterDevice]') AND type in (N'U'))
DROP TABLE [dbo].[RegisterDevice]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employee]') AND type in (N'U'))
DROP TABLE [dbo].[Employee]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Department]') AND type in (N'U'))
DROP TABLE [dbo].[Department]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeReplacement]') AND type in (N'U'))
DROP TABLE [dbo].[EmployeeReplacement]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Phone]') AND type in (N'U'))
DROP TABLE [dbo].[Phone]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Document]') AND type in (N'U'))
DROP TABLE [dbo].[Document]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND type in (N'U'))
DROP TABLE [dbo].[Position]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdditionalColumn]') AND type in (N'U'))
DROP TABLE [dbo].[AdditionalColumn]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Journal]') AND type in (N'U'))
DROP TABLE [dbo].[Journal]
GO	
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdditionalColumn](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](50) NULL,
	[TextData] [text] NULL,
	[GraphicsData] [varbinary](MAX) NULL,
	[EmployeeUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_AdditionalColumn] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Day](
	[Uid] [uniqueidentifier] NOT NULL,
	[NamedIntervalUid] [uniqueidentifier] NULL,
	[ScheduleSchemeUid] [uniqueidentifier] NULL,
	[Number] [int] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Day] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Department](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[ParentDepartmentUid] [uniqueidentifier] NULL,
	[ContactEmployeeUid] [uniqueidentifier] NULL,
	[AttendantUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Department_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Employee](
	[Uid] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[SecondName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[PositionUid] [uniqueidentifier] NULL,
	[DepartmentUid] [uniqueidentifier] NULL,
	[ScheduleUid] [uniqueidentifier] NULL,
	[Appointed] [datetime] NULL,
	[Dismissed] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Employee_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[EmployeeReplacement](
	[Uid] [uniqueidentifier] NOT NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[EmployeeUid] [uniqueidentifier] NULL,
	[DepartmentUid] [uniqueidentifier] NULL,
	[ScheduleUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_EmployeeReplacement] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Holiday](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Type] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[TransferDate] [datetime] NULL,
	[Reduction] [int] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Interval](
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Transition] [nvarchar](10) NULL,
	[Uid] [uniqueidentifier] NOT NULL,
	[NamedIntervalUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Interval_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[NamedInterval](
	[Name] [nvarchar](50) NULL,
	[Uid] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_NamedInterval_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Phone](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[NumberString] [nvarchar](50) NULL,
	[DepartmentUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Phone] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Position](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Position_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[RegisterDevice](
	[Uid] [uniqueidentifier] NOT NULL,
	[CanControl] [bit] NULL,
	[ScheduleUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_RegisterDevice] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Schedule](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ScheduleSchemeUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Schedule_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleScheme](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Type] [nvarchar](50) NULL,
	[Length] [int] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_ScheduleScheme_1] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Document](
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IssueDate] [datetime] NULL,
	[LaunchDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[RemovalDate] [datetime] NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Journal](
	[Uid] [uniqueidentifier] NOT NULL,
	[SysemDate] [datetime] NULL,
	[DeviceDate] [datetime] NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[DeviceNo] [int] NULL,
	[IpPort] [nvarchar](50) NULL,
	[CardNo] [int] NULL,
 CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Frame](
	[Uid] [uniqueidentifier] NOT NULL,
	[CameraUid] [uniqueidentifier] NULL,
	[JournalItemUid] [uniqueidentifier] NULL,
	[DateTime] [datetime] NULL,
	[FrameData] [varbinary](MAX) NULL,
 CONSTRAINT [PK_Frame] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE INDEX IntervalUidIndex ON [dbo].[Interval]([Uid])
CREATE INDEX NamedIntervalUidIndex ON NamedInterval([Uid])
CREATE INDEX DayUidIndex ON [Day]([Uid])
CREATE INDEX ScheduleSchemeUidIndex ON ScheduleScheme([Uid])
CREATE INDEX ScheduleUidIndex ON Schedule([Uid])
CREATE INDEX RegisterDeviceUidIndex ON RegisterDevice([Uid])
CREATE INDEX AdditionalColumnUidIndex ON AdditionalColumn([Uid])
CREATE INDEX PositionUidIndex ON Position([Uid])
CREATE INDEX EmployeeUidIndex ON Employee([Uid])
CREATE INDEX EmployeeReplacementUidIndex ON EmployeeReplacement([Uid])
CREATE INDEX PhoneUidIndex ON Phone([Uid])
CREATE INDEX DepartmentUidIndex ON Department([Uid])
CREATE INDEX DocumentUidIndex ON [Document]([Uid])
CREATE INDEX HolidayUidIndex ON Holiday([Uid])
CREATE INDEX JournalUidIndex ON Journal([Uid])
CREATE INDEX FrameUidIndex ON Frame([Uid])

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[AdditionalColumn]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalColumn_Employee] FOREIGN KEY([EmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
GO
ALTER TABLE [dbo].[AdditionalColumn] CHECK CONSTRAINT [FK_AdditionalColumn_Employee]
GO
ALTER TABLE [dbo].[Day]  WITH NOCHECK ADD  CONSTRAINT [FK_Day_NamedInterval] FOREIGN KEY([NamedIntervalUid])
REFERENCES [dbo].[NamedInterval] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Day] NOCHECK CONSTRAINT [FK_Day_NamedInterval]
GO
ALTER TABLE [dbo].[Day]  WITH CHECK ADD  CONSTRAINT [FK_Day_ScheduleScheme] FOREIGN KEY([ScheduleSchemeUid])
REFERENCES [dbo].[ScheduleScheme] ([Uid])
GO
ALTER TABLE [dbo].[Day] CHECK CONSTRAINT [FK_Day_ScheduleScheme]
GO
ALTER TABLE [dbo].[Department]  WITH CHECK ADD  CONSTRAINT [FK_Department_Department1] FOREIGN KEY([ParentDepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
GO
ALTER TABLE [dbo].[Department] CHECK CONSTRAINT [FK_Department_Department1]
GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Department1] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
ON UPDATE CASCADE
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Department1]
GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Position] FOREIGN KEY([PositionUid])
REFERENCES [dbo].[Position] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Position]
GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Schedule]
GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeReplacement_Department] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
GO
ALTER TABLE [dbo].[EmployeeReplacement] CHECK CONSTRAINT [FK_EmployeeReplacement_Department]
GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeReplacement_Employee] FOREIGN KEY([EmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
GO
ALTER TABLE [dbo].[EmployeeReplacement] CHECK CONSTRAINT [FK_EmployeeReplacement_Employee]
GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeReplacement_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
GO
ALTER TABLE [dbo].[EmployeeReplacement] CHECK CONSTRAINT [FK_EmployeeReplacement_Schedule]
GO
ALTER TABLE [dbo].[Interval]  WITH NOCHECK ADD  CONSTRAINT [FK_Interval_NamedInterval1] FOREIGN KEY([NamedIntervalUid])
REFERENCES [dbo].[NamedInterval] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Interval] NOCHECK CONSTRAINT [FK_Interval_NamedInterval1]
GO
ALTER TABLE [dbo].[Phone]  WITH CHECK ADD  CONSTRAINT [FK_Phone_Department] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
GO
ALTER TABLE [dbo].[Phone] CHECK CONSTRAINT [FK_Phone_Department]
GO
ALTER TABLE [dbo].[RegisterDevice]  WITH NOCHECK ADD  CONSTRAINT [FK_RegisterDevice_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[RegisterDevice] NOCHECK CONSTRAINT [FK_RegisterDevice_Schedule]
GO
ALTER TABLE [dbo].[Schedule]  WITH NOCHECK ADD  CONSTRAINT [FK_Schedule_ScheduleScheme] FOREIGN KEY([ScheduleSchemeUid])
REFERENCES [dbo].[ScheduleScheme] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Schedule] NOCHECK CONSTRAINT [FK_Schedule_ScheduleScheme]
GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD  CONSTRAINT [FK_Department_Employee1] FOREIGN KEY([ContactEmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Employee1]
GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD  CONSTRAINT [FK_Department_Employee2] FOREIGN KEY([AttendantUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Employee2]
GO
ALTER TABLE [dbo].[Frame]  WITH NOCHECK ADD  CONSTRAINT [FK_Frame_Journal] FOREIGN KEY([JournalItemUid])
REFERENCES [dbo].[Journal] ([Uid])
ON UPDATE SET NULL
ON DELETE SET NULL
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Frame] NOCHECK CONSTRAINT [FK_Frame_Journal]