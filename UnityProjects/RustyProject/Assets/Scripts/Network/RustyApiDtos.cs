using System;

namespace RustyProject.Network
{
    [Serializable]
    public class AuthRequestDto
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class AuthResponseDto
    {
        public int id;
        public string userId;
        public string username;
        public string token;
        public string expiresAt;
    }

    [Serializable]
    public class LevelProgressDto
    {
        public string levelKey;
        public int levelIndex;
        public int starsCollected;
        public bool completed;
    }

    [Serializable]
    public class UpdateCoinsDto
    {
        public int coinsDelta;
    }

    [Serializable]
    public class UpdateProgressDto
    {
        public int lastCompletedLevelIndex;
        public LevelProgressDto[] levelProgresses;
    }

    [Serializable]
    public class UserProfileDto
    {
        public int id;
        public string userId;
        public string username;
        public int coins;
        public int lastCompletedLevelIndex;
        public int totalStars;
        public string createdAt;
        public string lastLoginAt;
        public LevelProgressDto[] levelProgresses;
    }

    [Serializable]
    public class LeaderboardEntryDto
    {
        public int rank;
        public string username;
        public int coins;
        public int lastCompletedLevelIndex;
        public int totalStars;
    }

    [Serializable]
    public class LeaderboardResponseWrapper
    {
        public LeaderboardEntryDto[] items;
    }
}
