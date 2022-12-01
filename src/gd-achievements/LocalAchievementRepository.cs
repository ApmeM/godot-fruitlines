using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace Antilines.Presentation.Utils
{
    public class LocalAchievementRepository : IAchievementRepository
    {
        private class AchievementDefinition
        {
            [JsonProperty("goal")]
            public int Goal;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("description")]
            public string Description;

            [JsonProperty("icon_path")]
            public string IconPath;

            [JsonProperty("is_hidden")]
            public bool Hidden;
        }

        private class UserAchievement
        {
            [JsonProperty("achieved")]
            public bool Achieved;
            [JsonProperty("current_progress")]
            public int CurrentProgress;
            [JsonProperty("unlock_date")]
            public DateTime? UnlockDate;
        }

        const string ACHIEVEMENTS_DATA = "user://achievements.json";
        const string ACHIEVEMENTS_DEFINITION = "res://gd-achievements/achievements.json";

        public bool ProgressAchievement(string key, int progress)
        {
            var achievements = EnsureAchievementsLoaded();
            achievements[key].CurrentProgress = Math.Min(progress + achievements[key].CurrentProgress, achievements[key].Goal);
            SaveAchievementData(achievements);
            if (achievements[key].CurrentProgress < achievements[key].Goal)
            {
                return false;
            }

            return UnlockAchievement(key);
        }

        public bool UnlockAchievement(string key)
        {
            var achievements = EnsureAchievementsLoaded();
            if (!achievements.ContainsKey(key))
            {
                GD.PrintErr($"Achievement System: Attempt to get an achievement on {key}, key doesn't exist.");
                return false;
            }

            var currentAchievement = achievements[key];
            if (currentAchievement.Achieved)
            {
                return false;
            }

            currentAchievement.Achieved = true;
            currentAchievement.UnlockDate = DateTime.Now;
            SaveAchievementData(achievements);
            return true;
        }

        public Achievement GetAchievement(string key)
        {
            var achievements = EnsureAchievementsLoaded();
            return achievements[key];
        }

        public IEnumerable<Achievement> GetForList()
        {
            var achievements = EnsureAchievementsLoaded();
            return achievements.Values.Where(a => !a.Hidden || a.Achieved).OrderByDescending(a => a.Achieved);
        }

        public void ResetAchievements()
        {
            var achievements = EnsureAchievementsLoaded();
            foreach (var key in achievements)
                key.Value.Achieved = false;

            SaveAchievementData(achievements);
        }

        private Achievement ToAchievement(AchievementDefinition definition, UserAchievement achievement)
        {
            return new Achievement
            {
                Achieved = achievement?.Achieved ?? false,
                CurrentProgress = achievement?.CurrentProgress ?? 0,
                UnlockDate = achievement?.UnlockDate,
                Goal = definition.Goal,
                Name = definition.Name,
                IconPath = definition.IconPath,
                Description = definition.Description
            };
        }
        private UserAchievement ToData(Achievement achievement)
        {
            return new UserAchievement
            {
                Achieved = achievement.Achieved,
                CurrentProgress = achievement.CurrentProgress,
                UnlockDate = achievement.UnlockDate,
            };
        }

        private void SaveAchievementData(Dictionary<string, Achievement> achievements)
        {
            var userFileJson = new File();

            if (!userFileJson.FileExists(ACHIEVEMENTS_DATA))
            {
                GD.PrintErr("Achievement System: Can't open achievements data. It doesn't exists on device");
            }

            var data = achievements.ToDictionary(a => a.Key, a => ToData(a.Value));

            GD.Print("Achievement System: Saving achievements  " + string.Join(", ", data.Keys));
            userFileJson.Open(ACHIEVEMENTS_DATA, File.ModeFlags.Write);
            userFileJson.StoreString(JsonConvert.SerializeObject(data));
            userFileJson.Close();
        }

        private Dictionary<string, Achievement> EnsureAchievementsLoaded()
        {
            var definition = LoadAchievementDefinitions();
            var data = LoadAchievementData();
            return MergeDefinitionAndData(definition, data);
        }

        private Dictionary<string, AchievementDefinition> LoadAchievementDefinitions()
        {
            var file = new File();
            file.Open(ACHIEVEMENTS_DEFINITION, File.ModeFlags.Read);

            var data = JsonConvert.DeserializeObject<Dictionary<string, AchievementDefinition>>(file.GetAsText());

            file.Close();
            return data;
        }

        private Dictionary<string, UserAchievement> LoadAchievementData()
        {
            var file = new File();
            if (!file.FileExists(ACHIEVEMENTS_DATA))
            {
                return null;
            }

            file.Open(ACHIEVEMENTS_DATA, File.ModeFlags.Read);
            var data = JsonConvert.DeserializeObject<Dictionary<string, UserAchievement>>(file.GetAsText());
            file.Close();

            return data;
        }

        private Dictionary<string, Achievement> MergeDefinitionAndData(Dictionary<string, AchievementDefinition> definition, Dictionary<string, UserAchievement> data)
        {
            GD.Print("Achievement System: Loading achievements " + string.Join(", ", definition.Keys));
            if (data == null || data.Count == 0)
            {
                return definition.ToDictionary(a => a.Key, a => ToAchievement(a.Value, null));
            }

            return definition.ToDictionary(a => a.Key, a => ToAchievement(a.Value, data.ContainsKey(a.Key) ? data[a.Key] : null));
        }
    }
}