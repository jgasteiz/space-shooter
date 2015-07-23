using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyByContact : MonoBehaviour
{

	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

	private GameController gameController;
	private List<string> tagsToIgnore;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}

		tagsToIgnore = new List<string>() {"Boundary", "EnemyBolt", "EnemyShip"};
	}

	void OnTriggerEnter (Collider other)
	{
		// Ignore collissions with ignored tags.
		if (tagsToIgnore.Contains(other.tag)) {
			return;
		}

		// Explosion!
		Instantiate (explosion, transform.position, transform.rotation);

		// Player explosion
		if (other.tag == "Player") {
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
			gameController.PlayerDies ();
		}

		gameController.AddScore (scoreValue);

		// Destroy the game object - bolt or player
		Destroy (other.gameObject);

		// Destroy the game object itself
		Destroy (gameObject);
	}
}
