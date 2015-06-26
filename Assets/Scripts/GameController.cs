using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	public GameObject hazard;
	public GameObject enemyShip;
	public GameObject playerShip;
	
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int hazardCount;
	public int extraLives;
	public Vector3 spawnValues;

	public GUIText scoreText;
	public GUIText instructionsText;
	public GUIText titleText;
	public GUIText extraLivesText;

	private bool gameOver;
	private bool playerDead;
	private bool restart;
	private bool started;
	private int score;
	private int waveNum;

	private string TITLE = "SPACE\nSHOOTER";
	private string INSTRUCTIONS = "Moving: WASD\nFire: SPACEBAR\n\nPress SPACEBAR to begin";
	private string RESTART_INSTRUCTIONS = "Press 'R' to Restart";
	private string GAME_OVER = "GAME OVER";

	private int ASTEROID = 0;
	private int SHIP = 1;
	private KeyCode RESTART_KEYCODE = KeyCode.R;
	private KeyCode START_KEYCODE = KeyCode.Space;

	private PlayerController playerController;

	void Start ()
	{
		gameOver = false;
		playerDead = false;
		restart = false;
		started = false;
		score = 0;
		waveNum = 1;

		SetTitle (TITLE);
		SetInstructions (INSTRUCTIONS);
	}

	void Update ()
	{
		// If the game hasn't started yet, listen for spacebar press to being
		if (!started) {
			if (Input.GetKeyDown (START_KEYCODE)) {
				StartGame ();
			}
		}
		// If we're in restart listening mode, wait until
		// user presses R to restart.
		if (restart) {
			if (Input.GetKeyDown (RESTART_KEYCODE)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	/**
	 * This method will spawn waves of enemies after an
	 * initial wait time.
	 */
	IEnumerator SpawnWaves ()
	{
		// Wait a bit before starting the waves
		yield return new WaitForSeconds (startWait);

		while (true) {
			SetTitle ("Wave " + waveNum);
			yield return new WaitForSeconds (startWait);
			SetTitle ("");

			SpawnEnemy (SHIP);
			yield return new WaitForSeconds (spawnWait);

			yield return new WaitForSeconds (waveWait);

			// Increase wave num
			waveNum += 1;

			// Increase player's fire level
			playerController.increaseFireLevel ();

			// Change hazard count based on the current hazard count and wave num.
			hazardCount = increaseHazardCount(hazardCount, waveNum);

			// Change spawn wait based on the current spawn wait.
			spawnWait = decreaseSpawnWait(spawnWait);

			// If game over, do stuff
			if (gameOver) {
				SetInstructions (RESTART_INSTRUCTIONS);
				restart = true;
				break;
			} else if (playerDead) {
				SpawnPlayer ();
			}
		}
	}

	/**
	 * Given a hazard count and a number of wave, calculate a new
	 * hazard count.
	 */
	int increaseHazardCount (int currentHazardCount, int waveNum) {
		int newHazardCount = currentHazardCount;

		if (currentHazardCount < 60) {
			newHazardCount += currentHazardCount * waveNum / 10;
		}
		if (newHazardCount > 60) {
			newHazardCount = 60;
		}

		return newHazardCount;
	}

	/**
	 * Given a spawn wait, decrease it and return the new one.
	 */
	float decreaseSpawnWait (float currentSpawnWait) {
		float newSpawnWait = currentSpawnWait;
		
		if (currentSpawnWait > 0.25f) {
			newSpawnWait -= currentSpawnWait / 4;
		}
		if (newSpawnWait < 0.25f) {
			newSpawnWait = 0.25f;
		}
		
		return newSpawnWait;
	}

	/**
	 * Sets the game as started, sets the GUI texts to their
	 * initial values and spawns player and waves.
	 * 
	 */
	void StartGame ()
	{
		started = true;
		
		SetTitle ("");
		SetInstructions ("");
		UpdateScore ();
		UpdateExtraLives ();
		
		SpawnPlayer ();
		StartCoroutine (SpawnWaves ());
	}

	/**
	 * Given a type, spawns an enemy.
	 */
	void SpawnEnemy (int type)
	{
		float valueX = Random.Range (-spawnValues.x, spawnValues.x);
		Vector3 spawnPosition = new Vector3 (valueX, spawnValues.y, spawnValues.z);
		Quaternion spawnRotation = Quaternion.identity;
		if (type == ASTEROID) {
			Instantiate (hazard, spawnPosition, spawnRotation);
		} else if (type == SHIP) {
			Instantiate (enemyShip, spawnPosition, spawnRotation);
		}
	}

	void ShowTitleWave ()
	{

	}

	/**
	 * Update the title GuiText with the given title.
	 */
	void SetTitle (string title)
	{
		titleText.text = title;
	}

	/**
	 * Update the instructions GuiText with the given instructions.
	 */
	void SetInstructions (string instructions)
	{
		instructionsText.text = instructions;
	}

	/**
	 * Update the score GuiText with the current score.
	 */
	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

	/**
	 * Update the lives GuiText with the remaining lives.
	 */
	void UpdateExtraLives ()
	{
		extraLivesText.text = "Lives: " + extraLives;
	}

	/**
	 * Spawns the player ship in the origin.
	 */
	void SpawnPlayer ()
	{
		Vector3 spawnPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate (playerShip, spawnPosition, spawnRotation);
		playerDead = false;

		GameObject playerControllerObject = GameObject.FindWithTag ("Player");
		if (playerControllerObject != null) {
			playerController = playerControllerObject.GetComponent <PlayerController> ();
		}
		if (playerController == null) {
			Debug.Log ("Cannot find 'playerController' script");
		}
	}

	/**
	 * Update the score GuiText with the current score.
	 */
	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

	/**
	 * Removes a life from the remaining player lives and
	 * decides whether the game is over ot the player
	 * should respawn.
	 */
	public void PlayerDies ()
	{
		extraLives = extraLives - 1;
		if (extraLives < 0) {
			SetTitle (GAME_OVER);
			gameOver = true;
		} else {
			UpdateExtraLives ();
			playerDead = true;
		}
	}
}
