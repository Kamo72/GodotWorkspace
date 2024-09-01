using Godot;
using System;
using System.Net.Sockets;
using _favorClient.library;
using _favorClient.controls;
using System.Threading;

public partial class LoginInterface : UserInterface
{
    [Export]
    private TextEdit txtId;
    [Export]
    private TextEdit txtPw;

    [Export]
    private Button btnSignin;
    [Export]
    private Button btnSignup;

    [Export]
    private RichTextLabel richCondition;


    Action requestDisposer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        btnSignin.Disabled = MainClient.instance.state == 1 ? false : true;

        MainClient.instance.onConnect += () =>
        {
            btnSignin.Disabled = false;
        };
        MainClient.instance.onDisconnect += () =>
        {
            btnSignin.Disabled = true;
        };



        btnSignup.Pressed += () => {
            ControlExchange("SignupInterface", "res://controls/SignupInterface.tscn");
        };

        btnSignin.Pressed += () => {


            btnSignin.Disabled = true;
            MainClient.instance.Send(new Packet(Packet.Flag.ACCOUNT_SIGNIN, txtId.Text, txtPw.Text));

            requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ACCOUNT_SIGNIN_CALLBACK, packet =>
            {
                btnSignin.SetDeferred(Button.PropertyName.Disabled, false);

                //ACCOUNT_SIGNIN_CALLBACK
                bool isSucceed = (bool)packet.value[0];
                if (isSucceed)
                {
                    GD.Print("로그인 성공 : " + packet.value[1].ToString());
                    CallDeferred("ShowAcceptDialog", "로그인 성공", packet.value[1].ToString(), "확인");

                    //로그인 성공에 따른 처리
                    CallDeferred("ControlExchange", "RoomInterface", "res://controls/RoomInterface.tscn");
                    InroomInterface.userId = txtId.Text;
                }
                else
                {
                    GD.Print("로그인 실패 : " + packet.value[1].ToString());
                    CallDeferred("ShowAcceptDialog", "로그인 실패", packet.value[1].ToString(), "확인");
                }

                requestDisposer();
            });
        };

       
    }

   

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Node n = this;
        if (n is Control c)
            if (c.Visible == false) return;

        //if (MainClient.instance.state != 1) btnSignin.Disabled = false;

        string txtCondition = "";

        switch (MainClient.instance.state)
        {
            case 0:
                txtCondition += "[color=blue] 연결 중...";
                break;
            case 1:
                txtCondition += MainClient.instance.ping > 100 ?
                    "[color=orange] 연결 불량(핑 : " + MainClient.instance.ping + ")" :
                    "[color=green] 연결됨(핑 : " + MainClient.instance.ping + ")";
                break;
            case 2:
                txtCondition += "[color=red] 실패("+ Math.Round(MainClient.instance.conTryDelay * 10) / 10f + "초 뒤 재연결...)";
                break;
        }

        richCondition.Text = "서버 상태 :" + txtCondition;
    }

    void DebugAutoLogin(string id, Action tryNext)
    {
        MainClient.instance.Send(new Packet(Packet.Flag.ACCOUNT_SIGNIN, id, "testpw"));

        requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ACCOUNT_SIGNIN_CALLBACK, packet =>
        {
            //ACCOUNT_SIGNIN_CALLBACK
            bool isSucceed = (bool)packet.value[0];
            if (isSucceed)
            {
                CallDeferred("ControlExchange", "RoomInterface", "res://controls/RoomInterface.tscn");
                InroomInterface.userId = id;
            }
            else
            {
                Thread.Sleep(1000);
                tryNext();
            }

            requestDisposer();
        });

    }

}
