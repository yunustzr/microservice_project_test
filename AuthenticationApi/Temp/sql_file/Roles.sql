INSERT INTO Role (Id, Name, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, DeletedToken,IsDefault)
VALUES
(1, 'Admin', 'admin', NOW(), 'admin', NOW(), '',0),
(2, 'User', 'admin', NOW(), 'admin', NOW(), '',0),
(3, 'Auditor', 'admin', NOW(), 'admin', NOW(), '',0),
(4, 'Manager', 'admin', NOW(), 'admin', NOW(), '',0),
(5, 'Guest', 'admin', NOW(), 'admin', NOW(), '',0),
(6, 'Developer', 'admin', NOW(), 'admin', NOW(), '',0),
(7, 'Support', 'admin', NOW(), 'admin', NOW(), '',0),
(8, 'Analyst', 'admin', NOW(), 'admin', NOW(), '',0),
(9, 'Tester', 'admin', NOW(), 'admin', NOW(), '',0),
(10, 'Public', 'admin', NOW(), 'admin', NOW(), '',1);