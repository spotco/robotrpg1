using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyFloatingTargetingUI : MonoBehaviour {
		
	public bool _active;
	[SerializeField] public Animator _animator;
	
	[SerializeField] private float _max_scale, _min_scale, _min_dist, _max_dist;

	public EnemyFloatingTargetingUI i_initialize(float unit_target_scale) {
		_active = true;
		_max_scale *= unit_target_scale;
		_min_scale *= unit_target_scale;
		return this;
	}
	
	public void i_update(BaseEnemy itr_enemy, BattleGameEngine game) {
		this._active = true;
		float dist = Util.vec_dist(game._sceneref._player.transform.position,itr_enemy.get_center());
		dist = Mathf.Clamp(dist,_min_dist,_max_dist);
		float val = (_max_scale-_min_scale) * (1-(dist-_min_dist)/(_max_dist-_min_dist)) + _min_scale;
		this.transform.localScale = Util.valv(val);
	}
	
	public void fadeout() {
		_animator.SetBool("fadeout",true);
	}
	
	public bool should_remove() {
		return _animator.GetCurrentAnimatorStateInfo(0).IsName("End");
	}

}
