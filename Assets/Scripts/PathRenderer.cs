using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PathRenderer : MonoBehaviour {

	[SerializeField] private GameObject _arrow_dot_proto;
	[SerializeField] private GameObject _arrow_head_proto;

	private void Start () {
		_arrow_dot_proto.SetActive(false);
		_arrow_head_proto.SetActive(false);
	}

	//TODO -- pool me
	private MultiList<int,GameObject> _id_to_objs = new MultiList<int, GameObject>();
	public void id_draw_path(int id, Vector3 position, Vector3[] points) {
		if (_id_to_theta.ContainsKey(id)) _id_to_theta[id] = 0.0f;
		float dist_per = 0.5f;
		Vector3 last = position;
		float last_remainder = 0;
		for (int i = 1; i < points.Length; i++) {
			Vector3 itr = points[i];
			float itr_dist_total = Util.vec_dist(last,itr);
			float itr_dist = 0;
			if (i == 1) itr_dist = dist_per;
			while (itr_dist < itr_dist_total) {
				Vector3 neu_obj_pos = Vector3.Lerp(last,itr,itr_dist/itr_dist_total);
				
				GameObject neu_obj = Util.proto_clone(_arrow_dot_proto);
				neu_obj.transform.position = new Vector3(neu_obj_pos.x,neu_obj.transform.position.y,neu_obj_pos.z);
				_id_to_objs.add(id,neu_obj);
				
				itr_dist += dist_per;
			}
			
			if (i == points.Length-1) {
				GameObject neu_obj2 = Util.proto_clone(_arrow_head_proto);
				neu_obj2.transform.position = new Vector3(itr.x,neu_obj2.transform.position.y + 0.1f,itr.z);
				Util.transform_set_euler_world(neu_obj2.transform,new Vector3(90,-Mathf.Atan2(itr.z-last.z,itr.x-last.x)*Util.rad2deg + 90.0f));
				_id_to_objs.add(id,neu_obj2);
				
			}
			
			last_remainder = (itr_dist_total - itr_dist);
			last = itr;
		}
	}

	public void clear_path(int id) {
		foreach(GameObject itr in _id_to_objs.list(id)) {
			Destroy(itr);
		}
		_id_to_objs.clear(id);
	}

	private Dictionary<int, float> _id_to_theta = new Dictionary<int, float>();
	public void Update() {
		foreach(int id in _id_to_objs.keys()) {
			if (_id_to_objs.count_of(id) > 0) {
				if (!_id_to_theta.ContainsKey(id)) _id_to_theta[id] = 0.0f;
				float val = _id_to_theta[id];
				List<GameObject> list = _id_to_objs.list(id);

				val += 0.5f * list.Count * 0.02f;
				if (val > list.Count*1.35f) val = -list.Count*0.35f;

				for (int i_list = 0; i_list < list.Count; i_list++) {
					SpriteRenderer itr_list = list[i_list].GetComponent<SpriteRenderer>();
					if (itr_list.sprite.name != "move_arrow_cross") {
						Color itr_list_color = itr_list.color;
						float aval = Mathf.Pow(1-(Mathf.Abs(i_list-val))/list.Count,4.0f);
						itr_list_color.a = Mathf.Max(aval,0.25f);
						itr_list.color = itr_list_color;
						itr_list.transform.localScale = Util.valv(0.75f + aval * 0.5f);
					}
				}

				_id_to_theta[id] = val;
			}
		}
	}
}
