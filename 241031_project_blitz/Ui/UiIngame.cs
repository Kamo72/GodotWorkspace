using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class UiIngame : Control
{
    public static UiIngame instance = null;

    public override void _Ready()
    {
        base._Ready();
        instance = this;
    }

    public Conversation conversation => this.FindByName("Conversation") as Conversation;
    public Trade trade => this.FindByName("Trade") as Trade;


}
