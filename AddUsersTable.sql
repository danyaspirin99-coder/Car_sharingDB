USE Car_sharingDB;
GO

CREATE TABLE Users (
    ID_User     INT IDENTITY(1,1) PRIMARY KEY,
    Username    NVARCHAR(50)  NOT NULL UNIQUE,
    Password    NVARCHAR(50)  NOT NULL
);
GO

-- Пользователь по умолчанию: логин admin, пароль admin
INSERT INTO Users (Username, Password) VALUES ('admin', 'admin');
GO
