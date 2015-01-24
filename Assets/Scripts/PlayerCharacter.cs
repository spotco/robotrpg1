using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour {

	[SerializeField] public GameObject _follow_camera;
	[SerializeField] public GameObject _camera_anchor;
	[SerializeField] public GameObject _model_anchor;
	[SerializeField] public Rigidbody _body;
	[SerializeField] public Animation _animation;
	
	public void i_initialize() {
		_animation.playAutomatically = true;
		_animation.wrapMode = WrapMode.Loop;
		
		_animation["Idle"].speed = 0.25f;
		_animation["Walk_Left"].speed = 1.75f;
		_animation["Walk_Right"].speed = 1.75f;
	}
	
	public void i_update(BattleGameEngine game_engine) {
		move();
		fps_turn();
	}
	
	
	private static float MOVE_SPEED = 7f;
	private void move() {
		Vector3 neu_vel = _body.velocity;
		
		Vector3 forward = _camera_anchor.transform.forward;
		forward.y = 0;
		forward.Normalize();
		
		bool move_ws = false;
		Vector3 ws_v = forward;
		ws_v = Util.vec_scale(ws_v,MOVE_SPEED);
		if (Input.GetKey(KeyCode.W)) {
			move_ws = true;
			_animation["Walk"].speed = 1.75f;
			_animation.CrossFade("Walk");
			
		} else if (Input.GetKey(KeyCode.S)) {
			move_ws = true;
			ws_v = Util.vec_scale(ws_v,-1);
			_animation["Walk"].speed = -1.75f;
			_animation.CrossFade("Walk");
		}
		
		bool move_ad = false;
		Vector3 ad_v = Util.vec_cross(forward,new Vector3(0,1,0));
		ad_v.Normalize();
		ad_v = Util.vec_scale(ad_v,MOVE_SPEED);
		
		if (Input.GetKey(KeyCode.A)) {
			move_ad = true;
			_animation.CrossFade("Walk_Left");
			
		} else if (Input.GetKey(KeyCode.D)) {
			move_ad = true;
			ad_v = Util.vec_scale(ad_v,-1);
			_animation.CrossFade("Walk_Right");
		}
		
		if (move_ws && move_ad) {
			neu_vel = Util.vec_add(ws_v,ad_v);
		} else if (move_ws) {
			neu_vel = ws_v;
		} else if (move_ad) {
			neu_vel = ad_v;
		} else {
			_animation.CrossFade("Idle");
		}
		
		neu_vel.x *= 0.95f;
		neu_vel.z *= 0.95f;
		if (Math.Abs(neu_vel.x) < 0.2f) neu_vel.x = 0;
		if (Math.Abs(neu_vel.z) < 0.2f) neu_vel.z = 0;
		_body.velocity = neu_vel;
	}
	
	private static float MAX_X_ANGLE = 15;
	private static float FPS_LOOK_SCALE = 3.0f;
	private Vector3 _xy_angle = Vector3.zero;
	private void fps_turn() {
		_xy_angle.y += Input.GetAxis("Mouse X") * FPS_LOOK_SCALE;
		_xy_angle.x -= Input.GetAxis("Mouse Y") * FPS_LOOK_SCALE;
		
		if (Math.Abs(_xy_angle.x) > MAX_X_ANGLE) {
			_xy_angle.x = Util.sig(_xy_angle.x) * MAX_X_ANGLE;
		}
		
		_camera_anchor.transform.rotation = Quaternion.Euler(_xy_angle);
		
		Vector3 xy_angle_nox = new Vector3(0,_xy_angle.y,_xy_angle.z);
		
		_model_anchor.transform.rotation = Quaternion.Euler(xy_angle_nox);
	}
	
}
