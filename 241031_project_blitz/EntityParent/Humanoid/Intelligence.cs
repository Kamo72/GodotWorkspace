using Godot;
using Godot.Collections;

public partial class Humanoid 
{
    public Intelligence intelligence;
    public abstract class Intelligence 
    {
        public Humanoid master;
        public Intelligence(Humanoid humanoid)
        {
            master = humanoid;
        }

        public Dictionary<string, Vector2> vectorMap = new()
        {
            { "MoveVec", new Vector2()},
            { "AimPos", new Vector2()},
        };

        public Dictionary<string, bool> commandMap = new()
        {
            { "Reload", false },
            { "FirstWeapon", false },
            { "SecondWeapon", false },
            { "SubWeapon", false },
            { "Inventory", false },
            { "Interact", false },
            { "Fire", false },
            { "FireReleased", false },
        };

        public abstract void Process(float delta);
    }
}