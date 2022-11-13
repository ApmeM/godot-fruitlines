using Godot;
using IsometricGame.Presentation.Utils;

public class AchievementManager : Node
{
    private IAchievementRepository achievementRepository = new LocalAchievementRepository();

    [Signal]
    public delegate void AchievementUnlocked(Achievement data);

    public void ProgressAchievement(string key, int progress)
    {
        ProcessAchievement(key, achievementRepository.ProgressAchievement(key, progress));
    }

    public void UnlockAchievement(string key)
    {
        ProcessAchievement(key, achievementRepository.UnlockAchievement(key));
    }

    public void ProcessAchievement(string key, bool isOperationSuccess)
    {
        if (!isOperationSuccess)
        {
            return;
        }

        var data = achievementRepository.GetAchievement(key);
        this.EmitSignal(nameof(AchievementUnlocked), data);
    }
}