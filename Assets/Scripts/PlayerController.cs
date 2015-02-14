using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform centreShotSpawn;
	public Transform leftShotSpawn;
	public Transform rightShotSpawn;
	public Transform farLeftShotSpawn;
	public Transform farRightShotSpawn;
	public float fireRate;

	private int fireLevel;

	private float nextFire;

	void Start ()
	{
		fireLevel = 0;
		nextFire = 0.0f;
	}

	void Update ()
	{
		if ((Input.GetButton ("Fire1") || Input.GetKey ("space")) && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Fire ();
			audio.Play ();
		}
	}

	void Fire ()
	{
		switch (fireLevel) {
		case 0:
			Instantiate (shot, centreShotSpawn.position, centreShotSpawn.rotation);
			break;
		case 1:
			Instantiate (shot, leftShotSpawn.position, leftShotSpawn.rotation);
			Instantiate (shot, rightShotSpawn.position, rightShotSpawn.rotation);
			break;
		case 2:
			Instantiate (shot, centreShotSpawn.position, centreShotSpawn.rotation);
			Instantiate (shot, farLeftShotSpawn.position, farLeftShotSpawn.rotation);
			Instantiate (shot, farRightShotSpawn.position, farRightShotSpawn.rotation);
			break;
		}
	}

	public void increaseFireLevel ()
	{
		if (fireLevel < 2) {
			fireLevel += 1;
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.velocity = movement * speed;

		rigidbody.position = new Vector3 
		(
			Mathf.Clamp (rigidbody.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rigidbody.position.z, boundary.zMin, boundary.zMax)
		);

		rigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, rigidbody.velocity.x * -tilt);
	}
}
