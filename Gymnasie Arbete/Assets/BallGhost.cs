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

    public Material ghostmat;
    public Material ghostinvis;

	public bool alwaysShow = false;

    double creationTime;

	private void Start() {

        gameObject.GetComponent<SphereCollider>().enabled = false;
        creationTime = 0;

		mesh = GetComponent<MeshRenderer>();
		textMesh = text.GetComponent<MeshRenderer>();

        if (alwaysShow)
        {
            mesh.material = ghostmat;
            textMesh.enabled = true;
        }
        else
        {
            mesh.material = ghostinvis;
            textMesh.enabled = false;
        }
    }

    private void Update()
    {

        if (creationTime > .5f)
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }else
        {
            creationTime += Time.deltaTime;
        }

        text.transform.LookAt(Camera.main.transform);
        text.transform.Rotate(new Vector3(0, 180, 0));
    }

    public void Display() {
        mesh.material = ghostmat;
        textMesh.enabled = true;
    }

	public void EndDisplay() {
        if (!alwaysShow)
        {
            mesh.material = ghostinvis;
            textMesh.enabled = false;
        }
	}

    

}
