using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyByContactIndestructible : MonoBehaviour
{
	private List<string> tagsToDestroy;
	
	void Start ()
	{
		tagsToDestroy = new List<string>() {"EnemyBolt", "EnemyShip"};
	}
	
	void OnTriggerEnter (Collider other)
	{
		
		// Destroy the game object - bolt or player
		if (tagsToDestroy.Contains (other.tag)) {
			Destroy (other.gameObject);
		}
	}
}
