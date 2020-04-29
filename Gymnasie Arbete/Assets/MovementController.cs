using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MovementController : MonoBehaviour {

	public SteamVR_Action_Vector2 input;
	float speed = 1;
	CharacterController characterController;

	private void Start() {

		characterController = GetComponent<CharacterController>();

	}


	private void Update() {

		if (input.axis.magnitude > .1f) {
			Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
			characterController.Move(speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up) - new Vector3(0, 9.82f, 0) * Time.deltaTime);
		}

	}


}
