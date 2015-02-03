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
	[SerializeField] private Text _damage_text;
	[SerializeField] private Text _distance_text;

	[SerializeField] private Vector3 _preferred_local_position;

	private float _reticule_anim_t = 0; //1 out, 0 in
	private float _retic_target_alpha = 0;

	[NonSerialized] public bool _active = false;

	[SerializeField] private Text _name_text;

	private float _max_scale = 2.0f, _min_scale = 0.75f, _min_dist = 0.1f, _max_dist = 40.0f;

	public EnemyFloatingTargetingUI i_initialize(BaseEnemy itr_enemy) {
		_current_mode = EnemyFloatingTargetingUIMode.FadeIn;
		_active = true;
		_max_scale *= itr_enemy.get_reticule_scale();
		_min_scale *= itr_enemy.get_reticule_scale();
		_reticule_anim_t = 1.0f;
		_retic_target_alpha = _reticule_image.color.a;
		update_reticule_in_anim();

		_infodisp_root.SetActive(false);
		_line_to_bar.gameObject.SetActive(false);
		_name_text.text = itr_enemy.get_name();
		health_bar_fill_pct(itr_enemy._current_health/itr_enemy.get_max_health());

		_damage_text.text = "";
		_distance_text.text = "";

		return this;
	}

	public Vector3 get_infodisp_preferred_world_position() {
		RectTransform rt = _infodisp_root.GetComponent<RectTransform>();
		Vector3 cur_local_pos = rt.localPosition;
		rt.localPosition = _preferred_local_position;
		Vector3 rtv = rt.position;
		rt.localPosition = cur_local_pos;
		return rtv;
	}

	public Rect get_infodisp_size() {
		RectTransform rt = _infodisp_root.GetComponent<RectTransform>();
		Vector3 cur_local_pos = rt.localPosition;
		rt.localPosition = _preferred_local_position;

		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		float min_x = Mathf.Min(corners[0].x,corners[1].x,corners[2].x,corners[3].x);
		float max_x = Mathf.Max(corners[0].x,corners[1].x,corners[2].x,corners[3].x);
		float min_y = Mathf.Min(corners[0].y,corners[1].y,corners[2].y,corners[3].y);
		float max_y = Mathf.Max(corners[0].y,corners[1].y,corners[2].y,corners[3].y);
		Rect rtv = new Rect(min_x,max_y,max_x-min_x,max_y-min_y);

		rt.localPosition = cur_local_pos;

		return rtv;
	}

	public float get_sort_value() {
		return _infodisp_root.GetComponent<RectTransform>().position.y;
	}

	private bool _enemy_alive = true;
	public bool infodisp_reposition_active() {
		return _enemy_alive;
	}

	private Vector3 _target_infodisp_offset = Vector3.zero;
	private Vector3 _current_infodisp_offset = Vector3.zero;
	public void set_offset(float offset_x, float offset_y) {
		_target_infodisp_offset = new Vector3(offset_x,offset_y);
	}

	private void update_reticule_in_anim() {
		_reticule_transform.localScale = Util.valv(1.0f+1.0f*_reticule_anim_t);
		Color neu_retic_color = _reticule_image.color;
		neu_retic_color.a = (1-_reticule_anim_t)*_retic_target_alpha;
		_reticule_image.color = neu_retic_color;
	}
	
	public void i_update(BaseEnemy itr_enemy, BattleGameEngine game) {
		if (itr_enemy._alive) {
			_active = true;
			float dist = Util.vec_dist(game._sceneref._player.transform.position,itr_enemy.get_center());
			dist = Mathf.Clamp(dist,_min_dist,_max_dist);
			float val = (_max_scale-_min_scale) * (1-(dist-_min_dist)/(_max_dist-_min_dist)) + _min_scale;
			this.transform.localScale = Util.valv(val);

			health_bar_fill_pct(itr_enemy._current_health/itr_enemy.get_max_health());

			_damage_text.text = string.Format("{0}/{1}",itr_enemy._current_health,itr_enemy.get_max_health());
			_distance_text.text = string.Format("{0:F1}m",dist);
		}
		_enemy_alive = itr_enemy._alive;
	}
	
	public void fadeout() {
		_current_mode = EnemyFloatingTargetingUIMode.FadeOut;
	}
	
	public bool should_remove() {
		return (_current_mode == EnemyFloatingTargetingUIMode.FadeOut) && (_reticule_anim_t >= 1);
	}
	
	[SerializeField] private Image _bar_fill;
	private void health_bar_fill_pct(float pct) {
		_bar_fill.fillAmount = pct;
	}

	[SerializeField] private RectTransform _line_to_bar;
	private bool update_line_to_bar() {
		Vector3 delta = Util.vec_sub(_infodisp_root.transform.localPosition,_line_to_bar.transform.localPosition);
		Vector2 size = _line_to_bar.sizeDelta;
		size.y = delta.magnitude;
		_line_to_bar.sizeDelta = size;
		Util.transform_set_euler_world(_line_to_bar,new Vector3(0,0,Mathf.Atan2(delta.y,delta.x)*Util.rad2deg - 90));
		return delta.magnitude > 20;
	}
	
	public void Update() {
		if (_current_mode == EnemyFloatingTargetingUIMode.FadeIn) {
			_reticule_anim_t = Mathf.Clamp(_reticule_anim_t-0.1f,0,1);
			if (_reticule_anim_t <= 0) _current_mode = EnemyFloatingTargetingUIMode.Idle;
			_infodisp_root.SetActive(false);
			_line_to_bar.gameObject.SetActive(false);

		} else if (_current_mode == EnemyFloatingTargetingUIMode.Idle) {
			_infodisp_root.SetActive(true);
			_line_to_bar.gameObject.SetActive(true);

		} else if (_current_mode == EnemyFloatingTargetingUIMode.FadeOut) {
			_reticule_anim_t = Mathf.Clamp(_reticule_anim_t+0.1f,0,1);
			_infodisp_root.SetActive(false);
			_line_to_bar.gameObject.SetActive(false);
		}
		update_reticule_in_anim();

		RectTransform rt = _infodisp_root.GetComponent<RectTransform>();
		_current_infodisp_offset = Util.vec_drp(_current_infodisp_offset,_target_infodisp_offset,0.85f);
		rt.localPosition = new Vector3(_preferred_local_position.x+_current_infodisp_offset.x,_preferred_local_position.y+_current_infodisp_offset.y,0);

		_line_to_bar.gameObject.SetActive(update_line_to_bar() && _line_to_bar.gameObject.activeSelf);
	}

}
