using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Character : CharacterBody2D
    {
        public CharacterData.Type type;

        public CharacterData data => CharacterData.GetByType(type);


        public int id;




        public override void _Ready()
        {
        
        }

        public override void _PhysicsProcess(double delta)
        {



            MoveAndSlide();
        }








        public void SetupPlayer(string name)
        {
            //TODO
            //GetNode<Label>("Label").Text = name;
        }

    }
}
