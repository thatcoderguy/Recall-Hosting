USE [emailinterfacealpha]
GO
/****** Object:  Table [dbo].[tblDomain]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDomain](
	[domainid] [bigint] IDENTITY(1,1) NOT NULL,
	[domainname] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_tblDomain] PRIMARY KEY CLUSTERED 
(
	[domainid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAutoResponder]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAutoResponder](
	[autoresponderID] [bigint] IDENTITY(1,1) NOT NULL,
	[emailID] [bigint] NOT NULL,
	[autoResponderSubject] [nvarchar](150) NOT NULL,
	[autoResponserBodyText] [nvarchar](4000) NOT NULL,
	[autoResponderStatus] [bit] NOT NULL,
 CONSTRAINT [PK_tblAutoResponder] PRIMARY KEY CLUSTERED 
(
	[autoresponderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAliasDomain]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAliasDomain](
	[aliasDomainID] [bigint] IDENTITY(1,1) NOT NULL,
	[aliasDomain] [nvarchar](200) NOT NULL,
	[domainID] [bigint] NOT NULL,
 CONSTRAINT [PK_tblAliasDomain] PRIMARY KEY CLUSTERED 
(
	[aliasDomainID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAliasAddress]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAliasAddress](
	[aliasaddressid] [bigint] IDENTITY(1,1) NOT NULL,
	[domainID] [bigint] NOT NULL,
	[mailboxID] [bigint] NOT NULL,
	[aliasAddress] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_tblAliasAddress] PRIMARY KEY CLUSTERED 
(
	[aliasaddressid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_getdomain]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getdomain]
	-- Add the parameters for the stored procedure here
	@strDomain nvarchar(150)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT domainid, domainName 
	FROM tblDomain
	WHERE domainname LIKE '%'+@strDomain+'%'
	ORDER BY domainName
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getAutoResponder]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getAutoResponder]
	@intEmailID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT autoResponderStatus,autoResponserBodyText,autoResponderSubject
    FROM tblAutoResponder 
    WHERE emailID=@intEmailID
    
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getaliasdomains]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getaliasdomains]
	@intDomainID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT aliasDomainID, aliasDomain, domainID
	FROM tblAliasDomain
	WHERE domainID=@intDomainID 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getaliasaddresses]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getaliasaddresses]
	@intDomainID bigint,
	@intMailboxID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT aliasaddressid,mailboxid,domainid,aliasAddress
	FROM tblAliasAddress
	WHERE domainID=@intDomainID AND mailboxID=@intMailboxID 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deletealiasdomain]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_deletealiasdomain]
	@intAliasDomainID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM tblAliasDomain WHERE aliasDomainID=@intAliasDomainID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deletealiasaddress]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_deletealiasaddress]
	@intDomainID bigint,
	@intMailboxID bigint,
	@intAliasAddressID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM tblAliasAddress
	WHERE domainID=@intDomainID AND mailboxID=@intMailboxID AND aliasaddressid=@intAliasAddressID 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createaliasdomain]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createaliasdomain]
	@strAliasDomain nvarchar(200),
	@intDomainID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblAliasDomain(domainID,aliasDomain) values (@intDomainID,@strAliasDomain)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createaliasaddress]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createaliasaddress]
	@intDomainID bigint,
	@intMailboxID bigint,
	@strAliasAddress nvarchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblAliasAddress(mailboxid,domainid,aliasaddress)
	VALUES (@intMailboxID,@intDomainID,@strAliasAddress)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_alterAutoResponser]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_alterAutoResponser]
	@intEmailID bigint,
	@strSubject nvarchar(150),
	@strBodyText nvarchar(4000),
	@blnStatus bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM tblAutoResponder WHERE emailID=@intEmailID 
    
    INSERT INTO tblAutoResponder(autoResponderSubject,autoResponserBodyText,autoResponderStatus,emailID)
    VALUES (@strSubject,@strBodyText,@blnStatus,@intEmailID)
    
END
GO
/****** Object:  Table [dbo].[tblEmail]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblEmail](
	[emailID] [bigint] IDENTITY(1,1) NOT NULL,
	[emailName] [nvarchar](50) NOT NULL,
	[domainID] [bigint] NOT NULL,
	[emailType] [nvarchar](50) NOT NULL,
	[emailSize] [int] NOT NULL,
 CONSTRAINT [PK_tblEmail] PRIMARY KEY CLUSTERED 
(
	[emailID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_createdomain]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createdomain] 
	-- Add the parameters for the stored procedure here
	@strDomain nvarchar(150)
AS
BEGIN

	DECLARE @intDomainID bigint
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT @intDomainID=MIN(domainID) FROM tblDomain WHERE domainname=@strDomain
	
	IF @intDomainID=0 OR @intDomainID IS NULL BEGIN
	
		INSERT INTO tblDomain (domainname) VALUES (@strDomain)
	
		SELECT 1 as domainID,'' as domainName
		SELECT @intDomainID=SCOPE_IDENTITY()
		
		SELECT @intDomainID as domainID,@strDomain as domainName FROM tblDomain  WHERE domainid=@intDomainID
		
		INSERT INTO tblEmail(domainID,emailName,emailType,emailSize)
		VALUES (@intDomainID,'postmaster','Email',50)
		
	END ELSE BEGIN
	
		SELECT 0 as domainID,'' as domainName
		SELECT @intDomainID as domainID, @strDomain as domainName
		
	END
	
END
GO
/****** Object:  Table [dbo].[tblRetreiver]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRetreiver](
	[redirectorID] [bigint] IDENTITY(1,1) NOT NULL,
	[emailID] [bigint] NOT NULL,
	[popAddress] [nvarchar](50) NOT NULL,
	[popUsername] [nvarchar](250) NOT NULL,
	[popPassword] [nvarchar](50) NOT NULL,
	[status] [int] NOT NULL,
 CONSTRAINT [PK_tblRetreiver] PRIMARY KEY CLUSTERED 
(
	[redirectorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRedirector]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRedirector](
	[redirectorID] [bigint] IDENTITY(1,1) NOT NULL,
	[emailID] [bigint] NOT NULL,
	[redirectorEmail] [nvarchar](250) NOT NULL,
	[status] [int] NOT NULL,
 CONSTRAINT [PK_tblRedirector] PRIMARY KEY CLUSTERED 
(
	[redirectorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblForwarder]    Script Date: 04/06/2014 20:51:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblForwarder](
	[forwarderID] [bigint] IDENTITY(1,1) NOT NULL,
	[emailID] [bigint] NOT NULL,
	[forwarderEmail] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_tblForwarder] PRIMARY KEY CLUSTERED 
(
	[forwarderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_getemails]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getemails]
	-- Add the parameters for the stored procedure here
	@intDomainID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select domainName
	FROM tblDomain 
	WHERE domainid=@intDomainID

	SELECT emailID, emailName, domainName, emailType, emailSize
	FROM tblEmail e
	INNER JOIN tblDomain d ON e.domainID=d.domainid
	WHERE e.domainID=@intDomainID 
	ORDER BY emailName
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getemailaddress]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getemailaddress]
	-- Add the parameters for the stored procedure here
	@intEmailID as bigint,
	@intDomainID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT emailName + '@' + domainName as emailAddress, emailType, emailSize
	FROM tblEmail e
	INNER JOIN tblDomain d ON d.domainid=e.domainID
	WHERE emailID=@intEmailID AND d.domainid=@intDomainID 
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_updatemailboxsize]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_updatemailboxsize] 
	@intMailboxID bigint,
	@intDomainID bigint,
	@intNewEmailSize int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE tblEmail
	SET emailSize=@intNewEmailSize
	WHERE emailID=@intMailboxID AND domainID=@intDomainID
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getredirectdesinations]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getredirectdesinations] 
	@intEmailID as bigint,
	@intDomainID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT redirectorEmail,status,r.emailID,domainID,redirectorID 
    FROM tblRedirector r
    INNER JOIN tblEmail e ON e.emailID=r.emailID
    WHERE r.emailID=@intEmailID AND e.domainID=@intDomainID 
    
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getpopretrievers]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getpopretrievers] 
	@intEmailID as bigint,
	@intDomainID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT popAddress,popPassword,popUsername,status,r.emailID,domainID,redirectorID
    FROM tblRetreiver r
    INNER JOIN tblEmail e ON e.emailID=r.emailID
    WHERE r.emailID=@intEmailID AND e.domainID=@intDomainID 
    
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getpopretriever]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getpopretriever] 
	@intEmailID as bigint,
	@intRetrieverID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT popAddress,popPassword,popUsername,status,r.emailID,domainID,redirectorID
    FROM tblRetreiver r
    INNER JOIN tblEmail e ON e.emailID=r.emailID
    WHERE r.emailID=@intEmailID and r.redirectorID=@intRetrieverID
    
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getforwarddestinations]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getforwarddestinations] 
	@intEmailID as bigint,
	@intDomainID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT forwarderEmail,f.emailID,domainID,forwarderID 
    FROM tblForwarder f
    INNER JOIN tblEmail e ON e.emailID=f.emailID
    WHERE f.emailID=@intEmailID AND e.domainID=@intDomainID 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createredirector]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createredirector]
	@intEmailID as bigint,
	@strDestination as nvarchar(250),
	@intStatus as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblRedirector(emailID,redirectorEmail,status) VALUES (@intEmailID,@strDestination,@intStatus)
	
	UPDATE tblRedirector SET status=@intStatus WHERE emailID=@intEmailID 
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createpopretriever]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createpopretriever]
	@intEmailID as bigint,
	@strPOPAddress as nvarchar(250),
	@strPOPUsername as nvarchar(250),
	@strPOPPassword as nvarchar(250),
	@intStatus as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblRetreiver(emailID,popAddress,popUsername,popPassword,status) VALUES (@intEmailID,@strPOPAddress,@strPOPUsername,@strPOPPassword,@intStatus)
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createforwarder]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createforwarder]
	@intEmailID as bigint,
	@strDestination as nvarchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO tblForwarder(emailID,forwarderEmail) VALUES (@intEmailID,@strDestination)
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_createemail]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createemail]
	@strEmailName nvarchar(50),
	@intDomainID bigint,
	@strForwarder nvarchar(250)='',
	@intMailboxSize int
AS
BEGIN

	DECLARE @intEmailID bigint
	DECLARE @emailType nvarchar(50)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT @intEmailID=MIN(emailID) FROM tblEmail WHERE domainID=@intDomainID AND emailName=@strEmailName
	
	IF @intEmailID=0 OR @intEmailID IS NULL BEGIN
	
		IF @strForwarder='' BEGIN
			
			INSERT INTO tblEmail (domainID,emailName,emailType,emailSize) VALUES (@intDomainID,@strEmailName,'Email',@intMailboxSize)
			
			SET @intEmailID= SCOPE_IDENTITY()
			
			SELECT 1 as emailID,'' as emailName,1 as domainID,'' as emailType,@intMailboxSize  as emailSize
			SELECT @intEmailID as emailID, @strEmailName as emailName, @intDomainID as domainID, 'Email' as emailType, @intMailboxSize as emailSize		
		
		END ELSE BEGIN
		
			INSERT INTO tblEmail (domainID,emailName,emailType,emailSize) VALUES (@intDomainID,@strEmailName,'Forwarder',0)
		
			SET @intEmailID= SCOPE_IDENTITY()
			INSERT INTO tblForwarder(emailID,forwarderEmail) VALUES (@intEmailID,@strForwarder)
		
			SELECT 1 as emailID,'' as emailName,1 as domainID,'' as emailType,0 as emailSize
			SELECT @intEmailID as emailID, @strEmailName as emailName, @intDomainID as domainID, 'Forwarder' as emailType, 0 as emailSize
			
		END
	
	END ELSE BEGIN

		SELECT 0 as emailID,'' as emailName,0 as domainID, emailType,emailSize FROM tblEmail WHERE emailID=@intEmailID
		SELECT @intDomainID as domainID, @strEmailName as emailName, @intEmailID as emailID, emailType, emailSize FROM tblEmail WHERE emailID=@intEmailID
			
	END
	
	
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteredirector]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_deleteredirector]
	@intEmailID as bigint,
	@intRedirectorID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM tblRedirector WHERE emailID=@intEmailID AND redirectorID=@intRedirectorID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deletepopretriever]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_deletepopretriever]
	@intEmailID as bigint,
	@intRetrieverID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM tblRetreiver WHERE emailID=@intEmailID AND redirectorID=@intRetrieverID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteforwarder]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_deleteforwarder]
	@intEmailID as bigint,
	@intForwarderID as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @blnLastEmail int

    DELETE FROM tblForwarder WHERE emailID=@intEmailID AND forwarderID=@intForwarderID
    
    SELECT @blnLastEmail = COUNT(emailID) FROM tblForwarder WHERE emailID=@intEmailID 
    
    IF @blnLastEmail=0 BEGIN
    
		DELETE FROM tblEmail WHERE emailID=@intEmailID 
    
    END
    
END
GO
/****** Object:  StoredProcedure [dbo].[sp_deleteemail]    Script Date: 04/06/2014 20:52:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_deleteemail] 
	@intEmailID bigint,
	@intDomainID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM tblForwarder WHERE emailID=@intEmailID
	DELETE FROM tblRetreiver WHERE emailID=@intEmailID
	DELETE FROM tblRedirector WHERE emailID=@intEmailID 

	DELETE FROM tblEmail WHERE emailID=@intEmailID and domainID=@intDomainID 

END
GO
/****** Object:  Default [DF_tblAutoResponder_autoResponderStatus]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblAutoResponder] ADD  CONSTRAINT [DF_tblAutoResponder_autoResponderStatus]  DEFAULT ((0)) FOR [autoResponderStatus]
GO
/****** Object:  Default [DF_tblEmail_type]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblEmail] ADD  CONSTRAINT [DF_tblEmail_type]  DEFAULT (N'Email') FOR [emailType]
GO
/****** Object:  Default [DF_tblEmail_emailSize]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblEmail] ADD  CONSTRAINT [DF_tblEmail_emailSize]  DEFAULT ((1000)) FOR [emailSize]
GO
/****** Object:  Default [DF_tblRedirector_status]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblRedirector] ADD  CONSTRAINT [DF_tblRedirector_status]  DEFAULT ((1)) FOR [status]
GO
/****** Object:  Default [DF_tblRetreiver_status]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblRetreiver] ADD  CONSTRAINT [DF_tblRetreiver_status]  DEFAULT ((1)) FOR [status]
GO
/****** Object:  ForeignKey [FK_tblEmail_tblDomain]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblEmail]  WITH CHECK ADD  CONSTRAINT [FK_tblEmail_tblDomain] FOREIGN KEY([domainID])
REFERENCES [dbo].[tblDomain] ([domainid])
GO
ALTER TABLE [dbo].[tblEmail] CHECK CONSTRAINT [FK_tblEmail_tblDomain]
GO
/****** Object:  ForeignKey [FK_tblForwarder_tblEmail]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblForwarder]  WITH CHECK ADD  CONSTRAINT [FK_tblForwarder_tblEmail] FOREIGN KEY([emailID])
REFERENCES [dbo].[tblEmail] ([emailID])
GO
ALTER TABLE [dbo].[tblForwarder] CHECK CONSTRAINT [FK_tblForwarder_tblEmail]
GO
/****** Object:  ForeignKey [FK_tblRedirector_tblEmail]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblRedirector]  WITH CHECK ADD  CONSTRAINT [FK_tblRedirector_tblEmail] FOREIGN KEY([emailID])
REFERENCES [dbo].[tblEmail] ([emailID])
GO
ALTER TABLE [dbo].[tblRedirector] CHECK CONSTRAINT [FK_tblRedirector_tblEmail]
GO
/****** Object:  ForeignKey [FK_tblRetreiver_tblEmail]    Script Date: 04/06/2014 20:51:44 ******/
ALTER TABLE [dbo].[tblRetreiver]  WITH CHECK ADD  CONSTRAINT [FK_tblRetreiver_tblEmail] FOREIGN KEY([emailID])
REFERENCES [dbo].[tblEmail] ([emailID])
GO
ALTER TABLE [dbo].[tblRetreiver] CHECK CONSTRAINT [FK_tblRetreiver_tblEmail]
GO
