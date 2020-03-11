using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
	// Recording a projectiles values at point in time

	public Vector3 position;
	public Vector3 velocities;
	public float verticalAngle;
	public float horizontalAngle;
	public float time;

	
	public Point(Vector3 position, Vector3 velocities, float time)
	{
		this.position = position;
		this.velocities = velocities;
		this.time = time;

		float xz = Mathf.Sqrt(velocities.x * velocities.x + velocities.z * velocities.z);
		float xy = Mathf.Sqrt(velocities.x * velocities.x + velocities.y * velocities.y);
		verticalAngle = Mathf.Atan(velocities.y / xz) * Mathf.Rad2Deg;
		horizontalAngle = Mathf.Atan(velocities.z / xy) * Mathf.Rad2Deg;
	}

}
