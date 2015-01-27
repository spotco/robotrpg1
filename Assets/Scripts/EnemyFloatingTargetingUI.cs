using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class EnemyFloatingTargetingUI : MonoBehaviour {
		
	private enum EnemyFloatingTargetingUIMode {
		FadeIn,
		Idle,
		FadeOut
	};
	private EnemyFloatingTargetingUIMode _current_mode;

	[SerializeField] private RectTransform _reticule_transform;
	[SerializeField] private Image _reticule_image;
	[SerializeField] private GameObject _infodisp_root;

	private float _reticule_anim_t = 0; //1 out, 0 in
	private float _retic_target_alpha = 0;

	[NonSerialized] public bool _active = false;

	[SerializeField] private Text _name_text;

	private float _max_scale = 2.0f, _min_scale = 0.5f, _min_dist = 0.1f, _max_dist = 40.0f;

	public EnemyFloatingTargetingUI i_initialize(BaseEnemy itr_enemy) {
		_current_mode = EnemyFloatingTargetingUIMode.FadeIn;
		_active = true;
		_max_scale *= itr_enemy.get_reticule_scale();
		_min_scale *= itr_enemy.get_reticule_scale();
		_reticule_anim_t = 1.0f;
		_retic_target_alpha = _reticule_image.color.a;
		update_reticule_in_anim();

		_infodisp_root.SetActive(false);
		_name_text.text = itr_enemy.get_name();
		health_bar_fill_pct(itr_enemy._current_health/itr_enemy.get_max_health());

		return this;
	}

	private void update_reticule_in_anim() {
		_reticule_transform.localScale = Util.valv(1.0f+1.0f*_reticule_anim_t);
		Color neu_retic_color = _reticule_image.color;
		neu_retic_color.a = (1-_reticule_anim_t)*_retic_target_alpha;
		_reticule_image.color = neu_retic_color;
	}
	
	public void i_update(BaseEnemy itr_enemy, BattleGameEngine game) {
		_active = true;
		float dist = Util.vec_dist(game._sceneref._player.transform.position,itr_enemy.get_center());
		dist = Mathf.Clamp(dist,_min_dist,_max_dist);
		float val = (_max_scale-_min_scale) * (1-(dist-_min_dist)/(_max_dist-_min_dist)) + _min_scale;
		this.transform.localScale = Util.valv(val);

		health_bar_fill_pct(itr_enemy._current_health/itr_enemy.get_max_health());
	}
	
	public void fadeout() {
		_current_mode = EnemyFloatingTargetingUIMode.FadeOut;
	}
	
	public bool should_remove() {
		return (_current_mode == EnemyFloatingTargetingUIMode.FadeOut) && (_reticule_anim_t >= 1);
	}

	[SerializeField] private GameObject _bar_fill_obj;
	[SerializeField] private RectTransform _bar_fill;
	private static float BAR_MAX = 150;
	private static float BAR_MIN = 13;
	private void health_bar_fill_pct(float pct) {
		float tar_wid = pct*BAR_MAX;
		if (tar_wid < BAR_MIN) {
			_bar_fill_obj.SetActive(false);
		} else {
			_bar_fill_obj.SetActive(true);
			Vector2 prev_size = _bar_fill.sizeDelta;
			prev_size.x = tar_wid;
			_bar_fill.sizeDelta = prev_size;
		}
	}
	
	public void Update() {
		if (_current_mode == EnemyFloatingTargetingUIMode.FadeIn) {
			_reticule_anim_t = Mathf.Clamp(_reticule_anim_t-0.1f,0,1);
			if (_reticule_anim_t <= 0) _current_mode = EnemyFloatingTargetingUIMode.Idle;
			_infodisp_root.SetActive(false);

		} else if (_current_mode == EnemyFloatingTargetingUIMode.Idle) {
			_infodisp_root.SetActive(true);

		} else if (_current_mode == EnemyFloatingTargetingUIMode.FadeOut) {
			_reticule_anim_t = Mathf.Clamp(_reticule_anim_t+0.1f,0,1);
			_infodisp_root.SetActive(false);
		}
		update_reticule_in_anim();
	}

}
