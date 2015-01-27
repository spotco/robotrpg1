using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {

	public virtual void i_initialize(BattleGameEngine game) {
		_navagent = this.GetComponent<NavMeshAgent>();
		_current_health = get_max_health();
	}
	
	public virtual void i_update(BattleGameEngine game) {
	}
	
	public virtual float get_reticule_scale() { return 1.0f; }
	public virtual Vector3 get_center() { return Vector3.zero; }

	public float _current_health = 0;
	public virtual float get_max_health() {
		return 1.0f;
	}
	public virtual string get_name() {
		return "BaseEnemy";
	}

	protected NavMeshAgent _navagent;
	public void move_to(Vector3 pos) {
		_navagent.SetDestination(pos);
	}
	
	public AnimationManager _animation;
	public void freeze() {
		_navagent.updatePosition = false;
		_navagent.updateRotation = false;
		_animation.pause_anims();
	}
	
	public void unfreeze() {
		_navagent.updatePosition = true;
		_navagent.updateRotation = true;
		_animation.unpause_anims();
	}

}
