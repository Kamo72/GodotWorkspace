using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Conversation : Control
{
    //추가로 인게임 화면 사이에 0,0,0, 0.5 정도 되는 필터를 두는게 좋을 듯

    Label quoteTxt => ((Label)this.FindByName("QuoteText"));
    Label nameTxt => ((Label)this.FindByName("NameText"));
    Label jobTxt => ((Label)this.FindByName("JobText"));

    Button[] selectionBtn;

    List<Sprite2D> sprites;
    List<bool> spriteVisibilitys;

    public bool isConversation = false;
    public bool inSelection => selectionBtn[0].Visible;
    float alpha = 0f;

    Script script = null;
    int scriptNow = 0;


    public override void _Ready()
    {
        spriteVisibilitys = new() { false, false, false, false, false, };
        sprites = new() {
            this.FindByName("illust_0") as Sprite2D,
            this.FindByName("illust_1") as Sprite2D,
            this.FindByName("illust_2") as Sprite2D,
            this.FindByName("illust_3") as Sprite2D,
            this.FindByName("illust_4") as Sprite2D,
        };

        selectionBtn = new Button[] {
            (Button)this.FindByName("Selection_0"),
            (Button)this.FindByName("Selection_1"),
            (Button)this.FindByName("Selection_2"),
            (Button)this.FindByName("Selection_3"),
        };
    }
    public override void _Process(double delta)
    {
        alpha = (float)Math.Clamp(alpha + (isConversation ? delta : -delta), 0f, 1f);
        Visible = alpha > 0f;
        Modulate = new Color(1f, 1f, 1f, alpha);

        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite2D sprite = sprites[i];
            bool visibility = spriteVisibilitys[i];

            var alpha = (float)Math.Clamp(sprite.Modulate.A + (visibility ? delta : -delta), sprite.Texture == null ? 0f : 0.7f, 1f);
            sprite.Modulate = new Color(alpha, alpha, alpha, alpha);
        }
    }

    //Script 제어 기본
    public void SetScript(Script script)
    {
        this.script = script;
        isConversation = true;

        //페이지 초기화
        scriptNow = 0;

        //스프라이트 초기화
        spriteVisibilitys = new() { false, false, false, false, false, };
        foreach (Sprite2D sprite in sprites) 
            sprite.Modulate = new Color(0,0,0,0);

        //텍스트 초기화
        quoteTxt.Text = "";
        nameTxt.Text = "";
        jobTxt.Text = "";
        
        ApplyScript(this.script, scriptNow);
    }

    public void EndScript()
    { 
        //스크립트 종료
        script = null;
        isConversation = false;
    }

    public void ProcessScript()
    {
        if (script == null)
            return;

        //진행, 끝일 경우 EndScript()
        if ((script.nodeList.Count - 1) < ++scriptNow)
            EndScript();
        else if(!inSelection)
            ApplyScript(this.script, scriptNow);
    }

    void ApplyScript(Script script, int page)
    {
        //텍스트 적용
        quoteTxt.Text = script.nodeList[page].quote;
        nameTxt.Text = script.nodeList[page].name;
        jobTxt.Text = script.nodeList[page].job;

        //스프라이트 적용
        int pos = script.nodeList[page].pos;
        if (script.nodeList[page].sprite != null || script.nodeList[page].sprite != "")
        {
            sprites[pos].Texture = ResourceLoader.Load<Texture2D>(script.nodeList[page].sprite);
            int c = 0;
            spriteVisibilitys = new()
            {
                c++ == pos,
                c++ == pos,
                c++ == pos,
                c++ == pos,
                c++ == pos,
            };

        }

        //람다식 적용
        script.nodeList[page].action?.Invoke();

        //선택지 적용
        if (script.nodeList[page].selections != null)
            SetSelections(script.nodeList[page].selections);
    }


    void SetSelections(List<(string text, int page, Action action)> selections)
    {
        var selectionsCount = selections.Count;

        if (0 == selectionsCount || selectionsCount > 4) 
            throw new IndexOutOfRangeException("SetSelections - 0 == selectionsCount || selectionsCount > 4 is true!!");

        for (int i = 0; i < 4; i++) 
        {
            Button button = selectionBtn[i];
            button.Visible = i < selectionsCount;
            if (i < selectionsCount)
            {
                var page = selections[i].page;
                var action = selections[i].action;
                button.Text = selections[i].text;
                button.ButtonDown += () =>
                {
                    scriptNow = page;
                    action();
                    ApplyScript(script, page);
                    EndSelections();
                };
            }
        }


    }

    void EndSelections()
    {
        for (int i = 0; i < 4; i++)
        {
            Button button = selectionBtn[i];
            button.Visible = false;
        }
    }

}


public class Script
{
    public List<Node> nodeList = new() {
        new Node(){
            name = null,
            quote = null,
            job = null,
            pos = 0,
            sprite = null,
            selections = null,
            action = null,
        },
    };

    public struct Node
    {
        public string name { get; set; }    //화자
        public string quote { get; set; }      //텍스트
        public string job { get; set; }        //직업
        public int pos { get; set; }    //스프라이트 위치 0~4
        public string sprite { get; set; }
        public List<(string text, int page, Action action)> selections { get; set; }
        public Action action { get; set; }
    }

    public static string GetSprite(string character, string action) => $"res://Asset/IMG-Illust/{character}/{action}.png";
}



/* 아래와 같이 스크립트 적용
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
 */