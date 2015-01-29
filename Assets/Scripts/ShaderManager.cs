using UnityEngine;
using System.Collections;

public class ShaderManager {

	public static ShaderManager inst;

	public Shader RGBA_AlphaTest;
	public Shader RGBA_Transparent;

	public static void i_initialize() {
		inst = new ShaderManager();
		inst._initialize();
	}

	private void _initialize() {
		inst = this;
		RGBA_AlphaTest = Shader.Find("RGBA_AlphaTest");
		RGBA_Transparent = Shader.Find("RGBA_Transparent");
	}


}
