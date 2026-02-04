-- Проверка на существование БД и удаление если существует
IF DB_ID('Car_sharingDB') IS NOT NULL
BEGIN
    -- Закрываем все активные соединения с БД
    ALTER DATABASE Car_sharingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Car_sharingDB;
END
GO

CREATE DATABASE Car_sharingDB;
GO

-- Текущая база данных
USE Car_sharingDB;
GO

-- Создание таблицы Client (Клиент)
CREATE TABLE Client
(
    ID_Client INT IDENTITY(1,1) PRIMARY KEY,
    Surname NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    Father_name NVARCHAR(50) NULL,
    Date_of_birth DATE NOT NULL,
    Phone_number NVARCHAR(20) NOT NULL UNIQUE,
    License NVARCHAR(50) Not NULL UNIQUE
);
GO

-- Создание таблицы Vehicles (Машины)
CREATE TABLE Vehicles
(
    ID_Vehicles INT IDENTITY(1,1) PRIMARY KEY,
    [Number] NVARCHAR(10) NOT NULL,
	Model NVARCHAR(50) NOT NULL,
    Type_of_fuel NVARCHAR(20) NOT NULL CHECK ([Type_of_fuel] IN ('Бензин', 'Дизель', 'Электро')),
    [Year] INT NOT NULL CHECK ([Year] >= 1970),
    Color NVARCHAR(50) NOT NULL,
	Transmission NVARCHAR(50) NOT NULL CHECK(Transmission IN ('Автомат', 'Механика')),
	Drive NVARCHAR(50) NOT NULL CHECK(Drive IN ('Полный', 'Задний', 'Передний')),
    [Status] NVARCHAR(20) NOT NULL CHECK ([Status] IN ('Доступно', 'Забронировано', 'На обслуживании')), 
);
GO

-- Создание таблицы Client (Клиент)
CREATE TABLE Rent
(
    ID_Rent INT IDENTITY(1,1) PRIMARY KEY,
    Beginnig_date DATE NOT NULL DEFAULT GETDATE(),
    END_date DATE NOT NULL,
	ID_Client INT NOT NULL UNIQUE REFERENCES Client (ID_Client),
	ID_Vehicles INT NOT NULL UNIQUE REFERENCES Vehicles (ID_Vehicles)
);
GO

-- Создание таблицы Accessories (Аксессуары)
CREATE TABLE Accessories
(
    ID_Accessories INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL CHECK([Name] IN ('Детское кресло', 'Кабриолет', 'Кондиционер', 'багаж на крыше'))
);
GO

-- Создание таблицы Payments (Платежи)
CREATE TABLE Payments
(
    ID_Payments INT IDENTITY(1,1) PRIMARY KEY,
    Way_of_payment NVARCHAR(50) NOT NULL CHECK(Way_of_payment IN('Карта', 'Наличные', 'QR-код')),
    Amount DECIMAL(5, 2) NOT NULL CHECK(Amount > 0),

);
GO

-- Создание ассоциативной таблицы Client_Payments (Клиент-платежи)
CREATE TABLE Client_Payments
(
    ID_Client_Payments INT IDENTITY(1,1) PRIMARY KEY,
    ID_Client INT NOT NULL REFERENCES Client (ID_Client),
    ID_Payments INT NOT NULL REFERENCES Payments (ID_Payments),
    Type_of_payment NVARCHAR(50) NOT NULL CHECK( Type_of_payment IN ('Штраф', 'Плата за аренду', 'Ремонт'))
    
);
GO

-- Создание ассоциативной таблицы Vehicles_Accessories (Машина_аксессуары)
CREATE TABLE Vehicles_Accessories
(
    ID_Vehicles_Accessories INT IDENTITY(1,1) PRIMARY KEY,
    ID_Vehicles INT NOT NULL REFERENCES Vehicles (ID_Vehicles),
    ID_Accessories INT NOT NULL REFERENCES Accessories (ID_Accessories)
);
GO