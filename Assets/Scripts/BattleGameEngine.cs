using UnityEngine;
using System.Collections.Generic;
using System;

public class BattleGameEngine : MonoBehaviour {
	
	public enum BattleGameEngineMode {
		PlayerControl,
		TacticalMode,
		CameraTransition
	}
	
	[NonSerialized] public SceneRef _sceneref;
	[NonSerialized] public BattleGameEngineMode _current_mode;
	[NonSerialized] public List<BaseParticle> _particles = new List<BaseParticle>();

	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;
		
		_sceneref._topdown_camera.SetActive(false);
		_sceneref._transition_camera.SetActive(false);
		_sceneref._player._follow_camera.SetActive(true);
		
		_sceneref._player.i_initialize(this);
		_sceneref._ui.i_initialize(this);
		
		for (int i_enemy = 0; i_enemy < _sceneref._enemies.Count; i_enemy++) {
			BaseEnemy itr_enemy = _sceneref._enemies[i_enemy];
			itr_enemy.i_initialize(this);
		}
		
		_current_mode = BattleGameEngineMode.PlayerControl;
	}

	private static float TACTICAL_MOVE_SCALE = 0.5f;
	public void Update() {
		_sceneref._ui.i_update(this);
		if (_current_mode == BattleGameEngineMode.PlayerControl) {
			Screen.showCursor = false;
			Screen.lockCursor = true;

			_sceneref._player.i_update(this);
			for (int i_enemy = 0; i_enemy < _sceneref._enemies.Count; i_enemy++) {
				BaseEnemy itr_enemy = _sceneref._enemies[i_enemy];
				itr_enemy.i_update(this);
			}
			update_particles();
			
			if (Input.GetKeyUp(KeyCode.Tab)) {
				GameObject to_camera = _sceneref._topdown_camera;
				GameObject from_camera = _sceneref._player._follow_camera;
				Vector3 to_camera_position = to_camera.transform.position;
				to_camera_position.x = from_camera.transform.position.x + _sceneref._player._camera_anchor.transform.forward.x * 5;
				to_camera_position.z = from_camera.transform.position.z + _sceneref._player._camera_anchor.transform.forward.z * 5;
				to_camera.transform.position = to_camera_position;
               	transition_to_mode(BattleGameEngineMode.TacticalMode,from_camera,to_camera);
            }
            
			if (Input.GetMouseButton(0)) {
				RaycastHit[] hits = Physics.RaycastAll(_sceneref._player.get_center(),_sceneref._player.get_forward());
				float hit_min_dist = float.PositiveInfinity;
				RaycastHit closest_hit = new RaycastHit();
				bool hit_found = false;
				foreach(RaycastHit hit in hits) {
					if (hit.collider.gameObject == _sceneref._player.gameObject) continue;
					float dist = Util.vec_dist(_sceneref._player.get_center(),hit.point);
					if (dist < hit_min_dist) {
						closest_hit = hit;
						hit_min_dist = dist;
						hit_found = true;
					}
				}

				float hit_dist;
				float max_variance_angle = 5.0f;
				Vector3 fire_pos = _sceneref._player.get_bullet_pos();
				Vector3 target_location;
				if (hit_found) {
					hit_dist = Util.vec_dist(closest_hit.point,fire_pos);
					target_location = closest_hit.point;
				} else {
					hit_dist = 300;
					target_location = Util.vec_add(_sceneref._player.get_bullet_pos(),
						Util.vec_scale(_sceneref._player.get_forward(),hit_dist)
					);
				}
				float max_variance = hit_dist * Mathf.Tan(Util.deg2rad*max_variance_angle);

				Vector3 target_dir = Util.vec_sub(target_location,fire_pos);
				Vector3 px = Util.vec_any_normal(target_dir).normalized * max_variance * Util.rand_range(-1,1);
				Vector3 py = Util.vec_cross(target_dir,px).normalized * max_variance * Util.rand_range(-1,1);

				Vector3 target_position = new Vector3(target_location.x+px.x+py.x,target_location.y+px.y+py.y,target_location.z+px.z+py.z);
				RaycastHit bullet_hit;
				bool bullet_hit_found = Physics.Raycast(
					_sceneref._player.get_bullet_pos(), 
					Util.vec_sub(target_position,fire_pos),
					out bullet_hit
				);
				((BulletParticle)this.add_particle(BulletParticle.BULLET_MECH)).set_start_end_positions(
					_sceneref._player.get_bullet_pos(),
					bullet_hit_found?bullet_hit.point:target_position,
					50
				).set_do_bullet_hit_effect(
					bullet_hit_found
				).set_collision_normal(
					bullet_hit_found?bullet_hit.normal:Vector3.up
				);


				if (bullet_hit_found) {
					Collider[] splash_hits = Physics.OverlapSphere(bullet_hit.point,0.75f);
					foreach(Collider splash_hit in splash_hits) {
						BaseEnemy itr_enemy = splash_hit.gameObject.GetComponent<BaseEnemy>();
						if (itr_enemy != null) {
							float rnd = Util.rand_range(0,100);
							if (rnd < 5) {
								itr_enemy.take_damage(this,25,true);
							} else if (rnd < 45) {
								itr_enemy.take_damage(this,0);
							} else {
								itr_enemy.take_damage(this,5);
							}
						}
					}
				}
			}
			
		} else if (_current_mode == BattleGameEngineMode.CameraTransition) {
			Screen.showCursor = false;
			Screen.lockCursor = true;

			_sceneref._transition_camera.transform.position = Util.sin_lerp_vec(
				_transition_from_camera.transform.position,
				_transition_to_camera.transform.position,
				_camera_transition_lerp_t
			);
			
			_sceneref._transition_camera.transform.rotation = Quaternion.Lerp(
				_transition_from_camera.transform.rotation,
				_transition_to_camera.transform.rotation,
				Util.sin_lerp(0.0f,1.0f,_camera_transition_lerp_t)
			);
			
			_camera_transition_lerp_t += 0.025f;
			if (_camera_transition_lerp_t >= 1) {
				_current_mode = _target_camera_transition_mode;
				_transition_to_camera.SetActive(true);
				_sceneref._transition_camera.SetActive(false);
				if (_current_mode == BattleGameEngineMode.PlayerControl) {
					freeze_game(false);
				}
			}
		
		
		} else if (_current_mode == BattleGameEngineMode.TacticalMode) {
			Screen.showCursor = true;
			Screen.lockCursor = false;

			if (Input.GetKeyUp(KeyCode.Tab)) {
                transition_to_mode(BattleGameEngineMode.PlayerControl,_sceneref._topdown_camera,_sceneref._player._follow_camera);
            }
            
			Vector3 forward = new Vector3(_sceneref._player._camera_anchor.transform.forward.x,0,_sceneref._player._camera_anchor.transform.forward.z);
			Vector3 up = new Vector3(0,1,0);
			Vector3 right = Util.vec_cross(forward,up);
			float forward_mv_scale = 0.0f;
			float right_mv_scale = 0.0f;
            
            if (Input.GetKey(KeyCode.W)) {
            	forward_mv_scale = TACTICAL_MOVE_SCALE;
            	
            } else if (Input.GetKey(KeyCode.S)) {
            	forward_mv_scale = -TACTICAL_MOVE_SCALE;
            }
            if (Input.GetKey(KeyCode.A)) {
            	right_mv_scale = TACTICAL_MOVE_SCALE;
            } else if (Input.GetKey(KeyCode.D)) {
            	right_mv_scale = -TACTICAL_MOVE_SCALE;
            }
            forward.Scale(Util.valv(forward_mv_scale));
            right.Scale(Util.valv(right_mv_scale));
            Util.transform_position_delta(_sceneref._topdown_camera.transform,forward);
            Util.transform_position_delta(_sceneref._topdown_camera.transform,right);
            
            if (Input.GetMouseButtonUp(0)) {
				RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
				for(int i_hit = 0; i_hit < hits.Length; i_hit++) {
					RaycastHit itr_hit = hits[i_hit];
					if (itr_hit.collider.gameObject.GetComponent<BaseEnemy>() != null) {						
					} else if (itr_hit.collider.gameObject.GetComponent<PlayerCharacter>() != null) {
					} else if (itr_hit.collider.gameObject.GetComponent<WorldTerrain>() != null) {
						for (int i_enemy = 0; i_enemy < _sceneref._enemies.Count; i_enemy++) {
							BaseEnemy itr_enemy = _sceneref._enemies[i_enemy];
							itr_enemy.move_to(new Vector3(itr_hit.point.x+Util.rand_range(-1.5f,1.5f),itr_hit.point.y,itr_hit.point.z+Util.rand_range(-1.5f,1.5f)));	
						}
					}
				}
            }            
		}
	}

	public BaseParticle add_particle(string name) {

		BaseParticle particle = ((GameObject)Instantiate(Resources.Load(name))).GetComponent<BaseParticle>();
		particle.i_initialize(this);
		particle.gameObject.transform.parent = _sceneref._particle_root.transform;
		_particles.Add(particle);
		return particle;
	}

	private void update_particles() {
		for (int i_particle = _particles.Count-1; i_particle >= 0; i_particle--) {
			BaseParticle itr_particle = _particles[i_particle];
			itr_particle.i_update(this);
			if (itr_particle.should_remove(this)) {
				itr_particle.do_remove(this);
				itr_particle.gameObject.transform.parent = null;
				Destroy(itr_particle.gameObject);
				_particles.RemoveAt(i_particle);
			}
		}
	}
	
	private void freeze_game(bool val) {
		for (int i_enemy = 0; i_enemy < _sceneref._enemies.Count; i_enemy++) {
			BaseEnemy itr_enemy = _sceneref._enemies[i_enemy];
			if (val) {
				itr_enemy.freeze();
			} else {
				itr_enemy.unfreeze();
			}	
		}
		for (int i_particle = _particles.Count-1; i_particle >= 0; i_particle--) {
			BaseParticle itr_particle = _particles[i_particle];
			if (val) {
				itr_particle.freeze();
			} else {
				itr_particle.unfreeze();
			}
		}
		if (val) {
			_sceneref._player.freeze();
		} else {
			_sceneref._player.unfreeze();
		}
	}
	
	private BattleGameEngineMode _target_camera_transition_mode = BattleGameEngineMode.PlayerControl;
	private GameObject _transition_to_camera = null, _transition_from_camera = null;
	private float _camera_transition_lerp_t = 0;
	
	private void transition_to_mode(BattleGameEngineMode target_mode, GameObject from_camera, GameObject to_camera) {
		_current_mode = BattleGameEngineMode.CameraTransition;
		_target_camera_transition_mode = target_mode;
		_camera_transition_lerp_t = 0;
		Vector3 to_camera_rotation = to_camera.transform.rotation.eulerAngles;
		to_camera_rotation.y = from_camera.transform.rotation.eulerAngles.y;
		Util.transform_set_euler_world(to_camera.transform,to_camera_rotation);
		
		_transition_from_camera = from_camera;
		_transition_to_camera = to_camera;
		
		_sceneref._transition_camera.transform.position = from_camera.transform.position;
		Util.transform_set_euler_world(_sceneref._transition_camera.transform,from_camera.transform.rotation.eulerAngles);
		
		_sceneref._player._follow_camera.SetActive(false);
		_sceneref._topdown_camera.SetActive(false);
		_sceneref._transition_camera.SetActive(true);
		freeze_game(true);
	}
}
