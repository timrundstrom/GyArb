using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;


/// <summary>
/// Record the objects velocity while in the hand and then simulate on release.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Rigidbody))]
public class SimulatedInteractable : MonoBehaviour
{

	LineRenderer lr;
	Interactable interactable;
	Rigidbody rb;
	public GameObject ghost;

	List<Point> points = new List<Point>();
	List<Vector3> ghosts = new List<Vector3>();

	Vector3 startVelocities = new Vector3();
	Vector3 oldPosition = new Vector3();

	float g = 9.82f;
	bool simulating = false;
	bool setToSimulate = false;

	void Awake()
	{
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 0;

		interactable = GetComponent<Interactable>();
		rb = GetComponent<Rigidbody>();
	}
	

	void Update()
	{
		if (interactable.attachedToHand != null)
		{
			setToSimulate = true;
			// CLEAR VALUES: REMOVE OLD SIM
			points.Clear(); // Clear points
			ghosts.Clear();
			lr.positionCount = 0; // Remove line renederer arc
			foreach (Transform child in transform)
			{ // Go through every ghost and destroy them
				GameObject.Destroy(child.gameObject);
			}

			//RECORD VELOCITY
			startVelocities = (transform.position - oldPosition) / Time.deltaTime;
			oldPosition = transform.position;
		}
		else
		{
			if (setToSimulate)
			{
				setToSimulate = false;
				simulating = true;
				rb.isKinematic = true;
				StartCoroutine(Simulate()); // Begin simulation
			}
		}
		
	}

	IEnumerator Simulate()
	{
		float Vx = startVelocities.x;
		float Vy = startVelocities.y;
		float Vz = startVelocities.z;
		float elapsed_time = 0;
		float distance = 0;

		ghosts.Add(transform.position);

		while (gameObject.transform.position.y > 0)
		{
			Vector3 oldPosition = transform.position;
			Vy = startVelocities.y - (g * elapsed_time); // Update Vy
			transform.Translate(Vx * Time.deltaTime, Vy * Time.deltaTime, Vz * Time.deltaTime); // Move projectile

			// Create and add the point to the list of points
			if (distance > 1)
			{
				ghosts.Add(transform.position);
				distance = 0;
			}

			Vector3 velocities = new Vector3(Vx, Vy, Vz);
			Point newPoint = new Point(transform.position, velocities, elapsed_time);
			points.Add(newPoint);

			elapsed_time += Time.deltaTime; // Update current time
			distance += Vector3.Distance(oldPosition, transform.position);
			yield return null;
		}

		// Last ghost and gameobject is too close to eachother
		if (Vector3.Distance(ghosts[ghosts.Count - 1], transform.position) < .5f)
			ghosts.RemoveAt(ghosts.Count - 1);

		DrawLineRenderer(); // Draw the line renderer arc using all the points positions
							//points = ShortenList(points, Convert.ToInt32(distance)); // Shorten down the list to size calculated above
		DrawGhosts(); // Draw the ghosts
		rb.isKinematic = false;
	}

	void DrawLineRenderer()
	{
		// Set size
		lr.startWidth = 0.02f;
		lr.endWidth = 0.02f;

		// Set materials and color
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.startColor = new Color(0, 0, 0, .7f);
		lr.endColor = new Color(0, 0, 0, .7f);

		// Draw a line through every point
		lr.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++)
		{
			lr.SetPosition(i, points[i].position);
		}
	}
	
	void DrawGhosts()
	{
		// Set every ghost's parent to the script-objects' transform, in order to clear them easily later. 
		// Loop through points
		foreach (Vector3 position in ghosts)
		{
			// Instantiate ghost on every point
			var newGhost = Instantiate(ghost, position, Quaternion.identity);
			newGhost.transform.parent = gameObject.transform;
			//GUI.Label(new Rect(newGhost.transform.position, new Vector2(50, 50)), "Helo");
		}
	}
	

}