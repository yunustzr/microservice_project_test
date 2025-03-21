INSERT INTO UserRoles (Id, UserId, RoleId)
VALUES
(1, (SELECT Id FROM User WHERE Username = 'admin'), 1),
(2, (SELECT Id FROM User WHERE Username = 'user1'), 2),
(3, (SELECT Id FROM User WHERE Username = 'user2'), 2),
(4, (SELECT Id FROM User WHERE Username = 'user3'), 3),
(5, (SELECT Id FROM User WHERE Username = 'user4'), 4),
(6, (SELECT Id FROM User WHERE Username = 'user5'), 5),
(7, (SELECT Id FROM User WHERE Username = 'user6'), 6),
(8, (SELECT Id FROM User WHERE Username = 'user7'), 7),
(9, (SELECT Id FROM User WHERE Username = 'user8'), 8),
(10, (SELECT Id FROM User WHERE Username = 'user9'), 9);