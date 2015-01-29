using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ColliderPointer : MonoBehaviour {
	[SerializeField] public BaseEnemy _ptr_enemy;
	[SerializeField] public WorldTerrain _ptr_terrain;
	[SerializeField] public WorldProp _ptr_prop;
	[SerializeField] public PlayerCharacter _ptr_player;

	public enum Type {
		BaseEnemy,
		WorldTerrain,
		WorldProp,
		PlayerCharacter,
		ERR
	}

	public ColliderPointer.Type get_type() {
		int ct = 0;
		if (_ptr_enemy != null) ct++;
		if (_ptr_terrain != null) ct++;
		if (_ptr_prop != null) ct++;
		if (_ptr_player != null) ct++;
		if (ct != 1) {
			Debug.LogError(string.Format("SPERROR::ColliderPointer({0})",ct));
			return ColliderPointer.Type.ERR;
		}

		if (_ptr_enemy != null) return ColliderPointer.Type.BaseEnemy;
		if (_ptr_terrain != null) return ColliderPointer.Type.WorldTerrain;
		if (_ptr_prop != null) return ColliderPointer.Type.WorldProp;
		if (_ptr_player != null) return ColliderPointer.Type.PlayerCharacter;
		return ColliderPointer.Type.ERR;
	}

	public Collider get_collider() {
		return this.GetComponent<Collider>();
	}

	public static ColliderPointer cgetp(Collider col) {
		if (col.gameObject.GetComponent<ColliderPointer>() == null) Debug.LogError(string.Format("SPERROR::Collider no pointer {0}",col.gameObject.name));
		return col.gameObject.GetComponent<ColliderPointer>();
	}
}
