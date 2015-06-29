using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour {
	
	public float speed;
	public Boundary boundary;

	private int direction;
	
	void Start () {
		GetComponent<Rigidbody>().velocity = transform.forward * speed;

		// Get a random direction - 0 or 1
		direction = Random.Range(0, 2);
	}
	
	void FixedUpdate () {
		// 0 or 1 translated to left and right
		float x;
		if (direction == 0) {
			x = -1.0f;
		} else {
			x = 1.0f;
		}

		Vector3 movement = new Vector3 (x, 0.0f, transform.forward.z);
		GetComponent<Rigidbody>().velocity = movement * speed;
		
		GetComponent<Rigidbody>().position = new Vector3 
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);

		// If it hits a boundary, change direction
		if (direction == 1 && GetComponent<Rigidbody> ().position.x >= boundary.xMax) {
			direction = 0;
		} else if (direction == 0 && GetComponent<Rigidbody> ().position.x <= boundary.xMin) {
			direction = 1;
		}
	}
}
