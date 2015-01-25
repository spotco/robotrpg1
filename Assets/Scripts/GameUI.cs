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
				fui._animator.gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(itr_enemy.get_center());
			}
			if (!game._sceneref._player.point_in_fov(itr_enemy.transform.position)) continue;
			if (!_objid_to_targetingui.ContainsKey(itr_enemy.gameObject.GetInstanceID())) {
				_objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()] = Util.proto_clone(_proto_target_reticule).GetComponent<EnemyFloatingTargetingUI>().i_initialize(itr_enemy.get_reticule_scale());
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
	}
	
}
