using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Throwable))]
public class MonkeyAndHunter : MonoBehaviour {

	public SteamVR_Action_Boolean simAction;
	LineRenderer lr;
	Interactable interactable;
	MeshRenderer mesh;
	public GameObject ghost;

	List<Point> points = new List<Point>();
	List<Vector3> ghosts = new List<Vector3>();
	List<int> pointIndex = new List<int>();

	public Material mainMaterial;
	public Material simMaterial;

	float startX = 0f;
	Vector3 startSpeeds = new Vector3();
	Vector3 oldPosition = new Vector3();

	float g = 9.82f;
	bool startSimulation = false;
	bool simulating = false;

	private void Start() {
		mesh = GetComponent<MeshRenderer>();
		interactable = GetComponent<Interactable>();
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 0;

	}

	private void Update() {

		if(interactable.attachedToHand != null) {
			//look for start sim button
			SteamVR_Input_Sources source = interactable.attachedToHand.handType;
			if (simAction[source].stateDown) {
				startSimulation = true;
				mesh.material = simMaterial;

				//clear old sim
				points.Clear(); // Clear points
				pointIndex.Clear();
				ghosts.Clear();
				lr.positionCount = 0; // Remove line renederer arc
				foreach (Transform child in transform) { // Go through every ghost and destroy them
					GameObject.Destroy(child.gameObject);
				}
			}

			if (startSimulation) {
				// set starting speed
				startSpeeds.x = Mathf.Abs(transform.position.x - oldPosition.x) / Time.deltaTime;
				startSpeeds.y = Mathf.Abs(transform.position.y - oldPosition.y) / Time.deltaTime;
				startSpeeds.z = Mathf.Abs(transform.position.z - oldPosition.z) / Time.deltaTime;
				// set x at time=0
				startX = transform.position.x;
			}

		} else {
			if (startSimulation) {
				startSimulation = false;
				StartCoroutine(Simulation());
			}
		}
		oldPosition = transform.position;
	}

	//on collision end bool simulating
	private void OnCollisionEnter(Collision collision) {
		if (simulating) {
			simulating = false;
			//set material
			mesh.material = mainMaterial;
		}
	}


	IEnumerator Simulation() {
		simulating = true;
		float elapsed_time = 0f;
		float distance = 0f;

		ghosts.Add(transform.position);
		pointIndex.Add(0);
		Vector3 simOldPosition = transform.position;
		while (simulating) {

			if(distance > 1f) {
				ghosts.Add(transform.position);
				pointIndex.Add(points.Count-1);
				distance = 0f;
			}
			//get speeds
			Vector3 speeds = new Vector3();
			speeds.x = Mathf.Abs(transform.position.x - simOldPosition.x) / Time.deltaTime;
			speeds.y = Mathf.Abs(transform.position.y - simOldPosition.y) / Time.deltaTime;
			speeds.z = Mathf.Abs(transform.position.z - simOldPosition.z) / Time.deltaTime;
			//get distance
			float simX = Mathf.Abs(startX - transform.position.x);
			//make a point in time
			Point point = new Point(transform.position, speeds, elapsed_time, simX, transform.position.y);
			points.Add(point);
			//set new value for next iteration
			elapsed_time += Time.deltaTime;
			distance += Vector3.Distance(simOldPosition, transform.position);
			simOldPosition = transform.position;
			yield return null;
		}

		DrawLineRenderer(); 
		DrawGhosts();
	}

	void DrawLineRenderer() {
		// Set size
		lr.startWidth = 0.02f;
		lr.endWidth = 0.02f;

		// Set materials and color
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.startColor = new Color(0, 0, 0, .7f);
		lr.endColor = new Color(0, 0, 0, .7f);

		// Draw a line through every point
		lr.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++) {
			lr.SetPosition(i, points[i].position);
		}
	}

	void DrawGhosts() {
		// Set every ghost's parent to the script-objects' transform, in order to clear them easily later. 
		// Loop through points
		int counter = 0;
		foreach (Vector3 position in ghosts) {
			// Instantiate ghost on every point
			var newGhost = Instantiate(ghost, position, Quaternion.identity);
			newGhost.transform.parent = gameObject.transform;

			newGhost.GetComponentsInChildren<TextMesh>()[0].text = points[pointIndex[counter]].text;
			if (counter != 0 || counter == ghosts.Count-1) {
				newGhost.GetComponent<MeshRenderer>().enabled = false;
				newGhost.GetComponentsInChildren<MeshRenderer>()[0].enabled = false;
			} else {
				newGhost.GetComponent<BallGhost>().alwaysShow = true;
			}

			counter++;
		}
	}

}
