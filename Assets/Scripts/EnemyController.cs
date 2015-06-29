using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public GameObject shot;
	public Transform centreShotSpawn;
	public float fireRate;

	private float nextFire;

	// Use this for initialization
	void Start () {
		nextFire = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Fire ();
		}
	}

	void Fire ()
	{
		Instantiate (shot, centreShotSpawn.position, centreShotSpawn.rotation);
	}
}
