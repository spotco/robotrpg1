using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class BulletHoleParticle : BaseParticle {
	public static string BULLET_HOLE = "Particles/particle_bullet_hole";
	
	public override void i_initialize(BattleGameEngine game) {
		_img = _imgobj.GetComponent<SpriteRenderer>();
		_ct = CT_MAX;
		Util.transform_set_euler_world(_imgobj.transform,new Vector3(0,0,Util.rand_range(-180,180)));
	}
	private static float CT_MAX = 200;
	private float _ct = 0;
	private SpriteRenderer _img;
	[SerializeField] private GameObject _imgobj;                              
	public void set_position_and_lookat(Vector3 pos, Vector3 lookat) {
		this.transform.position = pos;
		this.transform.LookAt(lookat);
	}

	public override void i_update(BattleGameEngine game) {
		_ct--;
		Color color = _img.color;
		color.a = _ct/CT_MAX * 0.35f;
		_img.color = color;
	}
	public override bool should_remove(BattleGameEngine game) { 
		return _ct <= 0; 
	}
	public override void do_remove(BattleGameEngine game) {}
	
	public override void freeze() {
	}
	public override void unfreeze() {
	}
}
