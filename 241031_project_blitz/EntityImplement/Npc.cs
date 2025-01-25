using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Npc : Humanoid, IInteractable
{
    public override void _Ready()
    {
        base._Ready();
        intelligence = new NpcIntelligence(this);

        society = new Society(this, "revelation");
        health = new Health(this, 400);
        //inventory.firstWeapon.DoEquipItem(new MP_155());
        //inventory.TakeItem(new G12_BuckShot_7p5 { stackNow = 50 });
    }

    Control interUI;
    Label talkLabel;
    public override void _EnterTree()
    {
        base._EnterTree();

        WorldManager.interactables.Add(this);

        interUI = ResourceLoader.Load<PackedScene>("res://Prefab/UI/interactionUI.tscn").Instantiate() as Control;
        AddChild(interUI);
        interUI.Position = new Vector2(-28, -57);
        ((Label)interUI.FindByName("Label")).Text = "대화하기";
        interUI.Name = "InteractionUI";

        talkLabel = new Label();
        AddChild(talkLabel);
        talkLabel.Size = new(500, 250);
        talkLabel.Position = new(-250, -300);
        talkLabel.Modulate = Colors.White;
        talkLabel.HorizontalAlignment = HorizontalAlignment.Center;
        talkLabel.VerticalAlignment = VerticalAlignment.Bottom;
        talkLabel.AutowrapMode = TextServer.AutowrapMode.Word;
        talkLabel.ZIndex = 10;
        talkLabel.LabelSettings = new() { 
            FontSize = 24,
            ShadowColor = Colors.Black,
            ShadowSize = 8,
        };



        inventory.firstWeapon.DoEquipItem(new M4A1());
        inventory.backpack.DoEquipItem(new TestBackpack());
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
    }
    public override void _ExitTree()
    {
        base._ExitTree();

        WorldManager.interactables.Remove(this);
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        //Hightlight
        HightlightProcess((float)delta);

        //Visibility Code
        VisibleProcess((float)delta);
        CheckLineOfSight();

        //Talk Code
        TalkAlphaProcess((float)delta);
    }

    //Hightlight
    float highlightValue = 0f;
    float highlightDelay = 0.4f;
    bool isHighlighted => Player.player == null ? false : 
        (GlobalPosition - Player.player.GlobalPosition).Length() < interactableRange;
    void HightlightProcess(float delta)
    {
        highlightValue += (isHighlighted ? delta : -delta) / highlightDelay;
        highlightValue = Math.Clamp(highlightValue, 0f, 1f);

        interUI.Modulate = new Color(1, 1, 1, talkAlpha > 0.01f ? 0f : highlightValue);
    }

    //Visibility Code
    protected float visibility = 0f;
    void VisibleProcess(float delta)
    {
        const float getDelay = 0.3f, lossDelay = 0.7f;

        if (CheckLineOfSight() is Player)
            visibility += delta / getDelay;
        else
            visibility -= delta / lossDelay;

        visibility = visibility < 0f ? 0f : visibility > 1f ? 1f : visibility;

    }
    void SetModulate()
    {
        if(equippedWeapon != null)
            equippedWeapon.Modulate = new(1, 1, 1, visibility);
    }
    public Node CheckLineOfSight()
    {
        Player player = Player.player;

        if (player == null) return null;

        Vector2 from = GlobalPosition;           // Enemy 위치
        Vector2 to = player.GlobalPosition;      // Player 위치

        var spaceState = GetWorld2D().DirectSpaceState;

        var rayParams = new PhysicsRayQueryParameters2D
        {
            From = from,
            To = to,
            Exclude = new Godot.Collections.Array<Rid> { GetRid() } // Enemy 자신을 제외
        };

        var result = spaceState.IntersectRay(rayParams);

        if (result.Count > 0)
        {
            var collider = (Node)result["collider"];

            return collider;
        }
        else
            return null;
    }

    public (bool isVisible, float visibility) CheckLineOfSight(Humanoid target)
    {

        if (target == null) return (false, 0f);

        Vector2 from = GlobalPosition;           // Enemy 위치
        Vector2 to = target.GlobalPosition;      // Player 위치

        var spaceState = GetWorld2D().DirectSpaceState;

        var rayParams = new PhysicsRayQueryParameters2D
        {
            From = from,
            To = to,
            Exclude = new Godot.Collections.Array<Rid> { GetRid() } // Enemy 자신을 제외
        };

        var result = spaceState.IntersectRay(rayParams);

        if (result.Count > 0)
        {
            var collider = (Node)result["collider"];

            return (false, 1f);//this is need to be fixed
        }
        else
            return (true, 1f);//1f is need to be fixed
    }
    public bool CheckLineOfSight(IInteractable target)
    {

        if (target == null) return false;
        if (target is not Interactable) return false;

        Interactable interactable = target as Interactable;    
        Vector2 from = GlobalPosition;           // Enemy 위치
        Vector2 to = interactable.GlobalPosition;      // Player 위치

        var spaceState = GetWorld2D().DirectSpaceState;

        var rayParams = new PhysicsRayQueryParameters2D
        {
            From = from,
            To = to,
            Exclude = new Godot.Collections.Array<Rid> { GetRid() } // Enemy 자신을 제외
        };

        var result = spaceState.IntersectRay(rayParams);

        if (result.Count > 0)
        {
            var collider = (Node)result["collider"];

            return false;
        }
        else
            return true;
    }

    //Interaction Code
    public float interactableRange { get; set; } = 100f;

    public virtual void Interacted(Humanoid humanoid)
    {
        if (!IsInteractable(humanoid)) return;

        //TryTalk("뭐 시발련아");

        Script script = new Script()
        {
            nodeList = new() {
            new Script.Node(){
                name = humanoid.Name,
                job = "aa",
                quote = "안녕하세요",
                pos = 0,
                sprite = Script.GetSprite("Player", "ttest"),
                selections = new(){
                    ("말이 심하시네요.", 1, ()=>{ }),
                    ("일단 맞으세요.", 3, ()=>{ this.health.GetDamage(1000); }),
                }
                },
            new Script.Node(){
                name = humanoid.Name,
                job = "bb",
                quote = "말이 심하시네요",
                pos = 4,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = humanoid.Name,
                job = "aa",
                quote = "그러게요",
                pos = 0,
                sprite = Script.GetSprite("Player", "ttest"),
                action = ()=>{UiIngame.instance.conversation.EndScript();  }
                },
            new Script.Node(){
                name = humanoid.Name,
                job = "일단 맞으세요.",
                quote = "일단 맞으세요.",
                pos = 4,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = humanoid.Name,
                job = "으악.",
                quote = "으악",
                pos = 4,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            },
        };
        UiIngame.instance.conversation.SetScript(script);
    }
    protected virtual bool IsInteractable(Humanoid humanoid)
    {
        if(talkAlpha > 0f)
            return false;

        if(humanoid is Player player)
            return true;

        return false;
    }

    //Talk Code
    protected float talkAlpha = 1f;
    protected float talkAlphaMax => talkLabel.Text.Length == 0? 1 : talkLabel.Text.Length * 0.2f + 1f;

    void TalkAlphaProcess(float delta) 
    {
        talkAlpha -= delta;
        float alpha = talkAlpha < 1f ? talkAlpha / 1f : 1f;
        talkLabel.Modulate = new Color(Colors.White, Mathf.Pow(alpha, 0.5f));
        //GD.Print($"{talkAlpha} / {talkAlphaMax}");
    }

    public void SetTalk(string talkText) 
    {
        talkLabel.Text = talkText;
        talkAlpha = talkAlphaMax;
    }
    public void TryTalk(string talkText)
    {
        if(talkAlpha < 0f)
            SetTalk(talkText);
    }

    public void SetConversation() { }

    public override void _Draw()
    {
        SetModulate();
        DrawCircle(Vector2.Zero, 35, new(1,1,1, visibility));
    }
}

public class NpcIntelligence : Humanoid.Intelligence
{
    public NpcIntelligence(Humanoid humanoid) : base(humanoid)
    {
        memory = new Memory(this);
        pathfinder = new Pathfinder(this);
        perception = new Perception(this);
        spacefinder = new Spacefinder(this);
        tactical = new Tactical(this);
    }

    protected ((float range, float angle) peace, (float range, float angle) combat) sight = ((400, 60), (600, 90));
    protected bool isCombat = false;
    protected float lostTargetTime = 0f;

    public override void Process(float delta)
    {
        EnemySearching(delta);

        commandMap["Reload"] = IsNeedToReload();
    }

    void EnemySearching(float delta) 
    {
        var nowSight = isCombat ? sight.combat : sight.peace;

        var target = Player.player;
        if(target == null) return;
        Vector2 vecTarget = master.Position - target.Position, vecAim = master.Position - vectorMap["AimPos"];

        commandMap["Fire"] = false;
        commandMap["FireReleased"] = true;
        if (vecTarget.Length() < nowSight.range)
            if (Mathf.Abs(vecTarget.AngleTo(vecAim)) < Mathf.DegToRad(nowSight.angle))
                if (master is Npc npc)
                {
                    if (npc.CheckLineOfSight() is Player)
                    {
                        isCombat = true;
                        lostTargetTime = 4f;
                        vectorMap["AimPos"] = vectorMap["AimPos"].Lerp(target.Position, 0.1f);

                        //GD.Print($"(vectorMap[\"AimPos\"] - target.Position).Length() < 50f : {(vectorMap["AimPos"] - target.Position).Length()}  < 50f");
                        if ((vectorMap["AimPos"] - target.Position).Length() < 50f)
                            if(!IsNeedToReload())
                                commandMap["Fire"] = true;
                    }
                }

        if (lostTargetTime < 0f)
            isCombat = false;
    }

    bool IsNeedToReload()
    {
        if (!master.isEquiped) return false;
        if (master.nowEquip == null) return false;

        if (master.nowEquip.item == null) return false;
        if (master.nowEquip.item is WeaponItem weapon)
        {
            switch (weapon.weaponStatus.typeDt.magazineType)
            { 
                case MagazineType.MAGAZINE:
                    if(weapon.magazine == null || weapon.magazine.ammoCount == 0)
                        return true;
                    break;
                case MagazineType.TUBE:
                    if (weapon.chamber == null)
                        return true;
                    break;
            }
        }

        return false;
    }

}
