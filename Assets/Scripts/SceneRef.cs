using UnityEngine;
using System.Collections.Generic;

public class SceneRef : MonoBehaviour {

	public static SceneRef inst;
	
	[SerializeField] public GameObject _transition_camera;
	[SerializeField] public GameObject _topdown_camera;
	[SerializeField] public GameObject _particle_root;
	[SerializeField] public PlayerCharacter _player;
	[SerializeField] public GameUI _ui;
	
	[SerializeField] private GameObject _enemies_container;
	[SerializeField] public PathRenderer _path_renderer;

	public List<BaseEnemy> _enemies;
	
	public void Start() {
		inst = this;
		ShaderManager.i_initialize();
		_enemies = new List<BaseEnemy>(_enemies_container.GetComponentsInChildren<BaseEnemy>());
		this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
	}
	
}
