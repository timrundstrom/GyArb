using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
	// Recording a projectiles values at point in time

	public Vector3 position;
	public float velocityX;
	public float velocityY;
	public float velocityZ;
	public float verticalAngle;
	public float horizontalAngle;
	public float time;

	public Point(Vector3 position, float velocityX, float velocityY, float velocityZ, float time) {
		this.position = position;
		this.velocityX = velocityX;
		this.velocityY = velocityY;
		this.velocityZ = velocityZ;
		this.time = time;

		float xz = Mathf.Sqrt(velocityX * velocityX + velocityZ * velocityZ);
		verticalAngle = Mathf.Atan(velocityY / xz) * Mathf.Rad2Deg;
		horizontalAngle = Mathf.Atan(velocityZ / velocityX) * Mathf.Rad2Deg;
	}

}
