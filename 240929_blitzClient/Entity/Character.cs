using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Character : Humanoid
    {
        [Export]
        Label nameTxt;
        public override void _EnterTree()
        {
            rpcOwner = int.Parse(Name);
            base._EnterTree();
        }
        public void SetupPlayer(string name)
        {
            //TODO
            nameTxt.Text = name;
        }

    }


}
