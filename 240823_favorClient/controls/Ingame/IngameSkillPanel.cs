using _favorClient.library;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.controls.Ingame
{
    public partial class IngameSkillPanel : UserInterface
    {
        [Export]
        public Label keyTxt;

        [Export]
        public TextureRect skillTex;

        [Export]
        public Label cooldownTxt;

        [Export]
        public Panel cooldownPanel;


        public void SetInit(string keyName, string path) 
        {
            Texture2D tex = ResourceLoader.Load<Texture2D>(path);

            skillTex.Texture = tex;
            keyTxt.Text = keyName;
        }

        public void RefreshCooldown(float cooldown)
        {
            decimal data = (decimal)Math.Floor(cooldown * 10) / 10;

            cooldownTxt.Text = data.ToString();
            cooldownPanel.Modulate = new Color(1,1,1, cooldown > 0? 1 : 0);
        }

    }
}
