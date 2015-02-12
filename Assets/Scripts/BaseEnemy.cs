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
	public virtual void on_death(BattleGameEngine game) {
		game._sceneref._path_renderer.clear_path(this.GetInstanceID());
	}

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

	private bool _path_dirty = false;
	private Vector3 _last_vec_mid = Vector3.zero;
	public void frozen_update(BattleGameEngine game) {
		if (_navagent.path.status != NavMeshPathStatus.PathInvalid) {
			Vector3 tmp_vec_mid = _navagent.path.corners[_navagent.path.corners.Length/2];
			if (!_navagent.pathPending && _navagent.hasPath && (_path_dirty || tmp_vec_mid != _last_vec_mid)) {
				update_path_render(game);
				_last_vec_mid = tmp_vec_mid;
			}
		}
	}

	private void update_path_render(BattleGameEngine game) {
		if (_navagent.path.status != NavMeshPathStatus.PathInvalid) {
			_path_dirty = false;
			game._sceneref._path_renderer.clear_path(this.GetInstanceID());
			game._sceneref._path_renderer.id_draw_path(this.GetInstanceID(),transform.position,_navagent.path.corners);
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
	public void move_to(BattleGameEngine game, Vector3 pos) {
		if (_navagent.enabled) {
			bool tmp_frozen = _frozen;
			if (tmp_frozen) this.unfreeze(game);
			_navagent.SetDestination(pos);
			_path_dirty = true;
			if (tmp_frozen) this.freeze(game);
		}
	}
	
	public AnimationManager _animation;
	private bool _frozen = false;
	public virtual void freeze(BattleGameEngine game) {
		_frozen = true;
		if (_navagent.enabled) _navagent.Stop();
		_animation.pause_anims();
		update_path_render(game);
	}
	
	public virtual void unfreeze(BattleGameEngine game) {
		_frozen = false;
		if (_navagent.enabled) _navagent.Resume();
		_animation.unpause_anims();
		game._sceneref._path_renderer.clear_path(this.GetInstanceID());
	}

}
