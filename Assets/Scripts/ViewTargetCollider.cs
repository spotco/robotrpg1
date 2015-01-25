using UnityEngine;
using System.Collections.Generic;

public class ViewTargetCollider : MonoBehaviour {
	
	public HashSet<BaseEnemy> _enemies = new HashSet<BaseEnemy>();
	
	public void OnTriggerEnter(Collider col) {
		BaseEnemy enemy_component = col.gameObject.GetComponent<BaseEnemy>();
		if (enemy_component != null) {
			_enemies.Add(enemy_component);		
		}
	}
	
	public void OnTriggerExit(Collider col) {
		BaseEnemy enemy_component = col.gameObject.GetComponent<BaseEnemy>();
		if (enemy_component != null && _enemies.Contains(enemy_component)) {
			_enemies.Remove(enemy_component);		
		}
	}
	
	public void Update() {
		Debug.Log(_enemies.Count);
	}
}
