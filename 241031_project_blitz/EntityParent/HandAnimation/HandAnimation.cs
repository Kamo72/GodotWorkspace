using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class HandAnimation : Node2D
{
    protected Humanoid master => GetParent<Humanoid>();
    public Sprite2D sprite2D => this.FindByName("Sprite2D") as Sprite2D;
}