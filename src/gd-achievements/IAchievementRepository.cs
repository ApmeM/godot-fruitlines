using Godot;
using Antilines.Presentation.Utils;

public interface IAchievementRepository
{
    bool ProgressAchievement(string key, int progress);
    bool UnlockAchievement(string key);
    Achievement GetAchievement(string key);
}