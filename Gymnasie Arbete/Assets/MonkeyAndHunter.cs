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
    float startSpeed = 0;
	Vector3 oldPosition = new Vector3();

	float g = 9.82f;
	bool startSimulation = false;
	bool simulating = false;

    float getTime = 0;
    Vector3 getOldVelocities = new Vector3();
    float getOldVelocity;


	private void Start() {
		mesh = GetComponent<MeshRenderer>();
		interactable = GetComponent<Interactable>();
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 0;

	}

	private void Update()
    {

        //handle start sim input
        if (interactable.attachedToHand != null) {
            //look for start sim button
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            if (simAction[source].stateDown) {
                if (startSimulation)
                {
                    startSimulation = false;
                    mesh.material = mainMaterial;
                }
                else
                {
                    startSimulation = true;
                    mesh.material = simMaterial;

                    //clear old sim
                    points.Clear(); // Clear points
                    pointIndex.Clear();
                    ghosts.Clear();
                    lr.positionCount = 0; // Remove line renederer arc
                    foreach(GameObject ghostO in GameObject.FindGameObjectsWithTag("ghost"))
                    {
                        Destroy(ghostO);
                    }
                }
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
		
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (simulating)
            {
                
                float EndsimX = Mathf.Abs(startX - transform.position.x);
                Point point1 = new Point(transform.position, getOldVelocities, getOldVelocity, getTime, EndsimX, transform.position.y);

                var lastGhost = Instantiate(ghost, transform.position, Quaternion.identity);
                lastGhost.tag = "ghost";
                lastGhost.GetComponentsInChildren<TextMesh>()[0].text = point1.text;
                lastGhost.GetComponent<BallGhost>().alwaysShow = true;

                simulating = false;
                //set material
                mesh.material = mainMaterial;
            }
        }
	}


	IEnumerator Simulation() {
		simulating = true;
		float elapsed_time = 0f;
		float distance = 0f;

        startSpeeds.x = Mathf.Abs(transform.position.x - oldPosition.x) / Time.deltaTime;
        startSpeeds.y = Mathf.Abs(transform.position.y - oldPosition.y) / Time.deltaTime;
        startSpeeds.z = Mathf.Abs(transform.position.z - oldPosition.z) / Time.deltaTime;
        startSpeed = Vector3.Distance(transform.position, oldPosition) / Time.deltaTime;
        // set x at time=0
        startX = transform.position.x;

        bool recordFirst = true;

		Vector3 simOldPosition = transform.position;
		while (simulating) {

			if(distance > .4f && transform.position.x - simOldPosition.x != 0) {
				ghosts.Add(transform.position);
				pointIndex.Add(points.Count-1);
				distance = 0f;
			}
			//get speeds
			Vector3 speeds = new Vector3();
            //horizontal velocties does now change by itself
            speeds.x = startSpeeds.x;
            speeds.y = startSpeeds.y - (9.82f * elapsed_time);
            speeds.z = startSpeeds.z;
            
            //pythagoras sats
            float speedxy = Mathf.Sqrt(Mathf.Pow(speeds.x, 2) + Mathf.Pow(speeds.y, 2));
            float speed = Mathf.Sqrt(Mathf.Pow(speedxy, 2) + Mathf.Pow(speeds.z, 2));

			//get distance
			float simX = Mathf.Abs(startX - transform.position.x);
            //make a point in time
            Point point;
            if (recordFirst)
            {
                point = new Point(transform.position, startSpeeds, startSpeed, 0, 0, transform.position.y - (transform.localScale.y / 2));
                ghosts.Add(transform.position);
                pointIndex.Add(0);
                distance = 0;
                recordFirst = false;
            } else
            {
                point = new Point(transform.position, speeds, speed, elapsed_time, simX, transform.position.y - (transform.localScale.y / 2));
            }
            points.Add(point);
			//set new value for next iteration
			elapsed_time += Time.deltaTime;
			distance += Vector3.Distance(simOldPosition, transform.position);
			simOldPosition = transform.position;

            getOldVelocities = speeds;
            getOldVelocity = speed;
            getTime = elapsed_time;

			yield return null;
		}
        points.RemoveAt(1);
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
        for(int i = 0; i < ghosts.Count-1; i++) {
			// Instantiate ghost on every point
			var newGhost = Instantiate(ghost, ghosts[i], Quaternion.identity);
            newGhost.tag = "ghost";

			newGhost.GetComponentsInChildren<TextMesh>()[0].text = points[pointIndex[i]].text;
			if(i == 0)
            {
                newGhost.GetComponent<BallGhost>().alwaysShow = true;
            }
		}

    }

}
