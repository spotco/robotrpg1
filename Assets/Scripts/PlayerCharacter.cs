using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour {

	[SerializeField] public GameObject _follow_camera;
	[SerializeField] public Rigidbody _body;
	[SerializeField] public Animation _animation;
	
	public void i_initialize() {
		_animation.playAutomatically = true;
		_animation.wrapMode = WrapMode.Loop;
	}
	
	public void i_update() {
		move();
	}
	
	
	private static float MOVE_SPEED = 7f;
	private void move() {
		Vector3 neu_vel = _body.velocity;
		
		Vector3 forward = gameObject.transform.forward;
		forward.y = 0;
		forward.Normalize();
		
		bool move_ws = false;
		Vector3 ws_v = forward;
		ws_v = Util.vec_scale(ws_v,MOVE_SPEED);
		if (Input.GetKey(KeyCode.W)) {
			move_ws = true;
			_animation["Walk"].speed = 1;
			_animation.CrossFade("Walk");
			
		} else if (Input.GetKey(KeyCode.S)) {
			move_ws = true;
			ws_v = Util.vec_scale(ws_v,-1);
			_animation["Walk"].speed = -1;
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
	
}
