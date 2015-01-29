using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GenericSoldierEnemy : BaseEnemy {

	[SerializeField] private Animation _unit_animation;
	[SerializeField] private GameObject _center_locator;
	[SerializeField] private SkinnedMeshRenderer _mesh;
	
	public override void i_initialize(BattleGameEngine game) {
		base.i_initialize(game);
		
		_animation = new AnimationManager(_unit_animation);
		_animation.add_anim("Idle",0.5f);
		_animation.add_anim("Run",1.0f);
		_animation.add_anim("Death_2",1.0f);
		_animation.play_anim("Idle");			
		this.move_to(this.transform.position);
	}

	public override void on_death(BattleGameEngine game) {
		_animation.play_anim("Death_2",1,false);

		_mesh.material.shader = ShaderManager.inst.RGBA_Transparent;

		_navagent.enabled = false;
		rigidbody.isKinematic = false;
		rigidbody.AddForce(_last_damage_dir * 10,ForceMode.Impulse);
	}

	private float _fadeout_ct = 1.0f;
	public override void i_update(BattleGameEngine game) {
		base.i_update(game);
		if (this._alive) {
			if (_navagent.velocity.magnitude > 0.01f) {
				_animation.play_anim("Run");
			} else {
				_animation.play_anim("Idle");	
			}
		} else {
			_mesh.material.color = new Color(1.0f,1.0f,1.0f,_fadeout_ct);
			_fadeout_ct = Mathf.Clamp(_fadeout_ct-0.01f,0,1.0f);
		}
	}

	public override bool should_remove(BattleGameEngine game) {
		return !this._alive && _fadeout_ct <= 0;
	}

	public override float get_max_health() {
		return 50.0f;
	}

	public override string get_name() {
		return "Soldier";
	}
	
	public override float get_reticule_scale() { return 0.5f; }
	public override Vector3 get_center() { return _center_locator.transform.position; }

	public override void freeze() {
		rigidbody.Sleep();
		base.freeze();
	}
	public override void unfreeze() {
		rigidbody.WakeUp();
		base.unfreeze();
	}

}
