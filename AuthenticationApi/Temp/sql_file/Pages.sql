INSERT INTO Pages (Id, Name, RoutePath, OrderIndex, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, ModulesId)
VALUES
  (1, 'UserList', '/users', 1, 'admin', NOW(), 'admin', NOW(), 1),
  (2, 'ReportDashboard', '/reports', 2, 'admin', NOW(), 'admin', NOW(), 2);