IF NOT EXISTS (SELECT TOP 1 1 FROM AspNetUsers WHERE Email = 'super@admin.com' AND DeletedAt IS NULL)
BEGIN
	INSERT INTO [dbo].[AspNetUsers]
           ([FirstName]
           ,[LastName]
           ,[ProfileAttachmentId]
           ,[IsUserRegistered]
           ,[VerificationToken]
           ,[IsEmailVerified]
           ,[CreatedAt]
		   ,[PhoneNumberConfirmed]
		   ,[TwoFactorEnabled]
		   ,[LockoutEnabled]
           ,[UserName]
           ,[NormalizedUserName]
           ,[Email]
           ,[NormalizedEmail],
		   [EmailConfirmed]
           ,[PasswordHash]
           ,[SecurityStamp]
           ,[ConcurrencyStamp]
           ,[AccessFailedCount])
     VALUES
           ('Super', 'Admin', null, 1, 'ccf5fce0-229f-48a7-bb0c-d7e9eb678657', 1, GETDATE(),0,0,1, 'super@admin.com', 'SUPER@ADMIN.COM', 'super@admin.com',
		   'SUPER@ADMIN.COM',0, 'Admin@1234','M5H3CQTFRIKFJWNKNG7CAV2MUXZBA6CO','00a185b8-8ccf-4713-b4de-afceabdcda10',0)
END

DECLARE @SuperAdminId INT = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'super@admin.com')
DECLARE @EntityId INT

IF NOT EXISTS (SELECT TOP 1 1 FROM [ReferenceDataEntities] WHERE EntityType = 'BusinessCategory' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[ReferenceDataEntities] ([EntityType], [CreatedById], [CreatedAt], [UpdatedById], [UpdatedAt], [DeletedById], [DeletedAt]) 
	VALUES (N'BusinessCategory', @SuperAdminId, GETUTCDATE(), NULL, NULL, NULL, NULL)
END
IF NOT EXISTS (SELECT TOP 1 1 FROM [ReferenceDataEntities] WHERE EntityType = 'BusinessType' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[ReferenceDataEntities] ([EntityType], [CreatedById], [CreatedAt], [UpdatedById], [UpdatedAt], [DeletedById], [DeletedAt]) 
	VALUES (N'BusinessType', @SuperAdminId, GETUTCDATE(), NULL, NULL, NULL, NULL)
END
IF NOT EXISTS (SELECT TOP 1 1 FROM [ReferenceDataEntities] WHERE EntityType = 'PartyType' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[ReferenceDataEntities] ([EntityType], [CreatedById], [CreatedAt], [UpdatedById], [UpdatedAt], [DeletedById], [DeletedAt]) 
	VALUES (N'PartyType', @SuperAdminId, GETUTCDATE(), NULL, NULL, NULL, NULL)
END

IF EXISTS (SELECT TOP 1 1 FROM [ReferenceDataEntities] WHERE EntityType = 'PartyType' AND DeletedAt IS NULL)
BEGIN
	SET @EntityId = (SELECT TOP 1 Id FROM [ReferenceDataEntities] WHERE EntityType = 'PartyType' AND DeletedAt IS NULL)
	IF NOT EXISTS (SELECT TOP 1 1 FROM [ReferenceDataValues] WHERE EntityValue = 'Customer' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Customer', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS (SELECT TOP 1 1 FROM [ReferenceDataValues] WHERE EntityValue = 'Supplier' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Supplier', @EntityId, @SuperAdminId, GETUTCDATE())
	END
END

IF EXISTS (SELECT TOP 1 1 FROM [ReferenceDataEntities] WHERE EntityType = 'BusinessCategory' AND DeletedAt IS NULL)
BEGIN
	SET @EntityId = (SELECT TOP 1 Id FROM [ReferenceDataEntities] WHERE EntityType = 'BusinessCategory' AND DeletedAt IS NULL)
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Retail' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Retail', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Manufacturing' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Manufacturing', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Wholesale' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Wholesale', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Agriculture' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Agriculture', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Finance & Insurance' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Finance & Insurance', @EntityId, @SuperAdminId, GETUTCDATE())
	END
END
IF EXISTS (SELECT TOP 1 1 FROM [ReferenceDataEntities] WHERE EntityType = 'BusinessType')
BEGIN
	SET @EntityId = (SELECT TOP 1 Id FROM [ReferenceDataEntities] WHERE EntityType = 'BusinessType')
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Sole Proprietorship' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Sole Proprietorship', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Partnership' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Partnership', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Private Limited Company (Pvt Ltd)' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Private Limited Company (Pvt Ltd)',@EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Public Limited Company' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Public Limited Company', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Cooperative' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Cooperative', @EntityId, @SuperAdminId, GETUTCDATE())
	END
	IF NOT EXISTS(SELECT TOP 1 1 FROM ReferenceDataValues WHERE EntityValue = 'Nonprofit Organization' AND EntityTypeId = @EntityId)
	BEGIN
		INSERT [dbo].[ReferenceDataValues] ([EntityValue], [EntityTypeId], [CreatedById], [CreatedAt]) 
		VALUES (N'Nonprofit Organization',@EntityId, @SuperAdminId, GETUTCDATE())
	END
END

IF NOT EXISTS (SELECT TOP 1 1 FROM Roles WHERE RoleName = 'Owner/Admin' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[Roles] ([RoleName], [Description], [CreatedById], [CreatedAt]) 
	VALUES (N'Owner/Admin', N'Full access to all customer/supplier data', @SuperAdminId, GETUTCDATE())
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Roles WHERE RoleName = 'Purchase Manager' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[Roles] ([RoleName], [Description], [CreatedById], [CreatedAt]) 
	VALUES (N'Purchase Manager', N'Manages suppliers and purchase transactions', @SuperAdminId, GETUTCDATE())
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Roles WHERE RoleName = 'Sales Manager' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[Roles] ([RoleName], [Description], [CreatedById], [CreatedAt]) 
	VALUES (N'Sales Manager', N'Deals with customers and sales entries', @SuperAdminId, GETUTCDATE())
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Roles WHERE RoleName = 'Viewer/Read-only' AND DeletedAt IS NULL)
BEGIN
	INSERT [dbo].[Roles] ([RoleName], [Description], [CreatedById], [CreatedAt]) 
	VALUES (N'Viewer/Read-only', N'Can view ledgers for audit or reporting', @SuperAdminId, GETUTCDATE())
END