INSERT INTO User (Id, Username, NormalizedUsername, Email, NormalizedEmail, PasswordHash, IsActive, CreatedAt, LastLogin, FailedLoginAttempts, LockoutEnd, Timezone, Culture,IsLdapUser,IsTwoFactorEnabled)
VALUES
(UUID(), 'admin', 'ADMIN', 'admin@example.com', 'ADMIN@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user1', 'USER1', 'user1@example.com', 'USER1@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user2', 'USER2', 'user2@example.com', 'USER2@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user3', 'USER3', 'user3@example.com', 'USER3@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user4', 'USER4', 'user4@example.com', 'USER4@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user5', 'USER5', 'user5@example.com', 'USER5@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user6', 'USER6', 'user6@example.com', 'USER6@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',0,0),
(UUID(), 'user7', 'USER7', 'user7@example.com', 'USER7@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user8', 'USER8', 'user8@example.com', 'USER8@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',1,0),
(UUID(), 'user9', 'USER9', 'user9@example.com', 'USER9@EXAMPLE.COM', 'hashed_password', TRUE, NOW(), NOW(), 0, NULL, 'Europe/Istanbul', 'tr-TR',0,0);