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
}
