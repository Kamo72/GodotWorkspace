using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library
{
    public partial class UserInterface : Node
    {

        public void ShowAcceptDialog(string title, string descript, string confirm)
        {
            AcceptDialog actDialog = new AcceptDialog();
            actDialog.Visible = true;
            actDialog.Title = title;
            actDialog.DialogText = descript;
            actDialog.OkButtonText = confirm;
            GetParent().AddChild(actDialog);
            actDialog.PopupCentered();
        }
        public Node ControlExchange(string tConName, string tConRoot)
        {
            Node tCon;
            if (GetParent().FindChild(tConName) != null)
                tCon = GetNode("../" + tConName) as Node;
            else
            {
                tCon = ResourceLoader.Load<PackedScene>(tConRoot).Instantiate<Node>();
                GetParent().AddChild(tCon);
            }

            if (tCon is UserInterface n)
                n.SetVisible(true);

            this.SetVisible(false);

            return tCon;
        }

        void VisibleFunc(Node node, bool isVisible) 
        {
            if (node is Control control) 
                control.Visible = isVisible;

            foreach (var i in node.GetChildren())
                VisibleFunc(i, isVisible);
        }

        public void SetVisible(bool isVisible) 
        {
            VisibleFunc(this, isVisible);
        }


    }
}
