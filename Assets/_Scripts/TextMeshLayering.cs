using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshLayering : MonoBehaviour {
	public Renderer rd;
	public Transform parent;
	public Renderer parentRD;

	void Awake () {
		parentRD = parent.GetComponent<Renderer> ();

		rd.sortingLayerID = parentRD.sortingLayerID;
		rd.sortingLayerName = parentRD.sortingLayerName;
		rd.sortingOrder = parentRD.sortingOrder;
	}
}
