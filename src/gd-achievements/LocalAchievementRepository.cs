using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace Antilines.Presentation.Utils
{
    public class LocalAchievementRepository : IAchievementRepository
    {
        private class AchievementItem
        {
            [JsonProperty("achieved")]
            public bool Achieved;
            [JsonProperty("current_progress")]
            public int CurrentProgress;
            [JsonProperty("goal")]
            public int Goal;
            [JsonProperty("name")]
            public string Name;
            [JsonProperty("icon_path")]
            public string IconPath;
            [JsonProperty("is_hidden")]
            public bool Hidden;
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
            SaveAchievementData(achievements);
            return true;
        }

        public Achievement GetAchievement(string key)
        {
            var achievements = EnsureAchievementsLoaded();
            return ToAchievement(achievements[key]);
        }

        public IEnumerable<Achievement> GetForList()
        {
            var achievements = EnsureAchievementsLoaded();
            return achievements.Values.Where(a => !a.Hidden || a.Achieved).OrderByDescending(a => a.Achieved).Select(ToAchievement);
        }

        public void ResetAchievements()
        {
            var achievements = EnsureAchievementsLoaded();
            foreach (var key in achievements)
                key.Value.Achieved = false;

            SaveAchievementData(achievements);
        }

        private Achievement ToAchievement(AchievementItem data)
        {
            return new Achievement
            {
                Achieved = data.Achieved,
                CurrentProgress = data.CurrentProgress,
                Goal = data.Goal,
                Name = data.Name,
                IconPath = data.IconPath,
            };
        }

        private void SaveAchievementData(Dictionary<string, AchievementItem> data)
        {
            var userFileJson = new File();

            if (!userFileJson.FileExists(ACHIEVEMENTS_DATA))
            {
                GD.PrintErr("Achievement System: Can't open achievements data. It doesn't exists on device");
            }

            GD.Print("Achievement System: Saving achievements  " + string.Join(", ", data.Keys));
            userFileJson.Open(ACHIEVEMENTS_DATA, File.ModeFlags.Write);
            userFileJson.StoreString(JsonConvert.SerializeObject(data));
            userFileJson.Close();
        }

        private Dictionary<string, AchievementItem> EnsureAchievementsLoaded()
        {
            var definition = LoadAchievementDefinitions();
            var data = LoadAchievementData();
            return MergeDefinitionAndData(definition, data);
        }

        private Dictionary<string, AchievementItem> LoadAchievementDefinitions()
        {
            var file = new File();
            file.Open(ACHIEVEMENTS_DEFINITION, File.ModeFlags.Read);

            var data = JsonConvert.DeserializeObject<Dictionary<string, AchievementItem>>(file.GetAsText());

            file.Close();
            return data;
        }

        private Dictionary<string, AchievementItem> LoadAchievementData()
        {
            var file = new File();
            if (!file.FileExists(ACHIEVEMENTS_DATA))
            {
                return null;
            }

            file.Open(ACHIEVEMENTS_DATA, File.ModeFlags.Read);
            var data = JsonConvert.DeserializeObject<Dictionary<string, AchievementItem>>(file.GetAsText());
            file.Close();

            return data;
        }

        private Dictionary<string, AchievementItem> MergeDefinitionAndData(Dictionary<string, AchievementItem> definition, Dictionary<string, AchievementItem> data)
        {
            GD.Print("Achievement System: Loading achievements " + string.Join(", ", definition.Keys));
            if (data == null || data.Count == 0)
            {
                return definition;
            }

            var toDelete = data.Keys.Except(definition.Keys).ToList();
            GD.Print("Achievement System: Obsolete achievements removed" + string.Join(", ", toDelete));
            foreach (var item in toDelete)
            {
                data.Remove(item);
            }

            var toInsert = definition.Keys.Except(data.Keys).ToList();
            GD.Print("Achievement System: New achievements added " + string.Join(", ", toInsert));
            foreach (var item in toInsert)
            {
                data[item] = definition[item];
            }

            return data;
        }
    }
}