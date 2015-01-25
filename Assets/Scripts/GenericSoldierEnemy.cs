using UnityEngine;
using System.Collections;

public class GenericSoldierEnemy : BaseEnemy {

	[SerializeField] private Animation _animation;
	
	public override void i_initialize(BattleGameEngine game) {
		_animation.playAutomatically = true;
		_animation.wrapMode = WrapMode.Loop;
		_animation.Play("Run");
	}
	
	public override void i_update(BattleGameEngine game) {
		
	}

}
