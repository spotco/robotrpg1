using UnityEngine;
using System.Collections;

public class SceneRef : MonoBehaviour {

	[SerializeField] public GameObject _transition_camera;
	[SerializeField] public GameObject _topdown_camera;
	[SerializeField] public PlayerCharacter _player;
	
	public void Start() {		
		this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
	}
	
}
