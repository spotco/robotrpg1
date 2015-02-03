using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class GameUI : MonoBehaviour {
	
	[SerializeField] public GameObject _proto_target_reticule;
	[SerializeField] public GameObject _target_reticule_root;
	
	public void i_initialize(BattleGameEngine game) {
		_proto_target_reticule.gameObject.SetActive(false);
	}
	
	private Dictionary<int,EnemyFloatingTargetingUI> _objid_to_targetingui = new Dictionary<int, EnemyFloatingTargetingUI>();
	private List<int> _objsids_to_remove = new List<int>();
	private List<Rect> _ui_infopanel_rects = new List<Rect>();
	public void i_update(BattleGameEngine game) {
		foreach(EnemyFloatingTargetingUI fui in _objid_to_targetingui.Values) {
			fui._active = false;
		}

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


		foreach(EnemyFloatingTargetingUI fui in _objid_to_targetingui.Values) {
			if (!fui.infodisp_reposition_active()) continue;
			Rect fui_rect = fui.get_infodisp_size();
			bool intersect_ok = false;
			float offset_y = 0;
			int intersect_test_ct = 0;
			while (!intersect_ok) {
				bool intersection_found = false;
				foreach(Rect r in _ui_infopanel_rects) {
					if (fui_rect.Overlaps(r)) {
						intersection_found = true;
						break;
					}
				}
				if (intersection_found) {
					float OVERLAP_TEST_ITR = 15.0f;
					offset_y += OVERLAP_TEST_ITR;
					fui_rect.y += OVERLAP_TEST_ITR;
				} else {
					intersect_ok = true;
				}
				intersect_test_ct++;
				if (intersect_test_ct > 50) intersect_ok = true;
			}
			fui.set_offset(0,offset_y);
			_ui_infopanel_rects.Add(fui_rect);
		}
		_ui_infopanel_rects.Clear();

		var sort_targeting_uis = _target_reticule_root.GetComponentsInChildren<EnemyFloatingTargetingUI>(false).ToList();
		sort_targeting_uis.Sort((EnemyFloatingTargetingUI a, EnemyFloatingTargetingUI b)=> {
			return Util.sig(b.get_sort_value() - a.get_sort_value());
		});
		for (int i = 0; i < sort_targeting_uis.Count; i++) {
			sort_targeting_uis[i].transform.SetSiblingIndex(i);
		}
		
		update_aim_reticule();
		update_particles();
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

	[SerializeField] public GameObject _particle_root;
	[NonSerialized] public List<BaseUIParticle> _particles = new List<BaseUIParticle>();
	public BaseUIParticle add_particle(string name) {
		BaseUIParticle particle = ((GameObject)Instantiate(Resources.Load(name))).GetComponent<BaseUIParticle>();
		particle.i_initialize(this);
		particle.gameObject.transform.parent = _particle_root.transform;
		_particles.Add(particle);
		return particle;
	}

	private void update_particles() {
		for (int i_particle = _particles.Count-1; i_particle >= 0; i_particle--) {
			BaseUIParticle itr_particle = _particles[i_particle];
			itr_particle.i_update(this);
			if (itr_particle.should_remove(this)) {
				itr_particle.do_remove(this);
				itr_particle.gameObject.transform.parent = null;
				Destroy(itr_particle.gameObject);
				_particles.RemoveAt(i_particle);
			}
		}
	}
	
}
