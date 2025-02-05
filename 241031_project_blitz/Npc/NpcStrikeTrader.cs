using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//스태쉬 관리자
public partial class NpcStrikeTrader : Npc
{
    Trader trader;
    public override void _Ready()
    {
        base._Ready();
        Name = "유시온";
        job = "화력팀 수석 무기기술자";

        //intelligence = new Intelligence();
    }
    public override void _EnterTree()
    {
        base._EnterTree();

        inventory = new Inventory(this);
        trader = TraderManager.instance.traderLibrary["medic"];
    }


    public override void Interacted(Humanoid humanoid)
    {
        if (!IsInteractable(humanoid)) return;

        //TryTalk("뭐 시발련아");

        Script script = new Script()
        {
            nodeList = new() {
                //0
            new Script.Node(){
                name = Name,
                job = job,
                quote = "안녕하세요. 무슨 볼일이신가요?",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                selections = new()
                    {
                        new ("물건을 좀 보고싶어",1,()=>{
                            Trade.instance.OpenTrade(trader);
                            UiIngame.instance.conversation.EndScript();
                        }),
                        new ("요즘 좀 어때?",3,()=>{ }),
                        new ("별일 아냐",1,()=>{ }),
                    },
                },

            
            //2
            new Script.Node(){
                name = Name,
                job = job,
                quote = "네, 다른 용건이 있으시면 또 찾아주세요.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = Name,
                job = job,
                quote = "네, 다른 용건이 있으시면 또 찾아주세요.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                action = ()=>UiIngame.instance.conversation.EndScript(),
                },

            //4
            new Script.Node(){
                name = Name,
                job = job,
                quote = "평소와 별 다를건 없어요. 총 닦고, 조이고, 기름치죠.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = Name,
                job = job,
                quote = "평소와 별 다를건 없어요. 총 닦고, 조이고, 기름치죠.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                action = ()=>UiIngame.instance.conversation.EndScript(),
                },
            },
        };
        UiIngame.instance.conversation.SetScript(script);
    }
    protected override bool IsInteractable(Humanoid humanoid)
    {
        if (talkAlpha > 0f)
            return false;

        if (humanoid is Player player)
            return true;

        return false;
    }


}

