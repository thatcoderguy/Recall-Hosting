USE [workhorse]
GO
/****** Object:  Table [dbo].[tblLog]    Script Date: 04/06/2014 20:52:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblLog](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[commandID] [bigint] NOT NULL,
	[commandName] [nvarchar](500) NOT NULL,
	[dataDump] [nvarchar](max) NOT NULL,
	[actionDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tblLog] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblError]    Script Date: 04/06/2014 20:52:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblError](
	[errorID] [bigint] IDENTITY(1,1) NOT NULL,
	[accountID] [bigint] NULL,
	[errorDate] [datetime] NOT NULL,
	[errorDescription] [nvarchar](max) NOT NULL,
	[errorCode] [int] NULL,
	[errorLine] [int] NULL,
	[errorFunction] [nvarchar](50) NULL,
	[errorClass] [nvarchar](50) NULL,
	[errorCommandID] [bigint] NULL,
 CONSTRAINT [PK_tblError] PRIMARY KEY CLUSTERED 
(
	[errorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommandQueue]    Script Date: 04/06/2014 20:52:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommandQueue](
	[commandID] [bigint] IDENTITY(1,1) NOT NULL,
	[accountID] [bigint] NOT NULL,
	[domainName] [nvarchar](500) NOT NULL,
	[commandName] [nvarchar](500) NOT NULL,
	[requestdatetime] [datetime] NOT NULL,
	[failcount] [int] NOT NULL,
	[inuse] [bit] NOT NULL,
	[parameter1] [nvarchar](max) NULL,
	[parameter2] [nvarchar](max) NULL,
	[parameter3] [nvarchar](max) NULL,
	[parameter4] [nvarchar](max) NULL,
	[parameter5] [nvarchar](max) NULL,
	[parameter6] [nvarchar](max) NULL,
	[parameter7] [nvarchar](max) NULL,
	[parameter8] [nvarchar](max) NULL,
	[parameter9] [nvarchar](max) NULL,
	[parameter10] [nvarchar](max) NULL,
	[parameter11] [nvarchar](max) NULL,
	[parameter12] [nvarchar](max) NULL,
	[parameter13] [nvarchar](max) NULL,
	[parameter14] [nvarchar](max) NULL,
	[parameter15] [nvarchar](max) NULL,
	[parameter16] [nvarchar](max) NULL,
	[parameter17] [nvarchar](max) NULL,
	[parameter18] [nvarchar](max) NULL,
	[parameter19] [nvarchar](max) NULL,
	[parameter20] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblCommandQueue] PRIMARY KEY CLUSTERED 
(
	[commandID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCommandArchive]    Script Date: 04/06/2014 20:52:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommandArchive](
	[commandID] [bigint] NOT NULL,
	[accountID] [bigint] NOT NULL,
	[domainName] [nvarchar](500) NOT NULL,
	[commandName] [nvarchar](500) NOT NULL,
	[requestdatetime] [datetime] NOT NULL,
	[completedatetime] [datetime] NOT NULL,
	[parameter1] [nvarchar](max) NULL,
	[parameter2] [nvarchar](max) NULL,
	[parameter3] [nvarchar](max) NULL,
	[parameter4] [nvarchar](max) NULL,
	[parameter5] [nvarchar](max) NULL,
	[parameter6] [nvarchar](max) NULL,
	[parameter7] [nvarchar](max) NULL,
	[parameter8] [nvarchar](max) NULL,
	[parameter9] [nvarchar](max) NULL,
	[parameter10] [nvarchar](max) NULL,
	[parameter11] [nvarchar](max) NULL,
	[parameter12] [nvarchar](max) NULL,
	[parameter13] [nvarchar](max) NULL,
	[parameter14] [nvarchar](max) NULL,
	[parameter15] [nvarchar](max) NULL,
	[parameter16] [nvarchar](max) NULL,
	[parameter17] [nvarchar](max) NULL,
	[parameter18] [nvarchar](max) NULL,
	[parameter19] [nvarchar](max) NULL,
	[parameter20] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblCommandArchive] PRIMARY KEY CLUSTERED 
(
	[commandID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_inserterror]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_inserterror]
	@strErrorDescription nvarchar(max),
	@intCommandID bigint=0,
	@intAccountID bigint=0,
	@intErrorCode int=0,
	@intErrorLine int=0,
	@strErrorFunction nvarchar(50)='',
	@strErrorClass nvarchar(50)=''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblError(accountID,errorClass,errorCode,errorCommandID,errorDate,errorFunction,errorLine,errorDescription)
	VALUES (@intAccountID,@strErrorClass,@intErrorCode,@intCommandID,GETDATE(),@strErrorFunction,@intErrorLine,@strErrorDescription)
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_insertcommand]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_insertcommand]
	@intAccountID bigint,
	@strDomainName nvarchar(500),
	@strCommandName nvarchar(500),
	@strParam1 nvarchar(max),
	@strParam2 nvarchar(max)='',
	@strParam3 nvarchar(max)='',
	@strParam4 nvarchar(max)='',
	@strParam5 nvarchar(max)='',
	@strParam6 nvarchar(max)='',
	@strParam7 nvarchar(max)='',
	@strParam8 nvarchar(max)='',
	@strParam9 nvarchar(max)='',
	@strParam10 nvarchar(max)='',
	@strParam11 nvarchar(max)='',
	@strParam12 nvarchar(max)='',
	@strParam13 nvarchar(max)='',
	@strParam14 nvarchar(max)='',
	@strParam15 nvarchar(max)='',
	@strParam16 nvarchar(max)='',
	@strParam17 nvarchar(max)='',
	@strParam18 nvarchar(max)='',
	@strParam19 nvarchar(max)='',
	@strParam20 nvarchar(max)=''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO tblCommandQueue (accountID,domainName,commandName,parameter1,parameter2,parameter3,parameter4,
								parameter5, parameter6, parameter7, parameter8, parameter9, parameter10,
								parameter11,parameter12,parameter13,parameter14,parameter15,parameter16,
								parameter17,parameter18,parameter19,parameter20,requestdatetime,inuse,failcount)
VALUES (@intAccountID,@strCommandName,@strDomainName,@strParam1,@strParam2,@strParam3,@strParam4,@strParam5,
		@strParam6,@strParam7,@strParam8,@strParam9,@strParam10,@strParam11,@strParam12,@strParam13,@strParam14,
		@strParam15,@strParam16,@strParam17,@strParam18,@strParam19,@strParam20,getdate(),0,0)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getNumberOfQueuedCommands]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getNumberOfQueuedCommands] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT COUNT(*) as queuedcommands FROM tblCommandQueue WHERE inuse=0
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getNextCommand]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getNextCommand]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--this is to stop concurrency issues with grabbing commands
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

	--lock the table
	BEGIN TRANSACTION

	DECLARE @intCommandID bigint
	
	--FIFO (First In, First Out)
	SELECT @intCommandID=MIN(commandID) FROM tblCommandQueue WHERE inuse=0
	
	UPDATE tblCommandQueue SET inuse=1 WHERE commandID=@intCommandID
	
	--unlock the table
	COMMIT TRANSACTION
	
	SET NOCOUNT OFF
	
	SELECT commandID as intCommandID,commandName,accountID, domainName,isnull(parameter1,'') as parameter1, 
				isnull(parameter2,'') as parameter2, isnull(parameter3,'') as parameter3, 
				isnull(parameter4,'') as parameter4, isnull(parameter5,'') as parameter5, 
				isnull(parameter6,'') as parameter6, isnull(parameter7,'') as parameter7,
				isnull(parameter8,'') as parameter8, isnull(parameter9,'') as parameter9, 
				isnull(parameter10,'') as parameter10, isnull(parameter11,'') as parameter11, 
				isnull(parameter12,'') as parameter12, isnull(parameter13,'') as parameter13,
				isnull(parameter14,'') as parameter14, isnull(parameter15,'') as parameter15, 
				isnull(parameter16,'') as parameter16, isnull(parameter17,'') as parameter17, 
				isnull(parameter18,'') as parameter18, isnull(parameter19,'') as parameter19, 
				isnull(parameter20,'') as parameter20
				FROM tblCommandQueue WHERE commandID=@intCommandID
	
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createlogentry]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createlogentry] 
	@intCommandID bigint,
	@strCommandName nvarchar(500),
	@strDataDump nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblLog (commandID,commandName,dataDump,actionDate)
	VALUES (@intCommandID,@strCommandName,@strDataDump,GETDATE())
	
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_commandfailed]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_commandfailed] 
	@intCommandID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @intFailCount int
	
	--update the command's failcount
	UPDATE tblCommandQueue 
	SET failcount=failcount+1
	WHERE commandID=@intCommandID 
	
	--find out how many times the command has failed
	SELECT @intFailCount=failcount FROM tblCommandQueue WHERE commandID=@intCommandID 

	--if it has failed 3 times, then archive it
	IF @intFailCount=3 BEGIN
	
		INSERT INTO tblCommandArchive (commandID,accountID,domainName,commandName,requestdatetime,completedatetime,
					parameter1, parameter2, parameter3, parameter4, parameter5, parameter6,
					parameter7, parameter8, parameter9, parameter10, parameter11, parameter12,
					parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, 
					parameter19, parameter20)
		
		SELECT commandID,accountID,domainName,commandName,requestdatetime,GETDATE(),
				parameter1, parameter2, parameter3, parameter4, parameter5, parameter6,
				parameter7, parameter8, parameter9, parameter10, parameter11, parameter12,
				parameter13, parameter14, parameter15, parameter16, parameter17, parameter18,
				parameter19, parameter20 FROM tblCommandQueue WHERE commandID=@intCommandID and failcount=3
				
		DELETE FROM tblCommandQueue WHERE commandID=@intCommandID and failcount=3
		
	--otherwise allow another process to use
	END ELSE BEGIN
	
			UPDATE tblCommandQueue 
			SET inuse=0
			WHERE commandID=@intCommandID 
	
	END

END
GO
/****** Object:  StoredProcedure [dbo].[sp_archivecommand]    Script Date: 04/06/2014 20:52:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_archivecommand] 
	@intCommandID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblCommandArchive (commandID,accountID,domainName,commandName,requestdatetime,completedatetime,
				parameter1, parameter2, parameter3, parameter4, parameter5, parameter6,
				parameter7, parameter8, parameter9, parameter10, parameter11, parameter12,
				parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, 
				parameter19, parameter20)
	
	SELECT commandID,accountID,domainName,commandName,requestdatetime,GETDATE(),
			parameter1, parameter2, parameter3, parameter4, parameter5, parameter6,
			parameter7, parameter8, parameter9, parameter10, parameter11, parameter12,
			parameter13, parameter14, parameter15, parameter16, parameter17, parameter18,
			parameter19, parameter20 FROM tblCommandQueue WHERE commandID=@intCommandID 
			
	DELETE FROM tblCommandQueue WHERE commandID=@intCommandID

END
GO
/****** Object:  Default [DF_tblCommandQueue_requestdatetime]    Script Date: 04/06/2014 20:52:27 ******/
ALTER TABLE [dbo].[tblCommandQueue] ADD  CONSTRAINT [DF_tblCommandQueue_requestdatetime]  DEFAULT (getdate()) FOR [requestdatetime]
GO
