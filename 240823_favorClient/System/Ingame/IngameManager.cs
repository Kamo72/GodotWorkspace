using _favorClient.controls;
using _favorClient.library;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.System.Ingame
{
    public partial class IngameManager : Node
    {
        static IngameManager instance = null;

        public enum State 
        {
            PRE_LOADING,
            INITIATING,
            INGAME,
            FINALIZING,
            POST_LOADING,
        }
        public State state = State.INITIATING;

        public IngameInterface igUI;

        public override void _EnterTree()
        {
            instance = this;
            PreLoadingStart();
            base._EnterTree();
        }

        public override void _Ready()
        {
            Node n = GetNode("../IngameInterface");
            if (n is IngameInterface igUI)
                this.igUI = igUI;
            else
                GD.PrintErr("저기요 인게임 인터페이스가 없어요! 미친 이를 어떠케");
        }


        public void PreLoadingStart() 
        {
        
        }


    }
}
