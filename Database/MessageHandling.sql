USE [master]
GO
/****** Object:  Database [MessageHandling]    Script Date: 2019/06/24 11:08:48 ******/
CREATE DATABASE [MessageHandling]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MessageHandling', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\MessageHandling.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MessageHandling_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\MessageHandling_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [MessageHandling] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MessageHandling].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MessageHandling] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MessageHandling] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MessageHandling] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MessageHandling] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MessageHandling] SET ARITHABORT OFF 
GO
ALTER DATABASE [MessageHandling] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MessageHandling] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MessageHandling] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MessageHandling] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MessageHandling] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MessageHandling] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MessageHandling] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MessageHandling] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MessageHandling] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MessageHandling] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MessageHandling] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MessageHandling] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MessageHandling] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MessageHandling] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MessageHandling] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MessageHandling] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MessageHandling] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MessageHandling] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MessageHandling] SET  MULTI_USER 
GO
ALTER DATABASE [MessageHandling] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MessageHandling] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MessageHandling] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MessageHandling] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [MessageHandling] SET DELAYED_DURABILITY = DISABLED 
GO
USE [MessageHandling]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 2019/06/24 11:08:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[username] [varchar](20) NOT NULL,
	[password] [varchar](200) NULL,
	[status] [varchar](100) NOT NULL,
	[role] [varchar](12) NOT NULL,
	[imageurl] [varchar](100) NULL,
	[name] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountGroup]    Script Date: 2019/06/24 11:08:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountGroup](
	[username] [varchar](20) NOT NULL,
	[groupid] [int] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Chat]    Script Date: 2019/06/24 11:08:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chat](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[lasttext] [varchar](max) NOT NULL,
	[lastmessagedate] [datetime] NOT NULL,
	[isgroup] [bit] NOT NULL,
	[groupid] [int] NULL,
	[senderusername] [varchar](20) NOT NULL,
	[receiverusername] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 2019/06/24 11:08:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[creatorusername] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Message]    Script Date: 2019/06/24 11:08:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Message](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[text] [varchar](max) NOT NULL,
	[datesent] [datetime] NOT NULL,
	[groupsid] [int] NULL,
	[sender] [varchar](20) NOT NULL,
	[receiver] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccountGroup]  WITH CHECK ADD FOREIGN KEY([groupid])
REFERENCES [dbo].[Groups] ([id])
GO
ALTER TABLE [dbo].[AccountGroup]  WITH CHECK ADD FOREIGN KEY([username])
REFERENCES [dbo].[Account] ([username])
GO
ALTER TABLE [dbo].[Chat]  WITH CHECK ADD FOREIGN KEY([senderusername])
REFERENCES [dbo].[Account] ([username])
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD FOREIGN KEY([groupsid])
REFERENCES [dbo].[Groups] ([id])
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD FOREIGN KEY([receiver])
REFERENCES [dbo].[Account] ([username])
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD FOREIGN KEY([sender])
REFERENCES [dbo].[Account] ([username])
GO
INSERT INTO [dbo].[Groups] (name) VALUES ('Default')
GO
USE [master]
GO
ALTER DATABASE [MessageHandling] SET  READ_WRITE 
GO
