using Godot;
using System;

public partial class InventoryPage : Page
{


    Container myStatus => this.FindByName("MyStatus") as VBoxContainer;
    VBoxContainer myInventory => this.FindByName("MyInventory") as VBoxContainer;
    VBoxContainer otherInventory => this.FindByName("OtherInventory") as VBoxContainer;


    
}