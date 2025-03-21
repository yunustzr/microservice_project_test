INSERT INTO Policy  (Id, Name, Description, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsActive, Functionality, IsPublic)
VALUES
(1, 'AdminPolicy', 'Admin policy', 'admin', NOW(), 'admin', NOW(), 1, 'FullAccess', 'No'),
(2, 'UserPolicy', 'User policy', 'admin', NOW(), 'admin', NOW(), 1, 'LimitedAccess', 'No'),
(3, 'AuditorPolicy', 'Auditor policy', 'admin', NOW(), 'admin', NOW(), 1, 'ReadOnly', 'No'),
(4, 'ManagerPolicy', 'Manager policy', 'admin', NOW(), 'admin', NOW(), 1, 'ManageUsers', 'No'),
(5, 'GuestPolicy', 'Guest policy', 'admin', NOW(), 'admin', NOW(), 1, 'ViewOnly', 'Yes'),
(6, 'DeveloperPolicy', 'Developer policy', 'admin', NOW(), 'admin', NOW(), 1, 'APIAccess', 'No'),
(7, 'SupportPolicy', 'Support policy', 'admin', NOW(), 'admin', NOW(), 1, 'TicketAccess', 'No'),
(8, 'AnalystPolicy', 'Analyst policy', 'admin', NOW(), 'admin', NOW(), 1, 'ReportAccess', 'No'),
(9, 'TesterPolicy', 'Tester policy', 'admin', NOW(), 'admin', NOW(), 1, 'TestAccess', 'No'),
(10, 'PublicPolicy', 'Public policy', 'admin', NOW(), 'admin', NOW(), 1, 'PublicAccess', 'Yes');