using UnityEngine;
using System.Collections;

public class GenericSoldierEnemy : BaseEnemy {

	[SerializeField] private Animation _unit_animation;
	[SerializeField] private GameObject _center_locator;
	
	public override void i_initialize(BattleGameEngine game) {
		base.i_initialize(game);
		
		_animation = new AnimationManager(_unit_animation);
		_animation.add_anim("Idle",0.5f);
		_animation.add_anim("Run",1.0f);
		_animation.play_anim("Idle");			
		this.move_to(this.transform.position);
	}
	
	public override void i_update(BattleGameEngine game) {
		base.i_update(game);
		if (_navagent.velocity.magnitude > 0.01f) {
			_animation.play_anim("Run");
		} else {
			_animation.play_anim("Idle");	
		}
		
	}
	
	public override float get_reticule_scale() { return 0.5f; }
	public override Vector3 get_center() { return _center_locator.transform.position; }

}
