


--创建表
CREATE TABLE [dbo].[SYS_User] (
[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, ----自增主键
[GID] nvarchar(200) NULL,
[Account] nvarchar(200) NULL,
[Password] nvarchar(200) NULL,
[TypeId] int NULL,
[AddTime] datetime NULL ,
)

--唯一索引(非聚集索引)
CREATE UNIQUE INDEX [Account01] ON [SYS_User] ([Account])

--添加表说明
EXECUTE sp_addextendedproperty   N'MS_Description',N'用户表',N'user',N'dbo',N'table',N'SYS_User',NULL,NULL

--添加字段说明
EXECUTE   sp_addextendedproperty   N'MS_Description',N'唯一标识',N'user',N'dbo',N'table',N'SYS_User',N'column',N'GID'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'账号',N'user',N'dbo',N'table',N'SYS_User',N'column',N'Account'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'密码',N'user',N'dbo',N'table',N'SYS_User',N'column',N'Password'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'类型',N'user',N'dbo',N'table',N'SYS_User',N'column',N'TypeId'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'创建时间',N'user',N'dbo',N'table',N'SYS_User',N'column',N'AddTime'

INSERT INTO [dbo].[SYS_User] ([GID], [Account], [Password], [TypeId], [AddTime]) VALUES (N'D1CA3579-696A-4C3E-B08A-7C9D33748AC0', N'admin', N'123456', 1, GETDATE());



--创建表
CREATE TABLE [dbo].[Admin_Sql] (
[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, ----自增主键
[SqlName] nvarchar(200) NULL,
[SqlContent] nvarchar(MAX) NULL,
[Remarks] nvarchar(500) NULL,
[AddPeople] nvarchar(500) NULL,
[AddTime] datetime NULL ,
)
--唯一索引(非聚集索引)
CREATE UNIQUE INDEX [SqlName01] ON [Admin_Sql] ([SqlName])

--添加表说明
EXECUTE sp_addextendedproperty   N'MS_Description',N'SQL表',N'user',N'dbo',N'table',N'Admin_Sql',NULL,NULL

--添加字段说明
EXECUTE   sp_addextendedproperty   N'MS_Description',N'SQL名称',N'user',N'dbo',N'table',N'Admin_Sql',N'column',N'SqlName'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'SQL语句',N'user',N'dbo',N'table',N'Admin_Sql',N'column',N'SqlContent'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'备注',N'user',N'dbo',N'table',N'Admin_Sql',N'column',N'Remarks'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'添加人',N'user',N'dbo',N'table',N'Admin_Sql',N'column',N'AddPeople'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'创建时间',N'user',N'dbo',N'table',N'Admin_Sql',N'column',N'AddTime'


INSERT INTO [dbo].[Admin_Sql] ([SqlName], [SqlContent], [Remarks], [AddPeople], [AddTime]) VALUES (N'SelectA1', N'select * from Test WHERE Name = @Name', N'查询Name', N'admin', GETDATE());
INSERT INTO [dbo].[Admin_Sql] ([SqlName], [SqlContent], [Remarks], [AddPeople], [AddTime]) VALUES (N'SelectA2', N'select * from Test WHERE Id >= @IdA and  Id <= @IdB',  N'查询id区间', N'admin', GETDATE());
INSERT INTO [dbo].[Admin_Sql] ([SqlName], [SqlContent], [Remarks], [AddPeople], [AddTime]) VALUES (N'SelectA3', N'select * from Test WHERE Name like @Name', N'模糊查询Name', N'admin', GETDATE());
INSERT INTO [dbo].[Admin_Sql] ([SqlName], [SqlContent], [Remarks], [AddPeople], [AddTime]) VALUES (N'UpdateTest', N'UPDATE Test SET Remarks = @Remarks WHERE Id = @Id', N'根据id修改备注', N'admin', GETDATE());
INSERT INTO [dbo].[Admin_Sql] ([SqlName], [SqlContent], [Remarks], [AddPeople], [AddTime]) VALUES (N'DeleteTest', N'delete Test WHERE Id = @Id',  N'根据id删除', N'admin', GETDATE());


--创建表
CREATE TABLE [dbo].[Test] (
[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, ----自增主键
[Name] nvarchar(50) NULL  DEFAULT '默认值',  ----可设置默认值
[Remarks] nvarchar(MAX) NULL, --------------------字符串类型
[TypeId] int NULL,-------------------------------整型，取值范围[-231~231)
[ServicevalueExcludingVat] MONEY NULL, ----------货币型 
[ServicevalueEncludingVat] decimal(12,4) NULL, --精确数值型 共12位，小数点右4位
[VatPercentage] float NULL, ---------------------近似数值型
[AddDate] date NULL , ------------------------------日期
[AddTime] datetime NULL ,---------------------------时间
)

--添加表说明
EXECUTE sp_addextendedproperty   N'MS_Description',N'测试表',N'user',N'dbo',N'table',N'Test',NULL,NULL

--添加字段说明
EXECUTE   sp_addextendedproperty   N'MS_Description',N'名称',N'user',N'dbo',N'table',N'Test',N'column',N'Name'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'备注',N'user',N'dbo',N'table',N'Test',N'column',N'Remarks'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'类型',N'user',N'dbo',N'table',N'Test',N'column',N'TypeId'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'服务价值，不包括增值税',N'user',N'dbo',N'table',N'Test',N'column',N'ServicevalueExcludingVat'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'服务价值，包括增值税',N'user',N'dbo',N'table',N'Test',N'column',N'ServicevalueEncludingVat'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'增值税百分比',N'user',N'dbo',N'table',N'Test',N'column',N'VatPercentage'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'创建日期',N'user',N'dbo',N'table',N'Test',N'column',N'AddDate'
EXECUTE   sp_addextendedproperty   N'MS_Description',N'创建时间',N'user',N'dbo',N'table',N'Test',N'column',N'AddTime'

--插入模拟数据
DECLARE @TAB TABLE (
[Name] nvarchar(50) NULL,  ----可设置默认值
[Remarks] nvarchar(MAX) NULL, --------------------字符串类型
[TypeId] int NULL,-------------------------------整型，取值范围[-231~231)
[ServicevalueExcludingVat] MONEY NULL, ----------货币型 
[ServicevalueEncludingVat] decimal(12,4) NULL, --精确数值型 共12位，小数点右4位
[VatPercentage] float NULL, ---------------------近似数值型
[AddDate] date NULL , ------------------------------日期
[AddTime] datetime NULL ---------------------------时间
) 
DECLARE @i int   
SET @i = 1
WHILE @i<= 100000
BEGIN
  INSERT INTO @TAB
    VALUES(
    ABS(CHECKSUM(newid())%900000)+100000,NEWID(),
    CONVERT(int,RAND()*10)+1,
    CONVERT(MONEY,RAND()*1000),
    CONVERT(decimal(12,4),RAND()*1000),
    CONVERT(decimal(12,2),RAND()*100),
    DATEADD(dd, CONVERT(int,RAND()*1000), '2015-01-01'),
    DATEADD(dd, CONVERT(int,RAND()*1000), '2015-01-01'))  
  SET @i = @i + 1 
END

INSERT INTO [Test]([Name],[Remarks],[TypeId],[ServicevalueExcludingVat],[ServicevalueEncludingVat],[VatPercentage],[AddDate],[AddTime]) 
SELECT [Name],[Remarks],[TypeId],[ServicevalueExcludingVat],[ServicevalueEncludingVat],[VatPercentage],[AddDate],[AddTime] FROM @TAB

