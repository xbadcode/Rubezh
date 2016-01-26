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