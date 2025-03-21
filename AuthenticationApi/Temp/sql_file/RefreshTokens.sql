INSERT INTO RefreshTokens (Id, Token, Expires, Created, Revoked, UserId)
VALUES
(UUID(), 'token1', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'admin')),
(UUID(), 'token2', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user1')),
(UUID(), 'token3', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user2')),
(UUID(), 'token4', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user3')),
(UUID(), 'token5', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user4')),
(UUID(), 'token6', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user5')),
(UUID(), 'token7', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user6')),
(UUID(), 'token8', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user7')),
(UUID(), 'token9', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user8')),
(UUID(), 'token10', DATE_ADD(NOW(), INTERVAL 7 DAY), NOW(), NULL, (SELECT Id FROM Users WHERE Username = 'user9'));