using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Constantly record velocities. And just paint out ghosts as it is thrown using unity physics and display the velocities at each ghost.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Interactable))]
public class VelocityInteractable : MonoBehaviour
{
	LineRenderer lr;
	Interactable interactable;
	public GameObject ghost;

	List<Point> points = new List<Point>();
	List<Vector3> ghosts = new List<Vector3>();

	Vector3 startVelocities = new Vector3();
	Vector3 oldPosition = new Vector3();

	bool simulating = false;
	bool setToSimulate = false;

	void Start()
	{
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 0;

		interactable = GetComponent<Interactable>();
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
				StartCoroutine(Simulate()); // Begin simulation
			}
		}

	}
	IEnumerator Simulate()
	{
		float elapsed_time = 0;
		float distance = 0;

		ghosts.Add(transform.position);

		Vector3 simOldPosition = transform.position;
		while (gameObject.transform.position.y > 0)
		{

			// Create and add the point to the list of points
			if (distance > 1)
			{
				ghosts.Add(transform.position);
				distance = 0;
			}
			

			Vector3 velocities = (transform.position - simOldPosition) / Time.deltaTime;
			Point newPoint = new Point(transform.position, velocities, elapsed_time);
			points.Add(newPoint);

			elapsed_time += Time.deltaTime; // Update current time
			distance += Vector3.Distance(simOldPosition, transform.position);
			simOldPosition = transform.position;
			yield return null;
		}

		// Last ghost and gameobject is too close to eachother
		if (Vector3.Distance(ghosts[ghosts.Count - 1], transform.position) < .5f)
			ghosts.RemoveAt(ghosts.Count - 1);

		DrawLineRenderer(); // Draw the line renderer arc using all the points positions
							//points = ShortenList(points, Convert.ToInt32(distance)); // Shorten down the list to size calculated above
		DrawGhosts(); // Draw the ghosts
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
