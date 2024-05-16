using Godot;
using System;

public partial class Module : StaticBody2D
{
	public Ship master;
	public Type type = Type.Hull;
	public bool isActivated => health.inner.value > 0f && circuit.burned == false;

	public (
		(float level, float value, float max) armour,
		(float level, float value, float max) inner, bool necessary) health = ((3, 100, 100), (1, 100, 100), false);

	public float heat = -10f;
	public (float regen, float value, float max, bool burned) circuit = (4f, 100f, 100f, false);


	public override void _Ready(){}
    public override void _EnterTree()
    {
		master = GetParent() as Ship;
        base._EnterTree();
    }
    public override void _Process(double delta)
	{
		HeatProcess(delta);
		CircuitProcess(delta);
	}


	void HeatProcess(double delta){}
	void CircuitProcess(double delta){}

	public void GetDamage(Projectile.Damage damage)
	{
		switch(damage.type)
		{
			case Projectile.Damage.Type.Mass : {
				float peneLevel;

				//armour
				if(health.armour.value > 0f) 
				{
					peneLevel = damage.pene - health.armour.level;

					if(peneLevel > 2f){
						//100%, 25%
						health.armour.value -= damage.value;
						health.inner.value -= damage.value * 0.25f;	//overpene!
						master.health.value -= damage.value * 0.25f;
					}
					else if(peneLevel > -2f){
						//75 ~ 25%, 18.75 ~ 6.25%
						health.armour.value -= damage.value  * (peneLevel + 4f) / 8f; 
						health.inner.value -= damage.value  * (peneLevel + 4f) / 8f / 4f;	//overpene!
						master.health.value -= damage.value  * (peneLevel + 4f) / 8f / 4f;	//overpene!
					}
					else
						//0%
						health.armour.value = 0f;
					break;
				}

				//inner
				peneLevel = damage.pene - health.inner.level;

				if(peneLevel > 2f){
					//100%
					health.inner.value -= damage.value;
					master.health.value -= damage.value;
				}
				else if(peneLevel > -2f){
					//75 ~ 25%
					health.inner.value -= damage.value  * (peneLevel + 4f) / 8f; 
					master.health.value -= damage.value  * (peneLevel + 4f) / 8f
				}
				else
					//0%
					health.inner.value = 0f;

			}break;
		}
	}


	public enum Type 
	{
		Hull,
		Thruster,
		CIWS,
	}
}
