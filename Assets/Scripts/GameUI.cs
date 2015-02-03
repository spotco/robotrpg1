using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
	
	[SerializeField] public GameObject _proto_target_reticule;
	
	public void i_initialize(BattleGameEngine game) {
		_proto_target_reticule.gameObject.SetActive(false);
	}
	
	private Dictionary<int,EnemyFloatingTargetingUI> _objid_to_targetingui = new Dictionary<int, EnemyFloatingTargetingUI>();
	private List<int> _objsids_to_remove = new List<int>();
	public void i_update(BattleGameEngine game) {
	
		foreach(EnemyFloatingTargetingUI fui in _objid_to_targetingui.Values) fui._active = false;

		foreach(BaseEnemy itr_enemy in game._sceneref._enemies) {
			if (_objid_to_targetingui.ContainsKey(itr_enemy.gameObject.GetInstanceID())) {
				EnemyFloatingTargetingUI fui = _objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()];
				fui.gameObject.transform.position = Camera.main.WorldToScreenPoint(itr_enemy.get_center());
			}
			if (game._sceneref._player.point_angle_from_forward(itr_enemy.transform.position) > 10) continue;
			if (!_objid_to_targetingui.ContainsKey(itr_enemy.gameObject.GetInstanceID())) {
				_objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()] = Util.proto_clone(_proto_target_reticule).GetComponent<EnemyFloatingTargetingUI>().i_initialize(itr_enemy);
			}
			_objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()].i_update(itr_enemy,game);
		}
		
		foreach(int key in _objid_to_targetingui.Keys) {
			if (!_objid_to_targetingui[key]._active) _objid_to_targetingui[key].fadeout();
			if (_objid_to_targetingui[key].should_remove()) _objsids_to_remove.Add(key);
        }
        foreach(int key in _objsids_to_remove) {
			Destroy(_objid_to_targetingui[key].gameObject);
			_objid_to_targetingui.Remove(key);
		}
		_objsids_to_remove.Clear();

		update_aim_reticule();
	}

	private float _aim_reticule_theta = 0;
	private float _aim_reticule_cur_scale = 1, _aim_reticule_tar_scale = 1, _aim_reticule_tar_alpha = 0.3f, _aim_reticule_cur_alpha = 0.3f;
	[SerializeField] private RectTransform _aim_reticule;
	[SerializeField] private Image _aim_reticule_image;
	private void update_aim_reticule() {
		if (Input.GetMouseButton(0)) {
			_aim_reticule_theta += 10.0f;
			_aim_reticule_tar_alpha = 0.47f;
			_aim_reticule_tar_scale = 1.0f;
		} else {
			_aim_reticule_theta += 0.5f;
			_aim_reticule_tar_alpha = 0.2f;
			_aim_reticule_tar_scale = 1.5f;
		}
		_aim_reticule_cur_alpha = Util.drp(_aim_reticule_cur_alpha,_aim_reticule_tar_alpha,0.2f);
		_aim_reticule_cur_scale = Util.drp(_aim_reticule_cur_scale,_aim_reticule_tar_scale,0.2f);

		Util.transform_set_euler_world(_aim_reticule,new Vector3(0,0,_aim_reticule_theta));
		_aim_reticule.localScale = Util.valv(_aim_reticule_cur_scale);
		Color neu_color = _aim_reticule_image.color;
		neu_color.a = _aim_reticule_cur_alpha;
		_aim_reticule_image.color = neu_color;
	}
	
}
