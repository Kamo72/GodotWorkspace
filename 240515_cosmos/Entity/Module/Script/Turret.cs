using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Turret : Module
{
    Node2D turret => FindChild("Turret") as Node2D;
    Node2D firePoint => turret.FindChild("FirePoint") as Node2D;

    public override void _EnterTree()
    {
        type = Type.CIWS;
        base._EnterTree();
    }

    float time = 0;
    public override void _Process(double delta)
    {
        time += (float)delta;

        if(time > 1f)
        {
            time--;
            Fire();
        }

        TargetingProcess(delta);
        GD.Print(target);
        base._Process(delta);
    }


    Node2D target = null;
    protected virtual bool IsTargetable(Node2D node)
    {
        //if (node is not Module) return false;
        if (node.GetParent() == GetParent()) return false;
        if ((GlobalPosition - node.GlobalPosition).Length() > 200f ) return false;
        
        return true;
    }  

    public float turretSpeed = 1.2f;
    float targetRad = 0f; 
    void TargetingProcess(double delta)
    {
        if(target != null && IsTargetable(target))
        {
            targetRad = (target.GlobalPosition - GlobalPosition).Angle();
        }
        else{
            targetRad = 0f;
            target = FindNewTarget();
        }

        //Apply rotation
        float toAngle = Extension.FindShortestAngle(
            Mathf.RadToDeg(targetRad),
            Mathf.RadToDeg(turret.GlobalRotation));

        if(turretSpeed * delta > Mathf.Abs(toAngle))
            turret.Rotate(toAngle);
        else
            turret.Rotate(Mathf.Sign(toAngle) * turretSpeed * (float)delta);

        GD.Print(Mathf.RadToDeg(turret.GlobalRotation) + "/" + Mathf.RadToDeg(targetRad) +"/" + toAngle);
    }

    Node2D FindNewTarget()
    {
        Node2D candidate = null;
        float angleDif = 9999f;

        List<Node2D> list = new List<Node2D>();

        
        foreach (Node2D node in GetTree().Root.GetChildren())
        {
            if(IsInstanceValid(node) == false) continue;

            if(node is Ship ship)
                foreach(Node2D node2 in ship.GetChildren())
                    if (node2 is Module && IsTargetable(node2))
                        list.Add(node2);

            if(IsTargetable(node))
                list.Add(node);
        }

        GD.Print(list.Count);
        foreach (Node2D node in list)
        {
            if(IsInstanceValid(node) == false) continue;

            float targetRad = (node.GlobalPosition - GlobalPosition).Angle();
            float toAngle = Extension.FindShortestAngle(
                Mathf.RadToDeg(targetRad),
                Mathf.RadToDeg(turret.GlobalRotation));

            if(Mathf.Abs(toAngle) > angleDif) continue;

            angleDif = Mathf.Abs(toAngle);
            candidate = node;
        }

        return candidate;
    }


    void Fire()
    {
        Projectile proj = ResourceLoader.Load<PackedScene>("res://Entity/Projectile/Prefab/projectile.tscn").Instantiate() as Projectile;
        proj.Position = firePoint.GlobalPosition;
        proj.Rotation = firePoint.GlobalRotation;
        GetTree().Root.AddChild(proj);
    }

}


