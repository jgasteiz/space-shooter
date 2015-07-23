using UnityEngine;
using System.Collections;

public class DestroyPlayerByContact : MonoBehaviour
{
	
	public GameObject playerExplosion;
	private GameController gameController;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}	
	}

	void OnTriggerEnter (Collider other)
	{
		// Ignore collissions with boundary
		if (other.tag == "Boundary") {
			return;
		}

		// Player explosion
		if (other.tag == "Player") {
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
			gameController.PlayerDies ();

			// Destroy the game object - bolt or player
			Destroy (other.gameObject);
			
			// Destroy the game object itself
			Destroy (gameObject);
		}
	}
}
