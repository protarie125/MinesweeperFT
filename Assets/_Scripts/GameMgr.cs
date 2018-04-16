using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour {
	public GameObject levelSelection;
	public bool gameStarted;
	public bool gameEnded;

	public GameObject startNewGameMenu;
	public Text endGameText;

	public GameObject uiZone;
	public Text txtTiles;
	public Text txtMines;

	public int width;
	public int height;
	public int nTiles;
	public int nMines;
	public int remainTiles;
	public int nFlags;

	public GameTile prfTile;
	public float tileSize;

	public List<List<GameTile>> gameTiles = new List<List<GameTile>> ();
	public GameObject tileHolder;

	// Use this for initialization
	void Awake () {
		Screen.SetResolution (1600, 900, false);
		Init ();
	}

	void Update () {
		if (gameStarted) {
			// check the number of remain Game Tiles & flags
			int count = 0;
			nFlags = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (!gameTiles [y] [x].isOpen) {
						count += 1;
					}
					if (gameTiles [y] [x].isChecked) {
						nFlags += 1;
					}
				}
			}
			remainTiles = count;

			// UI
			UpdateUI ();

			// win game
			if (!gameEnded && remainTiles == nMines) {
				WinGame ();
			}
		}
	}

	void Init () {
		levelSelection.SetActive (true);
		gameStarted = false;
		gameEnded = false;
		uiZone.SetActive (false);
		startNewGameMenu.SetActive (false);
	}

	void UpdateUI () {
		txtTiles.text = "Tile: " + remainTiles.ToString () + " / " + nTiles.ToString();
		txtMines.text = "Mine: " + (nMines - nFlags).ToString ();
	}

	void SetupTiles () {
		// Tile holder
		tileHolder = new GameObject ();
		tileHolder.name = "Tiles";
		// make the List of tiles
		gameTiles.Clear ();
		for (int y = 0; y < height; y++) {
			List<GameTile> tileRow = new List<GameTile> ();
			tileRow.Clear ();
			for (int x = 0; x < width; x++) {
				GameTile temp = Instantiate (prfTile) as GameTile;
				temp.transform.SetParent (tileHolder.transform);
				temp.transform.localScale = new Vector3 (tileSize, tileSize, 1.0f);
				temp.gameMgr = this;
				temp.posX = x;
				temp.posY = y;
				tileRow.Add (temp);
			}
			gameTiles.Add (tileRow);
		}
		// set the position of tiles
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				float vx = (x - (int)(width / 2) + 0.5f * ((width + 1) % 2)) * tileSize;
				float vy = (y - (int)(height / 2) + 0.5f * ((height + 1) % 2)) * tileSize;
				gameTiles [y] [x].transform.position = new Vector2 (vx, vy);
			}
		}
	}

	public void MakeBoard () {
		Debug.Log ("The Game Manager is making a board...");
		// set Tiles
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				gameTiles [y] [x].Init ();
			}
		}
		// set mines
		for (int n = 0; n < nMines; n++) {
			bool mineSet = false;
			while (!mineSet) {
				int rx = Random.Range (0, width);
				int ry = Random.Range (0, height);
				if (!gameTiles [ry] [rx].isMine) {
					gameTiles [ry] [rx].isMine = true;
					gameTiles [ry] [rx].botRD.color = gameTiles [ry] [rx].botMine;
					mineSet = true;
				}
			}
		}
		// set numbers
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int count = 0;
				if (x - 1 >= 0) {
					if (y - 1 >= 0) {
						if (gameTiles [y - 1] [x - 1].isMine) // topleft
							count += 1;
					}
					if (gameTiles [y] [x - 1].isMine) // left
						count += 1;
					if (y + 1 < height) {
						if (gameTiles [y + 1] [x - 1].isMine) // bottomleft
							count += 1;
					}
				}
				if (y - 1 >= 0) {
					if (gameTiles [y - 1] [x].isMine) // top
						count += 1;
				}
				if (y + 1 < height) {
					if (gameTiles [y + 1] [x].isMine) // bottom
						count += 1;
				}
				if (x + 1 < width) {
					if (y - 1 >= 0) {
						if (gameTiles [y - 1] [x + 1].isMine) // topright
							count += 1;
					}
					if (gameTiles [y] [x + 1].isMine) // right
						count += 1;
					if (y + 1 < height) {
						if (gameTiles [y + 1] [x + 1].isMine) // bottomright
							count += 1;
					}
				}
				gameTiles [y] [x].number = count;
				gameTiles [y] [x].SetText ();
			}
		}
	}

	// start menu buttons
	void CommonGame () {
		gameStarted = false;
		nTiles = width * height;
		remainTiles = nTiles;
		nFlags = 0;
		SetupTiles ();
		levelSelection.SetActive (false);
		uiZone.SetActive (true);
		UpdateUI ();
	}

	public void MakeEasyGame () {
		width = 12;
		height = 12;
		tileSize = 0.72f;
		nMines = 15;
		CommonGame ();
	}

	public void MakeNormalGame () {
		width = 19;
		height = 13;
		tileSize = 0.64f;
		nMines = 30;
		CommonGame ();
	}

	public void MakeHardGame () {
		width = 26;
		height = 15;
		tileSize = 0.56f;
		nMines = 60;
		CommonGame ();
	}

	public void QuitGame () {
		Application.Quit ();
	}

	// end game part
	public void LoseGame () {
		Debug.Log ("You Lose...");
		endGameText.text = "You Lose...";
		gameEnded = true;
		// opne all mines
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (gameTiles [y] [x].isMine) {
					gameTiles [y] [x].mineFlag.SetActive (false);
					gameTiles [y] [x].top.SetActive (false);
				}
			}
		}
		// show Start New Game Menu
		startNewGameMenu.SetActive (true);
	}

	void WinGame () {
		Debug.Log ("You Win!");
		endGameText.text = "You Win!";
		gameEnded = true;
		// check all mines
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (gameTiles [y] [x].isMine) {
					gameTiles [y] [x].mineFlag.SetActive (true);
				}
			}
		}
		// show Start New Game Menu
		startNewGameMenu.SetActive (true);
	}

	// button action
	public void StartNewGame () {
		// destroy and claer the Game Tiles
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				Destroy (gameTiles [y] [x].gameObject);
			}
			gameTiles [y].Clear ();
		}
		gameTiles.Clear ();
		Destroy (tileHolder);
		// initialize
		Init ();
	}
}
