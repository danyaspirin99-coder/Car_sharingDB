-- =============================================
-- Скрипт создания БД для сервиса каршеринга
-- Запускать в SQL Server Management Studio
-- =============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'CarSharingDB')
    DROP DATABASE CarSharingDB;
GO

CREATE DATABASE CarSharingDB;
GO

USE CarSharingDB;
GO

-- =============================================
-- Таблица: Client
-- =============================================
CREATE TABLE Client (
    ID_Client       INT IDENTITY(1,1) PRIMARY KEY,
    Surname         NVARCHAR(50)    NOT NULL,
    Name            NVARCHAR(50)    NOT NULL,
    Father_name     NVARCHAR(50)    NULL,
    Date_of_birth   DATE            NOT NULL,
    Phone_number    NVARCHAR(20)    NOT NULL,
    License         NVARCHAR(50)    NOT NULL,

    CONSTRAINT UQ_Client_Phone   UNIQUE (Phone_number),
    CONSTRAINT UQ_Client_License UNIQUE (License)
);
GO

-- =============================================
-- Таблица: Vehicles
-- =============================================
CREATE TABLE Vehicles (
    ID_Vehicles     INT IDENTITY(1,1) PRIMARY KEY,
    Number          NVARCHAR(10)    NOT NULL,
    Model           NVARCHAR(50)    NOT NULL,
    Type_of_fuel    NVARCHAR(20)    NOT NULL,
    Year            INT             NOT NULL,
    Color           NVARCHAR(50)    NOT NULL,
    Transmission    NVARCHAR(50)    NOT NULL,
    Drive           NVARCHAR(50)    NOT NULL,
    Status          NVARCHAR(20)    NOT NULL DEFAULT 'Свободен',

    CONSTRAINT UQ_Vehicles_Number  UNIQUE (Number),
    CONSTRAINT CHK_Vehicles_Year   CHECK (Year BETWEEN 1970 AND 2100),
    CONSTRAINT CHK_Vehicles_Status CHECK (Status IN (N'Свободен', N'Занят', N'На обслуживании'))
);
GO

-- =============================================
-- Таблица: Rent
-- =============================================
CREATE TABLE Rent (
    ID_Rent         INT IDENTITY(1,1) PRIMARY KEY,
    Beginnig_date   DATE            NOT NULL,
    END_date        DATE            NOT NULL,
    ID_Client       INT             NOT NULL,
    ID_Vehicles     INT             NOT NULL,

    CONSTRAINT FK_Rent_Client   FOREIGN KEY (ID_Client)   REFERENCES Client(ID_Client),
    CONSTRAINT FK_Rent_Vehicle  FOREIGN KEY (ID_Vehicles) REFERENCES Vehicles(ID_Vehicles),
    CONSTRAINT CHK_Rent_Dates   CHECK (END_date > Beginnig_date)
);
GO

-- =============================================
-- Таблица: Accessories
-- =============================================
CREATE TABLE Accessories (
    ID_Accessories  INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(50)    NOT NULL,

    CONSTRAINT CHK_Accessories_Name CHECK (Name IN (
        N'Детское кресло', N'Кабриолет', N'Кондиционер', N'Багаж на крыше'
    ))
);
GO

-- =============================================
-- Таблица: Vehicles_Accessories (связь M:M)
-- =============================================
CREATE TABLE Vehicles_Accessories (
    ID_Vehicles_Accessories INT IDENTITY(1,1) PRIMARY KEY,
    ID_Vehicles             INT NOT NULL,
    ID_Accessories          INT NOT NULL,

    CONSTRAINT FK_VA_Vehicle    FOREIGN KEY (ID_Vehicles)   REFERENCES Vehicles(ID_Vehicles)   ON DELETE CASCADE,
    CONSTRAINT FK_VA_Accessory  FOREIGN KEY (ID_Accessories) REFERENCES Accessories(ID_Accessories) ON DELETE CASCADE,
    CONSTRAINT UQ_VA_Pair       UNIQUE (ID_Vehicles, ID_Accessories)
);
GO

-- =============================================
-- Таблица: Payments
-- =============================================
CREATE TABLE Payments (
    ID_Payments     INT IDENTITY(1,1) PRIMARY KEY,
    Way_of_payment  NVARCHAR(50)    NOT NULL,
    Amount          DECIMAL(8, 2)   NOT NULL,

    CONSTRAINT CHK_Payments_Way    CHECK (Way_of_payment IN (N'Карта', N'Наличные', N'QR-код')),
    CONSTRAINT CHK_Payments_Amount CHECK (Amount > 0)
);
GO

-- =============================================
-- Таблица: Client_Payments (связь M:M)
-- =============================================
CREATE TABLE Client_Payments (
    ID_Client_Payments  INT IDENTITY(1,1) PRIMARY KEY,
    ID_Client           INT             NOT NULL,
    ID_Payments         INT             NOT NULL,
    Type_of_payment     NVARCHAR(50)    NOT NULL,

    CONSTRAINT FK_CP_Client   FOREIGN KEY (ID_Client)   REFERENCES Client(ID_Client)     ON DELETE CASCADE,
    CONSTRAINT FK_CP_Payment  FOREIGN KEY (ID_Payments) REFERENCES Payments(ID_Payments) ON DELETE CASCADE,
    CONSTRAINT CHK_CP_Type    CHECK (Type_of_payment IN (N'Штраф', N'Плата за аренду', N'Ремонт'))
);
GO

-- =============================================
-- Тестовые данные
-- =============================================

INSERT