using Godot;
using Godot.Collections;

public partial class Humanoid
{
    /* Intelligence
     * 해당 객체는 인지, 기억, 판단과 이동 경로, 행동을 제어하고 이를 수행하는 객체입니다.
     * 플레이어의 경우엔 단순히 키보드와 마우스 입력을 받는 정도에 그치지만,
     * NPC의 경우엔 훨씬 복잡한 구성을 갖습니다.
     * 
     * 정보 수집을 담당하는 Perception
     * 정보 저장을 담당하는 Memory
     * 이동 경로를 담당하는 Pathfinder
     * 각 위치의 중요도를 파악하는 Spacefinder
     * 우선 목표, 행동, 교전 대상 등의 전술적 판단을 내리는 Tactical
     * 
     * 등이 있다...
     */
    public Intelligence intelligence;
    public abstract partial class Intelligence 
    {
        public Humanoid master;
        public Intelligence(Humanoid humanoid)
        {
            master = humanoid;
        }

        public Dictionary<string, Vector2> vectorMap = new()
        {
            { "MoveVec", new Vector2()},
            { "AimPos", new Vector2()},
        };

        public Dictionary<string, bool> commandMap = new()
        {
            { "Reload", false },
            { "FirstWeapon", false },
            { "SecondWeapon", false },
            { "SubWeapon", false },
            { "Inventory", false },
            { "Interact", false },
            { "Fire", false },
            { "FireReleased", false },
            { "Sprint", false },
            { "SprintInit", false },
        };

        public virtual void Process(float delta)
        {
            perception?.Process(delta);
            memory?.Process(delta);
            pathfinder?.Process(delta);
            spacefinder?.Process(delta);
            tactical?.Process(delta);
        }
    }
}