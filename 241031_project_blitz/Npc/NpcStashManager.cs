using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//스태쉬 관리자
public partial class NpcStashManager : Npc
{
    public override void _Ready()
    {
        base._Ready();
        Name = "박서준";
        job = "보급부 배급담당관";

    }
    public override void _EnterTree()
    {
        base._EnterTree();

        inventory = new Inventory(this);
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
                quote = "좋은 아침입니다. 배급 관련건이신가요?",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = Name,
                job = job,
                quote = "개인 창고는 오른쪽을 이용해주세요.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                selections = new()
                    {
                        new ("당신은 어떤 일을 하시나요?",2,()=>{ }),
                        new ("앙 기모찌",4,()=>{ }),
                        new ("맞짱 까자",6,()=>{ }),
                    },
                },

            
            //2
            new Script.Node(){
                name = Name,
                job = job,
                quote = "저는 배급담당관으로 일하고 있는 박서준입니다.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = Name,
                job = job,
                quote = "저는 배급담당관으로 일하고 있는 박서준입니다.",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                action = ()=>UiIngame.instance.conversation.EndScript(),
                },

            //4
            new Script.Node(){
                name = Name,
                job = job,
                quote = "그게 갑자기 무슨 소리세요...",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = Name,
                job = job,
                quote = "그게 갑자기 무슨 소리세요...",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                action = ()=>UiIngame.instance.conversation.EndScript(),
                },
            //6
            new Script.Node(){
                name = Name,
                job = job,
                quote = "히익",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                },
            new Script.Node(){
                name = Name,
                job = job,
                quote = "히익",
                pos = 2,
                sprite = Script.GetSprite("Player", "ttest"),
                action = () => UiIngame.instance.conversation.EndScript(),
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

