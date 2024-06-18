BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TB_PERSONAGENS]') AND [c].[name] = N'Nome');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TB_PERSONAGENS] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TB_PERSONAGENS] ALTER COLUMN [Nome] Varchar(200) NOT NULL;
GO

CREATE TABLE [TB_ARMAS] (
    [Id] int NOT NULL IDENTITY,
    [Nome] Varchar(200) NOT NULL,
    [Dano] int NOT NULL,
    CONSTRAINT [PK_TB_ARMAS] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Dano', N'Nome') AND [object_id] = OBJECT_ID(N'[TB_ARMAS]'))
    SET IDENTITY_INSERT [TB_ARMAS] ON;
INSERT INTO [TB_ARMAS] ([Id], [Dano], [Nome])
VALUES (1, 10, 'Sword'),
(2, 12, 'Axe'),
(3, 20, 'SwordFire'),
(4, 15, 'Cajado'),
(5, 55, 'Spear'),
(6, 30, 'CajadoDeath'),
(7, 100, 'Guns');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Dano', N'Nome') AND [object_id] = OBJECT_ID(N'[TB_ARMAS]'))
    SET IDENTITY_INSERT [TB_ARMAS] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240409234114_MigracaoArma', N'8.0.3');
GO

COMMIT;
GO

