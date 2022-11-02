namespace IsometricGame.Presentation.Utils
{
    public class AchievementRepository
    {
        public static void UnlockAchievement(Achievement achievement){
            Godot.GD.Print(achievement.ToString());
        }
    }
}