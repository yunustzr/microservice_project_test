INSERT INTO PolicyVersion (Id, Version, EffectiveDate, Changes, PolicyId)
VALUES
  (1, '1.0.0', NOW(), '{"changes": "Initial version"}', 1),
  (2, '1.0.1', NOW(), '{"changes": "Minor update"}', 1);