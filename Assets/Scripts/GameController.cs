using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	public GameObject hazard;
	public GameObject enemyShip;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;

	private bool gameOver;
	private bool restart;
	private int score;

	void Start ()
	{
		gameOver = false;
		restart = false;

		restartText.text = "";
		gameOverText.text = "";

		score = 0;
		UpdateScore ();
		StartCoroutine (SpawnWaves ());
	}

	void Update ()
	{
		if (restart) {
			if (Input.GetKeyDown (KeyCode.R)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	IEnumerator SpawnWaves ()
	{
		// Wait a bit before starting the waves
		yield return new WaitForSeconds (startWait);

		while (true) {
			for (int i = 0; i < hazardCount; i++) {
				float valueX = Random.Range (-spawnValues.x, spawnValues.x);
				Vector3 spawnPosition = new Vector3 (valueX, spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}

			spawnVader ();
			yield return new WaitForSeconds (spawnWait);

			yield return new WaitForSeconds (waveWait);

			if (gameOver) {
				restartText.text = "Press 'R' for Restart";
				restart = true;
				break;
			}
		}
	}

	private void spawnVader ()
	{
		float valueX = Random.Range (-spawnValues.x, spawnValues.x);
		Vector3 spawnPosition = new Vector3 (valueX, spawnValues.y, spawnValues.z);
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate (enemyShip, spawnPosition, spawnRotation);
	}
	
	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

	public void GameOver ()
	{
		gameOverText.text = "Game Over";
		gameOver = true;
	}
}
