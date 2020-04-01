using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class PlayerMovement: MonoBehaviour{
	private float speed = 3.0f;
	private float turnSpeed = 80f; 
	public GameObject global; 

	void Update() {
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		transform.position += move * speed * Time.deltaTime;

		if (move != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation (Vector3.forward, move);
		}
	}
}