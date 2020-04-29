using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class BallGhost : MonoBehaviour
{

	MeshRenderer mesh;
	public GameObject text;
	MeshRenderer textMesh;
	public bool alwaysShow = false;


	private void Start() {
		mesh = GetComponent<MeshRenderer>();
		textMesh = text.GetComponent<MeshRenderer>();
	}

	public void Display() {
		mesh.enabled = true;
		textMesh.enabled = true;
	}

	public void EndDisplay() {
		if (!alwaysShow) {
			mesh.enabled = false;
			textMesh.enabled = false;
		}
	}

}
