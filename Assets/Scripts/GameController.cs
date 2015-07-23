using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

	public GameObject hazard;
	public GameObject enemyShip;
	public GameObject playerShip;
	
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int hazardCount;
	public int maxHazardCount;
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
	IEnumerator mainLoop ()
	{

		List<GameObject> enemyShips;
		while (true) {

			SetTitle ("Wave " + waveNum);
			yield return new WaitForSeconds (startWait);
			SetTitle ("");

			enemyShips = new List<GameObject>();
			for (int i = 0; i < hazardCount; i++) {
				// If the player is dead, exit the loop.
				if (playerDead) {
					break;
				}
				enemyShips.Add(SpawnEnemy ());
				yield return new WaitForSeconds (spawnWait);
			}

			// Wait until all enemies are dead
			while (areEnemiesDead(enemyShips) == false) {
				yield return new WaitForSeconds (spawnWait);
			}

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
	 * Given a list of enemies, return true if all of
	 * them are dead - the objects are null
	 */
	bool areEnemiesDead(List<GameObject> enemies) {
		bool allEnemiesDead = true;
		foreach (GameObject enemy in enemies) {
			if (enemy != null) {
				allEnemiesDead = false;
			}
		}
		return allEnemiesDead;
	}

	/**
	 * Given a hazard count and a number of wave, calculate a new
	 * hazard count.
	 */
	int increaseHazardCount (int currentHazardCount, int waveNum) {
		int newHazardCount = currentHazardCount;

		if (currentHazardCount < maxHazardCount) {
			newHazardCount += currentHazardCount * waveNum / 10;
		}

		return Mathf.Min(newHazardCount, maxHazardCount);
	}

	/**
	 * Given a spawn wait, decrease it and return the new one.
	 */
	float decreaseSpawnWait (float currentSpawnWait) {
		float newSpawnWait = currentSpawnWait;
		
		if (currentSpawnWait > 0.25f) {
			newSpawnWait -= currentSpawnWait / 4;
		}
		
		return Mathf.Max(0.25f, newSpawnWait);
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
		StartCoroutine (mainLoop ());
	}

	/**
	 * Spawns an enemy.
	 */
	GameObject SpawnEnemy ()
	{
		float valueX = Random.Range (-spawnValues.x, spawnValues.x);
		Vector3 spawnPosition = new Vector3 (valueX, spawnValues.y, spawnValues.z);
		return (GameObject) Instantiate (enemyShip, spawnPosition, Quaternion.Euler (0.0f, 180.0f, 0.0f));
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
