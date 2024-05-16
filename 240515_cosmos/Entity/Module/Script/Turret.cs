using Godot;
using System;

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

        turret.Rotate(time * 0.02f);


        base._Process(delta);
    }



    void Fire()
    {
        Projectile proj = ResourceLoader.Load<PackedScene>("res://Entity/Projectile/Prefab/projectile.tscn").Instantiate() as Projectile;
        proj.Position = firePoint.GlobalPosition;
        proj.Rotation = firePoint.GlobalRotation;
        GetTree().Root.AddChild(proj);
    }

}


