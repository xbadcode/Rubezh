USE [SKD]
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Test')
BEGIN
	INSERT INTO Patches (Id) VALUES ('Test')    
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationUser')
BEGIN
	IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationUser')
	BEGIN
		CREATE TABLE OrganisationUser(
			[UID] [uniqueidentifier] NOT NULL,
			[UserUID] [uniqueidentifier] NOT NULL,
			[OrganisationUID] [uniqueidentifier] NOT NULL,
		CONSTRAINT [PK_OrganisationUser] PRIMARY KEY CLUSTERED 
		(
			[UID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]	
		ALTER TABLE [dbo].[OrganisationUser] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationUser_Organisation] FOREIGN KEY([OrganisationUid])
		REFERENCES [dbo].[Organisation] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[OrganisationUser] NOCHECK CONSTRAINT [FK_OrganisationUser_Organisation]
	END
	INSERT INTO Patches (Id) VALUES ('OrganisationUser')    
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AlterPatches')
BEGIN
	ALTER TABLE Patches ALTER COLUMN Id nvarchar(100) not null
	INSERT INTO Patches (Id) VALUES ('AlterPatches')    
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'GuardZone')
BEGIN
	IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationGuardZone')
	BEGIN
		DROP TABLE OrganisationGuardZone
	END
	IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'GuardZone')
	BEGIN
		CREATE TABLE [dbo].[GuardZone](
			[UID] [uniqueidentifier] NOT NULL,
			[ZoneUID] [uniqueidentifier] NOT NULL,
			[ParentUID] [uniqueidentifier] NOT NULL,
			[CanSet] [bit] NOT NULL,
			[CanReset] [bit] NOT NULL,
		CONSTRAINT [PK_GuardZone] PRIMARY KEY CLUSTERED 
		(
			[UID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
		ALTER TABLE [dbo].[GuardZone] WITH NOCHECK ADD CONSTRAINT [FK_GuardZone_Organisation] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[Organisation] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[GuardZone] NOCHECK CONSTRAINT [FK_GuardZone_Organisation]
		ALTER TABLE [dbo].[GuardZone] WITH NOCHECK ADD CONSTRAINT [FK_GuardZone_Employee] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[Employee] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[GuardZone] NOCHECK CONSTRAINT [FK_GuardZone_Employee]
		ALTER TABLE [dbo].[GuardZone] WITH NOCHECK ADD CONSTRAINT [FK_GuardZone_AccessTemplate] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[AccessTemplate] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[GuardZone] NOCHECK CONSTRAINT [FK_GuardZone_AccessTemplate]
	END
	INSERT INTO Patches (Id) VALUES ('GuardZone')    
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Doors')
BEGIN
	IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationZone')
	BEGIN
		DROP TABLE OrganisationGuardZone
	END
	IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardZone')
	BEGIN
		DROP TABLE OrganisationGuardZone
	END
	IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
	BEGIN
		CREATE TABLE [dbo].[CardDoor](
			[UID] [uniqueidentifier] NOT NULL,
			[DoorUID] [uniqueidentifier] NOT NULL,
			[ParentUID] [uniqueidentifier] NULL,
			[ParentType] [int] NULL,
			[IsWithEscort] [bit] NOT NULL,
			[IsAntipass] [bit] NOT NULL,
			[IntervalUID] [uniqueidentifier] NULL,
			[IntervalType] [int] NULL,
			[IsDeleted] [bit] NOT NULL ,
			[RemovalDate] [datetime] NOT NULL ,
		 CONSTRAINT [PK_CardDoor] PRIMARY KEY CLUSTERED 
		(
			[UID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
		
		ALTER TABLE [dbo].[CardDoor]  WITH NOCHECK ADD  CONSTRAINT [FK_CardDoor_Card] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[Card] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[CardDoor] NOCHECK CONSTRAINT [FK_CardDoor_Card]

		ALTER TABLE [dbo].[CardDoor] WITH NOCHECK ADD  CONSTRAINT [FK_CardDoor_AccessTemplate] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[AccessTemplate] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[CardDoor] NOCHECK CONSTRAINT [FK_CardDoor_AccessTemplate]

		CREATE TABLE [dbo].[OrganisationDoor](
			[UID] [uniqueidentifier] NOT NULL,
			[DoorUID] [uniqueidentifier] NOT NULL,
			[OrganisationUID] [uniqueidentifier] NOT NULL,
		CONSTRAINT [PK_OrganisationZone] PRIMARY KEY CLUSTERED 
		(
			[UID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[OrganisationDoor] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationDoor_Organisation] FOREIGN KEY([OrganisationUid])
		REFERENCES [dbo].[Organisation] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[OrganisationDoor] NOCHECK CONSTRAINT [FK_OrganisationDoor_Organisation]
	END
	INSERT INTO Patches (Id) VALUES ('Doors')    
END
GO