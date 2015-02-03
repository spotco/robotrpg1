using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemy : MonoBehaviour {

	public virtual void i_initialize(BattleGameEngine game) {
		_navagent = this.GetComponent<NavMeshAgent>();
		_current_health = this.get_max_health();
		_alive = true;
	}

	[SerializeField] private ColliderPointer _collider_pointer;
	public Collider get_collider() {
		return _collider_pointer.get_collider();
	}

	public virtual bool should_remove(BattleGameEngine game) { return true; }
	public virtual void do_remove(BattleGameEngine game) { }

	private int _hits_taken_count = 0;
	private int _total_damage_taken_count = 0;
	private int _crits_count = 0;
	private int _damage_disp_coalesce_ct = 0;
	public Vector3 _last_damage_dir = Vector3.up;
	public virtual void take_damage(BattleGameEngine game, int damage, Vector3 direction, bool crit = false) {
		if (_alive) {
			_hits_taken_count++;
			_total_damage_taken_count += damage;
			if (crit) _crits_count++;
			_current_health -= damage;
			_last_damage_dir = direction;

			if (_current_health <= 0) {
				_alive = false;
				this.on_death(game);
			}
		}
	}

	[NonSerialized] public bool _alive = true;
	public virtual void on_death(BattleGameEngine game) {}

	public virtual void i_update(BattleGameEngine game) {
		_damage_disp_coalesce_ct++;
		if (_damage_disp_coalesce_ct >= 10) {
			_damage_disp_coalesce_ct = 0;
			if (_hits_taken_count > 0) {

				Vector3 text_pos = Util.vec_delta(this.get_center(),Util.rand_range(-0.25f,0.25f),Util.rand_range(-0.25f,0.25f)+0.75f,Util.rand_range(-0.25f,0.25f));
				FlyUpDamageTextUIParticle.Mode text_mode = FlyUpDamageTextUIParticle.Mode.EnemyDamage;
				string val = string.Format("{0}",_total_damage_taken_count);
				if (_total_damage_taken_count == 0) {
					text_mode = FlyUpDamageTextUIParticle.Mode.Miss;
					val = "Miss";
				} else if (_crits_count > 0) {
					text_mode = FlyUpDamageTextUIParticle.Mode.EnemyCrit;
				}

				((FlyUpDamageTextUIParticle)game._sceneref._ui.add_particle(FlyUpDamageTextUIParticle.DAMAGE_TEXT)).start(
					Camera.main.WorldToScreenPoint(text_pos),
					val,
					text_mode
				);


				_hits_taken_count = 0;
				_total_damage_taken_count = 0;
				_crits_count = 0;

			}
		}
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
	public virtual void freeze() {
		_navagent.updatePosition = false;
		_navagent.updateRotation = false;
		_animation.pause_anims();
	}
	
	public virtual void unfreeze() {
		_navagent.updatePosition = true;
		_navagent.updateRotation = true;
		_animation.unpause_anims();
	}

}
