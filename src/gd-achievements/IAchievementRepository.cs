using Godot;
using IsometricGame.Presentation.Utils;

public interface IAchievementRepository
{
    bool ProgressAchievement(string key, int progress);
    bool UnlockAchievement(string key);
    Achievement GetAchievement(string key);
}