CREATE TABLE [Employees] (
    [Id] int NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Salary] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Companies] (
    [Id] int NOT NULL,
    [DirectorId] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] int NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Companies_Employees_DirectorId] FOREIGN KEY ([DirectorId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Divisions] (
    [Id] int NOT NULL,
    [DivisionLeadId] int NOT NULL,
    [CompanyId] int NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] int NOT NULL,
    CONSTRAINT [PK_Divisions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Divisions_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]),
    CONSTRAINT [FK_Divisions_Employees_DivisionLeadId] FOREIGN KEY ([DivisionLeadId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Projects] (
    [Id] int NOT NULL,
    [ProjectLeadId] int NOT NULL,
    [DivisionId] int NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] int NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Projects_Divisions_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([Id]),
    CONSTRAINT [FK_Projects_Employees_ProjectLeadId] FOREIGN KEY ([ProjectLeadId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Departments] (
    [Id] int NOT NULL,
    [DepartmentLeadId] int NOT NULL,
    [ProjectId] int NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] int NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Departments_Employees_DepartmentLeadId] FOREIGN KEY ([DepartmentLeadId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Departments_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id])
);
GO


CREATE INDEX [IX_Companies_DirectorId] ON [Companies] ([DirectorId]);
GO


CREATE INDEX [IX_Departments_DepartmentLeadId] ON [Departments] ([DepartmentLeadId]);
GO


CREATE INDEX [IX_Departments_ProjectId] ON [Departments] ([ProjectId]);
GO


CREATE INDEX [IX_Divisions_CompanyId] ON [Divisions] ([CompanyId]);
GO


CREATE INDEX [IX_Divisions_DivisionLeadId] ON [Divisions] ([DivisionLeadId]);
GO


CREATE INDEX [IX_Projects_DivisionId] ON [Projects] ([DivisionId]);
GO


CREATE INDEX [IX_Projects_ProjectLeadId] ON [Projects] ([ProjectLeadId]);
GO


