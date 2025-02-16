using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Stats : Control 
{

    private (ProgressBar bar, Label label) hp;
    private ProgressBar energyBar, waterBar;

    private Sprite2D spBar;
    private (Control control, Sprite2D sprite, Label label) bleeding;

    public override void _Ready()
    {
        hp = (
            this.FindByName("HpBar") as ProgressBar,
            this.FindByName("HpLabel") as Label);
        energyBar = this.FindByName("EnergyBar") as ProgressBar;
        waterBar = this.FindByName("WaterBar") as ProgressBar;
        spBar = this.FindByName("SpBar") as Sprite2D;
        bleeding = (
            this.FindByName("Bleeding") as Control, 
            this.FindByName("BleedingSpr") as Sprite2D, 
            this.FindByName("BleedingLabel") as Label);
    }
    public override void _Process(double delta)
    {
        UiProcess();
    }


    float bleedAlpha = 0f, bleedAlphaDelay = 2.0f;
    float spAlpha = 0f, spAlphaDelay = 2.0f;
    void UiProcess() 
    {
        if(Player.player == null) return;
        Humanoid.Health health = Player.player.health;

        hp.bar.MaxValue = health.hpMax;
        hp.bar.Value = health.hpNow;
        hp.label.Text = $"{Mathf.CeilToInt(health.hpNow)}/{Mathf.CeilToInt(health.hpMax)}";

        float spRatio = health.spNow / health.spMax;
        spBar.Scale = new Vector2(spRatio, 1f);

        energyBar.MaxValue = health.epMax;
        energyBar.Value = health.epNow;

        waterBar.MaxValue = health.wpMax;
        waterBar.Value = health.wpNow;


        bleeding.label.Text = $"{health.bleeding}";
    }



}
