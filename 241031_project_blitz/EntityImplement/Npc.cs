using Godot;

public partial class Npc : Humanoid
{
    public override void _Ready()
    {
        base._Ready();
        intelligence = new TestIntelligence(this);

        //inventory.firstWeapon.DoEquipItem(new MP_155());
        //inventory.TakeItem(new G12_BuckShot_7p5 { stackNow = 50 });

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

    public override void _Process(double delta)
    {
        base._Process(delta);

        //Visibility Code
        VisibleProcess((float)delta);
        CheckLineOfSight();
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
        Modulate = new(1, 1, 1, visibility);
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
    
    public override void _Draw()
    {
        SetModulate();
        DrawCircle(Vector2.Zero, 35, new(1,1,1, visibility));

        // 실제 조준점을 화면에 그리기
        //if (equippedWeapon != null)
        //    DrawLine(equippedWeapon.Position, (realAimPoint - GlobalPosition).Rotated(equippedWeapon.Rotation - facingDir), Colors.Red, 1);
    }
}

public class TestIntelligence : Humanoid.Intelligence
{
    public TestIntelligence(Humanoid humanoid) : base(humanoid) { }

    ((float range, float angle) peace, (float range, float angle) combat) sight = ((400, 60), (600, 90));
    bool isCombat = false;
    float lostTargetTime = 0f;

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

                        GD.Print($"(vectorMap[\"AimPos\"] - target.Position).Length() < 50f : {(vectorMap["AimPos"] - target.Position).Length()}  < 50f");
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
