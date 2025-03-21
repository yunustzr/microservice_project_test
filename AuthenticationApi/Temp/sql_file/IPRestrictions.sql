INSERT INTO IPRestrictions (Id, IPAddress, Subnet, IsAllowed, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, ResourceId)
VALUES
  (1, '192.168.1.0', '255.255.255.0', TRUE, 'admin', NOW(), 'admin', NOW(), 1),
  (2, '10.0.0.0', '255.0.0.0', FALSE, 'admin', NOW(), 'admin', NOW(), 2);