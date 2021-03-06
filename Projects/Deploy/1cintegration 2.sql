USE [Journal_1]
GO
/****** Object:  UserDefinedFunction [dbo].[z_worktime]    Script Date: 24.04.2015 13:46:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[z_worktime]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[z_worktime]
GO
/****** Object:  UserDefinedFunction [dbo].[f_getkluch]    Script Date: 24.04.2015 13:46:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_getkluch]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[f_getkluch]
GO
/****** Object:  UserDefinedFunction [dbo].[f_getioflag]    Script Date: 24.04.2015 13:46:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_getioflag]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[f_getioflag]
GO
/****** Object:  UserDefinedFunction [dbo].[f_datefilter]    Script Date: 24.04.2015 13:46:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_datefilter]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[f_datefilter]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_isValidDoor]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[f_isValidDoor]
GO
/****** Object:  UserDefinedFunction [dbo].[f_datefilter]    Script Date: 24.04.2015 13:14:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexey L. Oleynichenko
-- Create date: 22.04.2015
-- Description:	1c integration
-- =============================================
CREATE FUNCTION [dbo].[f_datefilter]
(
	@dt datetime,
	@mask varchar(8)
)
RETURNS bit
AS
BEGIN
	DECLARE @ret bit = 0;
	
	DECLARE @year int = YEAR(@dt);
	DECLARE @month int = MONTH(@dt);
	DECLARE @day int = DAY(@dt);
	
	DECLARE @maskYear varchar(4) = '';
	DECLARE @maskMonth varchar(2) = '';
	DECLARE @maskDay varchar(2) = '';

	IF LEN(@mask) = 4
		SET @maskYear = @mask;
		IF (CONVERT(int, @maskYear) = @year)
			SET @ret = 1;
	ELSE IF LEN(@mask) = 6
	BEGIN
		SET @maskYear = SUBSTRING(@mask, 1, 4);
		SET @maskMonth = SUBSTRING(@mask, 5, 2);
		
		IF ((CONVERT(int, @maskYear) = @year) AND (CONVERT(int, @maskMonth) = @month))
			SET @ret = 1;
	END
	ELSE IF LEN(@mask) = 8
	BEGIN
		SET @maskYear = SUBSTRING(@mask, 1, 4);
		SET @maskMonth = SUBSTRING(@mask, 5, 2);
		SET @maskDay = SUBSTRING(@mask, 7, 2);
		
		IF ((CONVERT(int, @maskYear) = @year) AND (CONVERT(int, @maskMonth) = @month) AND (CONVERT(int, @maskDay) = @day))
			SET @ret = 1;
	END

	RETURN @ret;
END

GO
/****** Object:  UserDefinedFunction [dbo].[f_getioflag]    Script Date: 24.04.2015 13:14:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexey L. Oleynichenko
-- Create date: 21.04.2015
-- Description:	1c integration
-- =============================================
CREATE FUNCTION [dbo].[f_getioflag](@expr int)
RETURNS varchar(1)
AS
BEGIN
	DECLARE @ret varchar(1) = '';

	IF @expr = 309
		SELECT @ret = 'I';
	ELSE IF @expr = 310
		SELECT @ret = 'O';

	RETURN @ret
END

GO
/****** Object:  UserDefinedFunction [dbo].[f_getkluch]    Script Date: 24.04.2015 13:14:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexey L. Oleynichenko
-- Create date: 21.04.2015
-- Description:	1c integration
-- =============================================
CREATE FUNCTION [dbo].[f_getkluch](@expr varchar(max))
RETURNS varchar(12)
AS
BEGIN
	DECLARE @ret varchar(12) = '';
	DECLARE @i int;
	
	SET @i = CHARINDEX('$%Номер карты%', @expr);
	IF @i > 0
	BEGIN
		SET @expr = RIGHT(@expr, LEN(@expr) - @i - 14);
		SET @i = CHARINDEX('%$', @expr);
		IF @i > 0
		BEGIN
			SET @expr = LEFT(@expr, @i - 1);
		END
		SELECT @ret = @expr;
	END

	RETURN @ret
END
GO
CREATE FUNCTION [dbo].[f_isValidDoor](@expr varchar(max))
RETURNS int
AS
BEGIN
	DECLARE @result int = 0;
	
	IF CHARINDEX('1281.Турникет 1', @expr) > 0
		SET @result = 1;
	ELSE IF CHARINDEX('1282.Турникет 2', @expr) > 0
		SET @result = 1;
	RETURN @result 
END

GO
/****** Object:  UserDefinedFunction [dbo].[z_worktime]    Script Date: 24.04.2015 13:14:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexey L. Oleynichenko
-- Create date: 23.04.2015
-- Description:	1c integration
-- =============================================
CREATE FUNCTION [dbo].[z_worktime] 
(
	@dt varchar(8)
)
RETURNS @tmp TABLE (
		DCREATE varchar(10),
		TCREATE varchar(8),
		IOFLAG varchar(1),
		KLUCH varchar(36),
		FIO varchar(300),
		NAMEROOM varchar(100)
		)
AS
BEGIN
	-- Empty GUID
	DECLARE @GUID_Empty varchar(36) = '00000000-0000-0000-0000-000000000000';

	INSERT INTO @tmp (DCREATE, TCREATE, IOFLAG, KLUCH, FIO, NAMEROOM)
	SELECT CONVERT(varchar(10), SystemDate, 120), CONVERT(varchar(8), SystemDate, 108), dbo.f_getioflag(Description), EmployeeUID, UserName, ObjectName
	FROM dbo.Journal
	WHERE (EmployeeUID <> @GUID_Empty) AND (Name = 181) AND (dbo.f_datefilter(SystemDate, @dt) = 1) AND (dbo.f_isValidDoor(ObjectName) = 1)
	ORDER BY SystemDate
	
	RETURN 
END

GO
