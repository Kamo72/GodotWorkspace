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
        UiProcess((float)delta);
    }


    float bleedAlpha = 0f, bleedAlphaDelay = 2.0f;
    float spAlpha = 0f, spAlphaDelay = 0.4f;
    float stackTime = 0f;
    void UiProcess(float delta) 
    {
        stackTime += delta; 
        if(Player.player == null) return;
        Humanoid.Health health = Player.player.health;

        hp.bar.MaxValue = health.hpMax;
        hp.bar.Value = health.hpNow;
        hp.label.Text = $"{Mathf.CeilToInt(health.hpNow)}/{Mathf.CeilToInt(health.hpMax)}";

        float spRatio = health.spNow / health.spMax;

        spAlpha += (health.spNow < health.spMax ? 1f : -1f) * (float)delta / spAlphaDelay;
        spAlpha = Mathf.Clamp(spAlpha, 0f, 1f);
        float spAlphaReal = !Player.player.movement.sprintMaintain? 0.5f * Mathf.Abs(Mathf.Sin(stackTime * 4f)) : 0.5f;

        spBar.Scale = new Vector2(spRatio, 1f);
        spBar.Modulate = new Color(1f, 1f,1f, spAlpha * spAlphaReal);

        energyBar.MaxValue = health.epMax;
        energyBar.Value = health.epNow;

        waterBar.MaxValue = health.wpMax;
        waterBar.Value = health.wpNow;

        bleedAlpha += (health.bleeding > 0 ? 1f : -1f) * (float)delta/bleedAlphaDelay;
        bleedAlpha = Mathf.Clamp(bleedAlpha, 0f, 1f);
        bleeding.label.Text = $"{Mathf.CeilToInt(health.bleeding)}";
        bleeding.sprite.Modulate = new Color(1f, 0f, 0f, bleedAlpha);
    }



}
