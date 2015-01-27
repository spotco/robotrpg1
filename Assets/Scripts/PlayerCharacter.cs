using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour {

	[SerializeField] public GameObject _follow_camera;
	[SerializeField] public GameObject _camera_anchor;
	[SerializeField] public GameObject _model_anchor;
	[SerializeField] public Rigidbody _body;
	[SerializeField] private GameObject _center;
	
	[SerializeField] private Animation _character_animation;
	public AnimationManager _animation;
	
	public void i_initialize(BattleGameEngine game) {
		_animation = new AnimationManager(_character_animation);
		_animation.add_anim("Idle",0.25f);
		_animation.add_anim("Walk",1.75f);
		_animation.add_anim("Walk_Left",1.75f);
		_animation.add_anim("Walk_Right",1.75f);
		_animation.play_anim("Idle");
	}
	
	public Vector3 get_forward() {
		return Util.vec_sub(_center.transform.position,_follow_camera.transform.position).normalized;
	}
	
	public bool point_in_fov(Vector3 pos) {
		return Vector3.Angle(get_forward(),Util.vec_sub(pos,_center.transform.position)) < 10;
	}
	
	public void freeze() {
		_animation.pause_anims();
		_body.isKinematic = true;
	}
	
	public void unfreeze() {
		_animation.unpause_anims();
		_body.isKinematic = false;
	}
	
	public void i_update(BattleGameEngine game_engine) {
		bool moving = move();
		fps_turn(moving);
	}
	
	
	private static float MOVE_SPEED = 7f;
	private bool move() {
		Vector3 neu_vel = _body.velocity;
		
		Vector3 forward = _camera_anchor.transform.forward;
		forward.y = 0;
		forward.Normalize();
		
		bool move_ws = false;
		Vector3 ws_v = forward;
		ws_v = Util.vec_scale(ws_v,MOVE_SPEED);
		if (Input.GetKey(KeyCode.W)) {
			move_ws = true;
			_animation.play_anim("Walk");
			
		} else if (Input.GetKey(KeyCode.S)) {
			move_ws = true;
			ws_v = Util.vec_scale(ws_v,-1);
			_animation.play_anim("Walk",-1);
		}
		
		bool move_ad = false;
		Vector3 ad_v = Util.vec_cross(forward,new Vector3(0,1,0));
		ad_v.Normalize();
		ad_v = Util.vec_scale(ad_v,MOVE_SPEED);
		
		if (Input.GetKey(KeyCode.A)) {
			move_ad = true;
			_animation.play_anim("Walk_Left");
			
		} else if (Input.GetKey(KeyCode.D)) {
			move_ad = true;
			ad_v = Util.vec_scale(ad_v,-1);
			_animation.play_anim("Walk_Right");
		}
		
		if (move_ws && move_ad) {
			neu_vel = Util.vec_add(ws_v,ad_v);
		} else if (move_ws) {
			neu_vel = ws_v;
		} else if (move_ad) {
			neu_vel = ad_v;
		} else {
			_animation.play_anim("Idle");
		}
		
		neu_vel.x *= 0.95f;
		neu_vel.z *= 0.95f;
		if (Math.Abs(neu_vel.x) < 0.2f) neu_vel.x = 0;
		if (Math.Abs(neu_vel.z) < 0.2f) neu_vel.z = 0;
		_body.velocity = neu_vel;
		
		return move_ad || move_ws;
	}
	
	private static float MAX_X_ANGLE = 25;
	private static float FPS_LOOK_SCALE = 5.0f;
	private Vector3 _xy_angle = Vector3.zero;
	private void fps_turn(bool moving) {
		float mousex_delta = Input.GetAxis("Mouse X");
		float mousey_delta = Input.GetAxis("Mouse Y");
		_xy_angle.y += mousex_delta * FPS_LOOK_SCALE;
		_xy_angle.x -= mousey_delta * FPS_LOOK_SCALE;
		
		if (Math.Abs(_xy_angle.x) > MAX_X_ANGLE) {
			_xy_angle.x = Util.sig(_xy_angle.x) * MAX_X_ANGLE;
		}
		_camera_anchor.transform.rotation = Quaternion.Euler(_xy_angle);
		Vector3 xy_angle_nox = new Vector3(0,_xy_angle.y,_xy_angle.z);
		_model_anchor.transform.rotation = Quaternion.Euler(xy_angle_nox);
		
		if (!moving && (Mathf.Abs(mousex_delta) > 0.1f || Mathf.Abs(mousey_delta) > 0.1f)) {
			_animation.play_anim("Walk",0.5f);
		}
	}
	
}
