



using Godot;
using System.Runtime.InteropServices;

public partial class DoorTransition : Interactable
{
    [Export]
    PackedScene sceneToTransition;
    [Export]
    string doorName;

    public override void Interacted(Humanoid humanoid)
    {
        //if (item == null || humanoid == null) return;
        //if (humanoid.inventory == null) return;

        //bool taken = humanoid.inventory.TakeItem(item);

        //if (!taken) return;

        //GD.Print("DropppedItem interacted!");
        //item.droppedItem = null;
        //GetParent().RemoveChild(this);
    }

    public void DoTransition() 
    {
        //@TODO
    }
}