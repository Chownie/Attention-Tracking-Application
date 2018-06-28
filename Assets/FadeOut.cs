using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {
	private CanvasGroup group;

	// Use this for initialization
	void Start () {
		group = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
		if(group.alpha > 0.01) {
			group.alpha -= 0.5f * Time.deltaTime;
		}
	}
}
