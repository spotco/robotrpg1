using UnityEngine;
using System.Collections;

public class BaseParticle : MonoBehaviour {
	public virtual void i_initialize(BattleGameEngine game) {}
	public virtual void i_update(BattleGameEngine game) {}
	public virtual bool should_remove(BattleGameEngine game) { return true; }
	public virtual void do_remove(BattleGameEngine game) {}
	public virtual void freeze() {}
	public virtual void unfreeze() {}

	public BaseParticle set_position(Vector3 pos) {
		this.gameObject.transform.position = pos;
		return this;
	}

}
