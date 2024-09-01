using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _favorClient.library;

public partial class SignupInterface : UserInterface
{

    [Export]
    private TextEdit txtId;
    [Export]
    private TextEdit txtName;
    [Export]
    private TextEdit txtPw;

    [Export]
    private Button btnGoBack;
    [Export]
    private Button btnSignup;

    [Export]
    private RichTextLabel richCondition;


    Action requestDisposer;
    public override void _Ready()
    {
        btnGoBack.Pressed += () => {
            this.ControlExchange("LoginInterface", "res://controls/LoginInterface.tscn");
        };

       
        btnSignup.Pressed += () => {

            btnSignup.Disabled = true;
            MainClient.instance.Send(new Packet(Packet.Flag.ACCOUNT_SIGNUP, txtId.Text, txtName.Text, txtPw.Text));

            requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ACCOUNT_SIGNUP_CALLBACK, packet =>
            {
                btnSignup.SetDeferred(Button.PropertyName.Disabled, false);

                //ACCOUNT_SIGNUP_CALLBACK
                bool isSucceed = (bool)packet.value[0];
                if (isSucceed)
                {
                    GD.Print("회원가입 성공 : " + packet.value[1].ToString());
                    CallDeferred("ShowAcceptDialog", "회원가입 성공", packet.value[1].ToString(), "확인");
                    CallDeferred("ControlExchange", "LoginInterface", "res://controls/LoginInterface.tscn");
                }
                else
                {
                    GD.Print("회원가입 실패 : " + packet.value[1].ToString());
                    CallDeferred("ShowAccepDialog", "회원가입 실패", packet.value[1].ToString(), "확인");
                }

                requestDisposer();
            });
        };

    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Node n = this;
        if(n is Control c)
            if (c.Visible == false) return;

        if (MainClient.instance.state != 1) btnSignup.Disabled = false;

        string txtCondition = "";

        switch (MainClient.instance.state)
        {
            case 0:
                txtCondition += "[color=blue] 연결 중...";
                break;
            case 1:
                txtCondition += MainClient.instance.ping > 100 ?
                    "[color=orange] 연결 상태 불량(" + MainClient.instance.ping + ")" :
                    "[color=green] 연결 성공(" + MainClient.instance.ping + ")";
                break;
            case 2:
                txtCondition += "[color=red] 연결 실패(" + Math.Round(MainClient.instance.conTryDelay*10)/10f + "초 뒤 재연결...)";
                break;
        }

        richCondition.Text = "서버 상태 :" + txtCondition;
    }
}
