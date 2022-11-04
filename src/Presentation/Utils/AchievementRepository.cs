namespace IsometricGame.Presentation.Utils
{
    using System.Collections.Generic;

    public class AchievementRepository
    {
        private Dictionary<Achievement, string> achievementGoogleIDMap = new Dictionary<Achievement, string>{
            {Achievement.LinesMultiply3, "CgkI9q6y54MQEAIQAQ"},
            {Achievement.LinesMultiply5, "CgkI9q6y54MQEAIQBA"},
            {Achievement.LinesMultiply7, "CgkI9q6y54MQEAIQBQ"},
            {Achievement.LinesRow6, "CgkI9q6y54MQEAIQBg"},
            {Achievement.LinesRow7, "CgkI9q6y54MQEAIQBw"},
            {Achievement.LinesRow9, "CgkI9q6y54MQEAIQCA"},
            {Achievement.AntilinesScore90, "CgkI9q6y54MQEAIQCQ"},
            {Achievement.AntilinesScore30, "CgkI9q6y54MQEAIQCg"},
            {Achievement.AntilinesScore9, "CgkI9q6y54MQEAIQCw"},
            {Achievement.GroupRemove10, "CgkI9q6y54MQEAIQDQ"},
            {Achievement.GroupRemoveAllType, "CgkI9q6y54MQEAIQDg"},
            {Achievement.GroupClearAll, "CgkI9q6y54MQEAIQDw"}
        };

        public static void UnlockAchievement(Achievement achievement)
        {
            Godot.GD.Print(achievement.ToString());
        }
    }
}