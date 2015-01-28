using UnityEngine;
using System.Collections;

public class BulletParticle : BaseParticle {

	public static string BULLET_MECH = "Particles/particle_bullet_mech";
	
	public override void i_initialize(BattleGameEngine game) {}

	private Vector3 _start_pos, _end_pos;
	private Vector3 _collision_normal;
	private float _time, _time_max;
	public BulletParticle set_start_end_positions(Vector3 start, Vector3 end, float speed) {
		_start_pos = start;
		_end_pos = end;
		_time = 0;
		_time_max = Util.vec_dist(_start_pos,_end_pos)/speed;
		this.set_position(_start_pos);
		this.transform.LookAt(end);
		return this;
	}
	public BulletParticle set_collision_normal(Vector3 n) {
		_collision_normal = n;
		return this;
	}

	public override void i_update(BattleGameEngine game) {
		this.set_position(Vector3.Lerp(_start_pos,_end_pos,_time/_time_max));
		_time += Time.deltaTime;
	}
	public override bool should_remove(BattleGameEngine game) { 
		return _time >= _time_max;
	}
	private bool _do_hit_effect = false;
	public BulletParticle set_do_bullet_hit_effect(bool val) {
		_do_hit_effect = val;
		return this;
	}
	public override void do_remove(BattleGameEngine game) {
		if (_do_hit_effect) {
			game.add_particle(ParticleSystemWrapperParticle.BULLET_IMPACT).set_position(
				_end_pos
			);
			((BulletHoleParticle)game.add_particle(BulletHoleParticle.BULLET_HOLE)).set_position_and_lookat(
				Vector3.Lerp(_start_pos,_end_pos,(_time_max-0.0005f)/_time_max),
				Util.vec_add(_end_pos,_collision_normal)
			);
		}
	}
	public override void freeze() {}
	public override void unfreeze() {}
}
