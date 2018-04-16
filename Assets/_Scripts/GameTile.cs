using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
	public GameMgr gameMgr;
	public int posX;
	public int posY;

	public GameObject top;
	public SpriteRenderer topRD;
	public Color32 topExit;
	public Color32 topOver;

	public GameObject textHolder;
	public TextMesh tm;

	public GameObject bottom;
	public SpriteRenderer botRD;
	public Color32 botDef;
	public Color32 botMine;

	public GameObject mineFlag;
	public bool isChecked;

	public bool isMine;
	public int number;
	public bool isOpen;

	void Awake () {
		Init ();
	}

	public void Init () {
		isMine = false;
		number = 0;
		isOpen = false;

		isChecked = false;

		topRD = top.GetComponent<SpriteRenderer> ();
		tm = textHolder.GetComponentInChildren<TextMesh> ();

		botRD = bottom.GetComponent<SpriteRenderer> ();
		botRD.color = botDef;

		SetText ();
	}

	public void SetText () {
		if (isMine) {
			tm.text = "!";
		} else {
			if (number == 0) {
				tm.text = " ";
			} else {
				tm.text = number.ToString ();
			}
		}
	}

	public void Open () {
		if (!isOpen) {
			top.SetActive (false);
			isOpen = true;
		}
	}

	void toggleFlag () {
		if (!isChecked) {
			mineFlag.SetActive (true);
			isChecked = true;
		} else {
			mineFlag.SetActive (false);
			isChecked = false;
		}
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (!gameMgr.gameEnded) {
			topRD.color = topOver;
		}
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		if (!gameMgr.gameEnded) {
			topRD.color = topExit;
		}
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (!gameMgr.gameEnded) {
			if (eventData.button == PointerEventData.InputButton.Left) {			
				while (!gameMgr.gameStarted) {
					gameMgr.MakeBoard ();
					if (!isMine) {
						gameMgr.gameStarted = true;
					}
				}

				if (!isChecked) {
					Open ();

					if (number == 0 && !isMine) {
						OpenArounds ();
					}

					if (isMine) {
						gameMgr.LoseGame ();
					}
				}
			}

			if (eventData.button == PointerEventData.InputButton.Right) {
				if (!isOpen) {
					toggleFlag ();
				}
			}
		}
	}

	#endregion

	void OpenArounds () {
		if (posX - 1 >= 0) {
			if (posY - 1 >= 0)
				gameMgr.gameTiles [posY - 1] [posX - 1].Open ();

			gameMgr.gameTiles [posY] [posX - 1].Open ();

			if (posY + 1 < gameMgr.height)
				gameMgr.gameTiles [posY + 1] [posX - 1].Open ();
		}

		if (posY - 1 >= 0)
			gameMgr.gameTiles [posY - 1] [posX].Open ();

		if (posY + 1 < gameMgr.height)
			gameMgr.gameTiles [posY + 1] [posX].Open ();

		if (posX + 1 < gameMgr.width) {
			if (posY - 1 >= 0)
				gameMgr.gameTiles [posY - 1] [posX + 1].Open ();

			gameMgr.gameTiles [posY] [posX + 1].Open ();

			if (posY + 1 < gameMgr.height)
				gameMgr.gameTiles [posY + 1] [posX + 1].Open ();
		}
	}
}
