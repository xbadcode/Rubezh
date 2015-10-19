USE [Journal_0]
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'VideoUID')
BEGIN
	ALTER TABLE Journal ADD VideoUID uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('VideoUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CameraUID')
BEGIN
	ALTER TABLE Journal ADD CameraUID uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('CameraUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ErrorCode')
BEGIN
	ALTER TABLE Journal ADD ErrorCode int NULL
	INSERT INTO Patches (Id) VALUES ('ErrorCode')
END
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ControllerUID')
BEGIN
	ALTER TABLE Journal ADD ControllerUID uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('ControllerUID')
END
