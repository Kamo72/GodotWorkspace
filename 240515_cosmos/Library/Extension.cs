using Godot;
using System;

public static partial class Extension
{
    public static Node FindByName(this Node node, string name)
    {
        foreach(var n in node.GetChildren())
        {
            if(n.Name == name) return n;

            Node res = FindByName(n, name);

            if(res != null) return res; 
        }

        return null;
    }
    public static Node FindByName(this SceneTree tree, string name)
    {
        Node root = tree.Root.GetChild(0);

        return root.FindByName(name);
    }
    
    public static float FindShortestAngle(float direction, float targetDirection)
    {

        while (direction < 0) direction += 360;
        while (targetDirection < 0) targetDirection += 360;


        // Normalize the angles to be between 0 and 360 degrees
        direction %= 360;
        targetDirection %= 360;

        // Calculate the absolute difference between the two angles
        float angleDifference = targetDirection - direction;

        // Ensure the angle is in the range of -180 to 180 degrees
        if (angleDifference <= -180)
        {
            angleDifference += 360;
        }
        else if (angleDifference > 180)
        {
            angleDifference -= 360;
        }

        return angleDifference;
    }
}
