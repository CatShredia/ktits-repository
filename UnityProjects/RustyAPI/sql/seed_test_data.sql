INSERT INTO "Users" ("UserId", "Username", "PasswordHash", "Coins", "LastCompletedLevelIndex", "CreatedAt", "LastLoginAt")
VALUES
('11111111-1111-1111-1111-111111111111', 'rusty_alex', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 420, 2, NOW(), NOW()),
('22222222-2222-2222-2222-222222222222', 'rusty_nina', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 360, 1, NOW(), NOW()),
('33333333-3333-3333-3333-333333333333', 'rusty_igor', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 290, 1, NOW(), NOW()),
('44444444-4444-4444-4444-444444444444', 'rusty_lena', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 190, 0, NOW(), NOW()),
('55555555-5555-5555-5555-555555555555', 'rusty_guest', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 120, 0, NOW(), NOW())
ON CONFLICT ("Username") DO NOTHING;

INSERT INTO "UserLevelProgresses" ("UserId", "LevelKey", "LevelIndex", "StarsCollected", "Completed", "UpdatedAt")
SELECT u."Id", p."LevelKey", p."LevelIndex", p."StarsCollected", p."Completed", NOW()
FROM (VALUES
    ('rusty_alex', 'FirstLevelData', 0, 3, TRUE),
    ('rusty_alex', 'SecondLevel', 1, 3, TRUE),
    ('rusty_alex', 'ThirdLevel', 2, 1, FALSE),
    ('rusty_nina', 'FirstLevelData', 0, 3, TRUE),
    ('rusty_nina', 'SecondLevel', 1, 2, FALSE),
    ('rusty_nina', 'ThirdLevel', 2, 0, FALSE),
    ('rusty_igor', 'FirstLevelData', 0, 3, TRUE),
    ('rusty_igor', 'SecondLevel', 1, 1, FALSE),
    ('rusty_lena', 'FirstLevelData', 0, 2, FALSE),
    ('rusty_guest', 'FirstLevelData', 0, 1, FALSE)
) AS p("Username", "LevelKey", "LevelIndex", "StarsCollected", "Completed")
JOIN "Users" u ON u."Username" = p."Username"
ON CONFLICT ("UserId", "LevelKey") DO NOTHING;
