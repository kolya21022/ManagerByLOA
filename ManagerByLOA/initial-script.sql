-- Создание/инициализация БД Доверенностей [2018.06.10]


-- -- Удаление базы данных, если она существует
-- USE [master]
-- IF EXISTS (select * from [sys].[databases] where [name]='LetterOfAttorneyDb')
-- 	DROP DATABASE [LetterOfAttorneyDb]
-- GO

-- -- Удаление таблиц базы данных, если они существуют (сначала дочерние по вторичным ключам, затем родительские) 
-- USE [LetterOfAttorneyDb];
-- GO
-- IF OBJECT_ID('CurrentDbYear', 'U') IS NOT NULL          DROP TABLE [CurrentDbYear]
-- GO
-- IF OBJECT_ID('PreviousYearHistories', 'U') IS NOT NULL  DROP TABLE [PreviousYearHistories]
-- GO
-- IF OBJECT_ID('Shipments', 'U') IS NOT NULL              DROP TABLE [Shipments]
-- GO
-- IF OBJECT_ID('LetterOfAttorneys', 'U') IS NOT NULL      DROP TABLE [LetterOfAttorneys]
-- GO
-- IF OBJECT_ID('Cargoes', 'U') IS NOT NULL                DROP TABLE [Cargoes]
-- GO
-- IF OBJECT_ID('Measures', 'U') IS NOT NULL               DROP TABLE [Measures]
-- GO
-- IF OBJECT_ID('Companies', 'U') IS NOT NULL              DROP TABLE [Companies]
-- GO
-- IF OBJECT_ID('Couriers', 'U') IS NOT NULL               DROP TABLE [Couriers]
-- GO


-- Создание базы данных
USE [master]
CREATE DATABASE [LetterOfAttorneyDb]
GO
ALTER DATABASE [LetterOfAttorneyDb] MODIFY FILE 
( NAME = N'LetterOfAttorneyDb' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
GO
ALTER DATABASE [LetterOfAttorneyDb] MODIFY FILE 
( NAME = N'LetterOfAttorneyDb_log' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'LetterOfAttorneyDb', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LetterOfAttorneyDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [LetterOfAttorneyDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET DISABLE_BROKER 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET MULTI_USER 
GO
ALTER DATABASE [LetterOfAttorneyDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [LetterOfAttorneyDb] SET DB_CHAINING OFF 
GO



-- Создание таблицы [Единицы измерения]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Measures] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[name] nvarchar(150) NOT NULL, 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Measures] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Measures',
	@value=N'Таблица единиц измерения'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'name', 
	@value = N'Наименование'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Ключ уникальности поля [name]
ALTER TABLE [dbo].[Measures]
	ADD CONSTRAINT [UQ_Measures_Name] UNIQUE NONCLUSTERED ([name]) ON [PRIMARY]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateMeasures] ON [Measures] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateMeasures')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Measures] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [Организаций]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies] (
	[id] bigint IDENTITY(1, 1) NOT NULL,
	[name] nvarchar(150) NOT NULL,
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Companies',
	@value=N'Таблица огранизаций'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Companies', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Companies', 
	@level2name = N'name', 
	@value = N'Название'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Companies',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Companies',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Companies',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Companies',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Ключ уникальности поля [name]
ALTER TABLE [dbo].[Companies]
	ADD CONSTRAINT [UQ_Companies_Name] UNIQUE NONCLUSTERED ([name]) ON [PRIMARY]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateCompanies] ON [Companies] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateCompanies')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Companies] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [Курьеров]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Couriers] ( 
	[id] [bigint] IDENTITY(1, 1) NOT NULL, 
	[lastName] [nvarchar](150) NOT NULL, 
	[firstName] [nvarchar](150) NOT NULL, 
	[middleName] [nvarchar](150) NOT NULL, 
	[sex] [nvarchar](6) NOT NULL, 
	[passportSeriesAndNumber] [nvarchar](150) NOT NULL, 
	[passportIssuedByOrganization] [nvarchar](150) NOT NULL, 
	[passportIssueDate] [datetime] NOT NULL, 
	[profession] [nvarchar](150) NOT NULL, 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Couriers] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Couriers',
	@value=N'Таблица курьеров'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'lastName', 
	@value = N'Фамилия'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'firstName', 
	@value = N'Имя'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'middleName', 
	@value = N'Отчество'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'sex', 
	@value = N'Пол'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'passportSeriesAndNumber', 
	@value = N'Серия и номер паспорта'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'passportIssuedByOrganization', 
	@value = N'Организация выдавшая паспорт'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'passportIssueDate', 
	@value = N'Дата выдачи паспорта'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Couriers', 
	@level2name = N'profession', 
	@value = N'Профессия (должность)'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Couriers',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Couriers',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Couriers',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Couriers',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Ключ уникальности поля [passportSeriesAndNumber]
ALTER TABLE [dbo].[Couriers]
	ADD CONSTRAINT [UQ_Couriers_PassportSeriesAndNumber] UNIQUE NONCLUSTERED ([passportSeriesAndNumber]) ON [PRIMARY]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateCouriers] ON [Couriers] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateCouriers')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Couriers] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [Доверенностей]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LetterOfAttorneys] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[ordinalNumber] [bigint] NOT NULL, 
	[companiesId] [bigint] NOT NULL, 
	[isEmployeeCompany] [bit] NOT NULL, 
	[employeePersonnelNumber] [bigint], 
	[couriersId] [bigint], 
	[validityDateStart] [datetime] NOT NULL, 
	[validityDateEnd] [datetime] NOT NULL, 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_LetterOfAttorneys] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'LetterOfAttorneys',
	@value=N'Таблица доверенностей'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'ordinalNumber', 
	@value = N'Порядковый номер в текущем году'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'companiesId', 
	@value = N'Вторичный ключ на таблицу организаций'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'isEmployeeCompany', 
	@value = N'Признак сотрудника, или курьера'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'employeePersonnelNumber', 
	@value = N'Табельный номер сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'couriersId', 
	@value = N'Вторичный ключ на таблицу курьеров'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'validityDateStart', 
	@value = N'Дата начала срока действия доверенности'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'LetterOfAttorneys', 
	@level2name = N'validityDateEnd', 
	@value = N'Дата окончания срока действия доверенности'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'LetterOfAttorneys',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'LetterOfAttorneys',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'LetterOfAttorneys',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'LetterOfAttorneys',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Ключ уникальности поля [ordinalNumber] 
ALTER TABLE [dbo].[LetterOfAttorneys]
	ADD CONSTRAINT [UQ_LetterOfAttorneys_OrdinalNumber] UNIQUE NONCLUSTERED ([ordinalNumber]) ON [PRIMARY]
GO

-- Вторичный ключ на таблицу [организаций] 
ALTER TABLE [dbo].[LetterOfAttorneys] WITH CHECK
ADD CONSTRAINT [FK_LetterOfAttorneysToCompanies] FOREIGN KEY([companiesId])
REFERENCES [dbo].[Companies] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[LetterOfAttorneys] CHECK CONSTRAINT [FK_LetterOfAttorneysToCompanies]
GO

-- Вторичный ключ на таблицу [курьеров] 
ALTER TABLE [dbo].[LetterOfAttorneys] WITH CHECK
ADD CONSTRAINT [FK_LetterOfAttorneysToCouriers] FOREIGN KEY([couriersId])
REFERENCES [dbo].[Couriers] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[LetterOfAttorneys] CHECK CONSTRAINT [FK_LetterOfAttorneysToCouriers]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateLetterOfAttorneys] ON [LetterOfAttorneys] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateLetterOfAttorneys')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [LetterOfAttorneys] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [перевозимых в ТМЦ грузов]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cargoes] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[name] nvarchar(150) NOT NULL, 
	[measuresId] [bigint], 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Cargoes] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Cargoes',
	@value=N'Таблица предметов (грузов), которые первозятся по доверенностям'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Cargoes', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Cargoes', 
	@level2name = N'name', 
	@value = N'Наименование'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Cargoes', 
	@level2name = N'measuresId', 
	@value = N'Вторичный ключ на таблицу единиц измерения'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Cargoes',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Cargoes',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Cargoes',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Cargoes',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Ключ уникальности пары полей [name] и [measuresId] 
ALTER TABLE [dbo].[Cargoes]
	ADD CONSTRAINT [UQ_Cargoes_Name_And_MeasuresId] UNIQUE NONCLUSTERED ([name], [measuresId]) ON [PRIMARY]
GO

-- Вторичный ключ на таблицу [единиц измерения] 
ALTER TABLE [dbo].[Cargoes] WITH CHECK
ADD CONSTRAINT [FK_CargoesToMeasures] FOREIGN KEY([measuresId])
REFERENCES [dbo].[Measures] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Cargoes] CHECK CONSTRAINT [FK_CargoesToMeasures]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateCargoes] ON [Cargoes] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateCargoes')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Cargoes] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [перевозимых грузов по конкретным доверенностям (товарно-материальные ценности, ТМЦ)]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shipments] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[letterOfAttorneysId] [bigint] NOT NULL, 
	[cargoesId] [bigint] NOT NULL, 
	[count] [float], 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Shipments] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Shipments',
	@value=N'Таблица перевозимых грузов по конкретным доверенностям (товарно-материальные ценности, ТМЦ)'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Shipments', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Shipments', 
	@level2name = N'letterOfAttorneysId', 
	@value = N'Вторичный ключ на таблицу доверенностей'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Shipments', 
	@level2name = N'cargoesId', 
	@value = N'Вторичный ключ на таблицу грузов'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Shipments', 
	@level2name = N'count', 
	@value = N'Количество'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Shipments',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Shipments',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Shipments',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Shipments',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Вторичный ключ на таблицу [грузов (предметов)] 
ALTER TABLE [dbo].[Shipments] WITH CHECK
ADD CONSTRAINT [FK_ShipmentsToCargoes] FOREIGN KEY([cargoesId])
REFERENCES [dbo].[Cargoes] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Shipments] CHECK CONSTRAINT [FK_ShipmentsToCargoes]
GO

-- Вторичный ключ на таблицу [доверенностей] 
ALTER TABLE [dbo].[Shipments] WITH CHECK
ADD CONSTRAINT [FK_ShipmentsToLetterOfAttorneys] FOREIGN KEY([letterOfAttorneysId])
REFERENCES [dbo].[LetterOfAttorneys] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Shipments] CHECK CONSTRAINT [FK_ShipmentsToLetterOfAttorneys]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateShipments] ON [Shipments] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateShipments')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Shipments] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [Истории доверенностей (ТМЦ) предыдущих лет]
USE [LetterOfAttorneyDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreviousYearHistories] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[year] [bigint] NOT NULL, 
	[ordinalNumber] [bigint] NOT NULL, 
	[validityDateStart] [datetime] NOT NULL, 
	[validityDateEnd] [datetime] NOT NULL, 
	[company] nvarchar(150) NOT NULL, 
	[employeePersonnelNumber] [bigint], 
	[courierOrEmployeeFullName] nvarchar(150), 
	[courierOrEmployeeProfession] nvarchar(150), 
	[passportSeriesAndNumber] nvarchar(150), 
	[passportIssuedByOrganization] nvarchar(150), 
	[passportIssueDate] [datetime], 
	[shipmentName] nvarchar(150) NOT NULL, 
	[shipmentCount] [float], 
	[shipmentMeasure] nvarchar(150), 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_PreviousYearHistories] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'PreviousYearHistories',
	@value=N'Таблица истории доверенностей предыдущих лет'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'year', 
	@value = N'Год создания доверенности в базе данных'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'ordinalNumber', 
	@value = N'Порядковый номер доверенности'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'validityDateStart', 
	@value = N'Дата начала срока действия доверенности'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'validityDateEnd', 
	@value = N'Дата окончания срока действия доверенности'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'company', 
	@value = N'Организация'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'employeePersonnelNumber', 
	@value = N'Табельный номер сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'courierOrEmployeeFullName', 
	@value = N'ФИО курьера или сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'courierOrEmployeeProfession', 
	@value = N'Профессия курьера или сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'passportSeriesAndNumber', 
	@value = N'Номер и серия паспорта курьера или сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'passportIssuedByOrganization', 
	@value = N'Орган выдавший паспорт курьера или сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'passportIssueDate', 
	@value = N'Дата выдачи паспорта курьера или сотрудника'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'shipmentName', 
	@value = N'Наименование товарно-материальной ценности'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'shipmentCount', 
	@value = N'Количество товарно-материальной ценности'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'PreviousYearHistories', 
	@level2name = N'shipmentMeasure', 
	@value = N'Единица измерения товарно-материальной ценности'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'PreviousYearHistories',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'PreviousYearHistories',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'PreviousYearHistories',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'PreviousYearHistories',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdatePreviousYearHistories] ON [PreviousYearHistories] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdatePreviousYearHistories')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [PreviousYearHistories] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO



-- Создание таблицы [Маркера текущего года выдачи доверенностей]
CREATE TABLE CurrentDbYear (
	[oneRowLock] char(1) NOT NULL DEFAULT 'x', 
	[year] [bigint] NOT NULL, 
	[dateTransitionToYear] [datetime] NOT NULL, 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	CONSTRAINT [PK_CurrentDbYear] PRIMARY KEY ([oneRowLock]), 
	CONSTRAINT [CK_CurrentDbYear_Locked] CHECK ([oneRowLock] = 'x')
)
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'CurrentDbYear',
	@value=N'Таблица текущего года выдачи доверенностей'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'CurrentDbYear', 
	@level2name = N'oneRowLock', 
	@value = N'Поле блокировщик, не дающий существовать более одной записи в таблице'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'CurrentDbYear', 
	@level2name = N'year', 
	@value = N'Текущий год выдачи доверенностей'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'CurrentDbYear', 
	@level2name = N'dateTransitionToYear', 
	@value = N'Дата перевода на указанный год'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'CurrentDbYear',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'CurrentDbYear',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'CurrentDbYear',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'CurrentDbYear',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
CREATE TRIGGER [TriggerUpdateCurrentDbYear] ON [CurrentDbYear] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateCurrentDbYear')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [CurrentDbYear] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
	END
GO

-- Вставка инициализационной записи текущего года 
INSERT INTO [CurrentDbYear] ([year], [dateTransitionToYear]) VALUES (YEAR(GETDATE()), GETDATE()) 
GO

-- Пример перехода на следующий год
-- UPDATE [CurrentDbYear] SET [year] = 2019, [dateTransitionToYear] = GETDATE() 
-- GO
