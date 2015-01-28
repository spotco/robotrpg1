using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemWrapperParticle : BaseParticle {
	public static string BULLET_IMPACT = "Particles/particle_bullet_impact";

	public override void i_initialize(BattleGameEngine game) {
	}
	public override void i_update(BattleGameEngine game) {
	}
	public override bool should_remove(BattleGameEngine game) { 
		return !this.particleSystem.IsAlive(); 
	}
	public override void do_remove(BattleGameEngine game) {}

	public override void freeze() {
		particleSystem.Pause();
	}
	public override void unfreeze() {
		particleSystem.Play();
	}
}
