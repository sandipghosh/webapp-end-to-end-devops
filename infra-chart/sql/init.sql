CREATE TABLE `TaskItems` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `IsCompleted` tinyint(1) NOT NULL DEFAULT FALSE,
    `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP(),
    CONSTRAINT `PK_TaskItems` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;