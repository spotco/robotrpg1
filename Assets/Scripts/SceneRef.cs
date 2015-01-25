﻿using UnityEngine;
using System.Collections.Generic;

public class SceneRef : MonoBehaviour {

	public static SceneRef inst;
	
	[SerializeField] public GameObject _transition_camera;
	[SerializeField] public GameObject _topdown_camera;
	[SerializeField] public PlayerCharacter _player;
	
	[SerializeField] private GameObject _enemies_container;
	public List<BaseEnemy> _enemies;
	
	public void Start() {
		inst = this;
		
		_enemies = new List<BaseEnemy>(_enemies_container.GetComponentsInChildren<BaseEnemy>());
		this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
	}
	
}
