using UnityEngine;
using System.Collections;

public class BattleGameEngine : MonoBehaviour {

	private SceneRef _sceneref;

	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;
		
		_sceneref._topdown_camera.SetActive(false);
		_sceneref._transition_camera.SetActive(false);
		_sceneref._player._follow_camera.SetActive(true);
		
		_sceneref._player.i_initialize();
	}
	
	public void Update() {
		_sceneref._player.i_update();
		
	}
}
