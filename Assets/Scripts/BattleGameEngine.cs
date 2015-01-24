using UnityEngine;
using System.Collections;

public class BattleGameEngine : MonoBehaviour {
	
	public enum BattleGameEngineMode {
		PlayerControl,
		TacticalMode,
		CameraTransition
	}
	
	private SceneRef _sceneref;
	private BattleGameEngineMode _current_mode;

	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;
		
		_sceneref._topdown_camera.SetActive(false);
		_sceneref._transition_camera.SetActive(false);
		_sceneref._player._follow_camera.SetActive(true);
		
		_sceneref._player.i_initialize();
		
		_current_mode = BattleGameEngineMode.PlayerControl;
	}
	
	public void Update() {
		if (_current_mode == BattleGameEngineMode.PlayerControl) {
			_sceneref._player.i_update(this);
			if (Input.GetKeyUp(KeyCode.Tab)) {
				GameObject to_camera = _sceneref._topdown_camera;
				GameObject from_camera = _sceneref._player._follow_camera;
				Vector3 to_camera_position = to_camera.transform.position;
				to_camera_position.x = from_camera.transform.position.x + _sceneref._player._camera_anchor.transform.forward.x * 5;
				to_camera_position.z = from_camera.transform.position.z + _sceneref._player._camera_anchor.transform.forward.z * 5;
				to_camera.transform.position = to_camera_position;
               	transition_to_mode(BattleGameEngineMode.TacticalMode,from_camera,to_camera);
            }
			
		} else if (_current_mode == BattleGameEngineMode.CameraTransition) {
			Vector3 transition_camera_pos = _sceneref._transition_camera.transform.position;
			transition_camera_pos = Util.vec_drp(transition_camera_pos,_camera_transition_to_pos,0.1f);
			_sceneref._transition_camera.transform.position = transition_camera_pos;
			
			float pct = Util.vec_dist(transition_camera_pos,_camera_transition_from_pos)/Util.vec_dist(_camera_transition_from_pos,_camera_transition_to_pos);
			_sceneref._transition_camera.transform.rotation = Quaternion.Lerp(_camera_transition_from_rot,_camera_transition_to_rot,pct);
			
			if (Util.vec_dist(transition_camera_pos,_camera_transition_to_pos) < 0.05f) {
				_current_mode = _target_camera_transition_mode;
				_target_transition_camera.SetActive(true);
				_sceneref._transition_camera.SetActive(false);
			}
		
		
		} else if (_current_mode == BattleGameEngineMode.TacticalMode) {
			if (Input.GetKeyUp(KeyCode.Tab)) {
                transition_to_mode(BattleGameEngineMode.PlayerControl,_sceneref._topdown_camera,_sceneref._player._follow_camera);
            }
            if (Input.GetKey(KeyCode.W)) {
            
            } else if (Input.GetKey(KeyCode.S)) {
            
            }
            if (Input.GetKey(KeyCode.A)) {
            
            } else if (Input.GetKey(KeyCode.D)) {
            
            }
		}
	}
	
	private BattleGameEngineMode _target_camera_transition_mode = BattleGameEngineMode.PlayerControl;
	private GameObject _target_transition_camera = null;
	private Vector3 _camera_transition_to_pos = Vector3.zero, _camera_transition_from_pos = Vector3.zero;
	private Quaternion _camera_transition_to_rot = Quaternion.identity, _camera_transition_from_rot = Quaternion.identity;
	private void transition_to_mode(BattleGameEngineMode target_mode, GameObject from_camera, GameObject to_camera) {
		_current_mode = BattleGameEngineMode.CameraTransition;
		_target_camera_transition_mode = target_mode;
		
		Vector3 to_camera_rotation = to_camera.transform.rotation.eulerAngles;
		to_camera_rotation.y = from_camera.transform.rotation.eulerAngles.y;
		Util.transform_set_euler_world(to_camera.transform,to_camera_rotation);
		
		_target_transition_camera = to_camera;
		
		_sceneref._transition_camera.transform.position = from_camera.transform.position;
		Util.transform_set_euler_world(_sceneref._transition_camera.transform,from_camera.transform.rotation.eulerAngles);
		
		_camera_transition_from_pos = from_camera.transform.position;
		_camera_transition_from_rot = from_camera.transform.rotation;
		_camera_transition_to_pos = to_camera.transform.position;
		_camera_transition_to_rot = to_camera.transform.rotation;
		
		_sceneref._player._follow_camera.SetActive(false);
		_sceneref._topdown_camera.SetActive(false);
		_sceneref._transition_camera.SetActive(true);
	}
}
