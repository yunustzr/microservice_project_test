INSERT INTO Permission (Id, Name, Description, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, OperationId, ResourceId)
VALUES
(1, 'CreateUser', 'Create user permission', 'admin', NOW(), 'admin', NOW(), 1, 1),
(2, 'ReadUser', 'Read user permission', 'admin', NOW(), 'admin', NOW(), 2, 1),
(3, 'UpdateUser', 'Update user permission', 'admin', NOW(), 'admin', NOW(), 3, 1),
(4, 'DeleteUser', 'Delete user permission', 'admin', NOW(), 'admin', NOW(), 4, 1),
(5, 'CreateRole', 'Create role permission', 'admin', NOW(), 'admin', NOW(), 1, 2),
(6, 'ReadRole', 'Read role permission', 'admin', NOW(), 'admin', NOW(), 2, 2),
(7, 'UpdateRole', 'Update role permission', 'admin', NOW(), 'admin', NOW(), 3, 2),
(8, 'DeleteRole', 'Delete role permission', 'admin', NOW(), 'admin', NOW(), 4, 2),
(9, 'CreatePolicy', 'Create policy permission', 'admin', NOW(), 'admin', NOW(), 1, 4),
(10, 'ReadPolicy', 'Read policy permission', 'admin', NOW(), 'admin', NOW(), 2, 4);