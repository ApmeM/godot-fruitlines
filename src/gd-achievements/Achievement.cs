using System;

public class Achievement : Godot.Object
{
    public bool Achieved;
    public int CurrentProgress;
    public int Goal;
    public string Name;
    public string Description;
    public string IconPath;
    public bool Hidden;
    public DateTime? UnlockDate;
}
