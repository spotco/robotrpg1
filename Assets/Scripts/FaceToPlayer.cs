using UnityEngine;
using System.Collections;

public class FaceToPlayer : MonoBehaviour {

	void Update () {
		if (SceneRef.inst != null && SceneRef.inst._player != null) {
			this.transform.LookAt(SceneRef.inst._player._follow_camera.transform.position);
		}
	}
}
