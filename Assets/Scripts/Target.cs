using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Target : MonoBehaviour {
	private List<GameObject> neighbours = new List<GameObject>();
	private List<GameObject> inactiveNeighbours = new List<GameObject>();
	public Material DefaultMaterial;
	public Material LockedMaterial;
	public Material ColourMaterial;

	public enum ConditionType{Hypercolor, Depth, Size, Transparency, Colour};

	public ConditionType Condition;
	
	[SerializeField]
	private GameObject Left;
	[SerializeField]
	private GameObject Right;

	[SerializeField]
	private Renderer LeftRenderer;
	[SerializeField]
	private Renderer RightRenderer;

	[Header("HyperColor Settings")]
	public Material LeftMaterial;
	public Material RightMaterial;

	[Header("Transparency Settings")]
	[Range(0, 1)]
	public float LeftOpacity;
	[Range(0, 1)]
	public float RightOpacity;

	/*
	[Header("Depth Settings")]
	[Range(-1, 1)]
	public float LeftDepth;
	[Range(-1, 1)]
	public float RightDepth;
	 */

	[Header("Size Settings")]
	[Range(1, 3)]
	public float LeftScale;
	[Range(1, 3)]
	public float RightScale;
	
	public void OnGazeLock() {
		SendMessageUpwards("Focus", this);
	}

	public void OnGazeExit() {
		SendMessageUpwards("Unfocus");
	}

	void Update() {
		foreach(GameObject node in neighbours) {
			Debug.DrawLine(transform.position, node.transform.position, Color.green, 0.1f);
		}
	}

	public void Consume() {
		foreach(GameObject node in neighbours) {
			Target target = node.GetComponent<Target>();
			target.DeactivateNeighbour(this.gameObject);
		}
		foreach(GameObject node in inactiveNeighbours) {
			Target target = node.GetComponent<Target>();
			target.DeactivateNeighbour(this.gameObject);
		}
	}

	public void DeactivateNeighbour(GameObject neighbour) {
		if(neighbours.Contains(neighbour)) {
			neighbours.Remove(neighbour);
			inactiveNeighbours.Add(neighbour);
		}
	}

	public void GetNeighbours(float SearchDistance) {
		neighbours = new List<GameObject>();
		inactiveNeighbours = new List<GameObject>();
		Collider[] col = Physics.OverlapSphere(transform.position, SearchDistance * 1.5f, LayerMask.GetMask("Both Eyes"));
		foreach(Collider target in col) {
			if(target.gameObject != gameObject) {
				neighbours.Add(target.gameObject);
			}
		}
	}

	public GameObject ConsumeRandomNeighbour() {
		GameObject neighbour = GetRandomNeighbour();
		if(neighbours.Contains(neighbour)) {
			neighbours.Remove(neighbour);
			inactiveNeighbours.Add(neighbour);
		}
		return neighbour;
	}

	public GameObject GetRandomNeighbourOfR(int Distance) {
		if(Distance == 0) {
			return ConsumeRandomNeighbour();
		} else {
			return GetRandomNeighbour().GetComponent<Target>().GetRandomNeighbourOfR(Distance - 1);
		}
	}

	public GameObject GetRandomNeighbour() {
		if(neighbours.Count > 0) {
			int selection = Random.Range(0, neighbours.Count);
			return neighbours[selection];
		} else {
			int selection = Random.Range(0, inactiveNeighbours.Count);
			return inactiveNeighbours[selection];
		}
	}

	public void LockEffect() {
		LeftRenderer.material = LockedMaterial;
		RightRenderer.material = LockedMaterial;
	}

	private void EnableHypercolor() {
		LeftRenderer.material = LeftMaterial;
		RightRenderer.material = RightMaterial;
	}

	private void EnableDepth() {
		/*LeftRenderer.sortingOrder = (int)LeftDepth;
		RightRenderer.sortingOrder = (int)RightDepth;
		Left.transform.localPosition += new Vector3(0, 0, LeftDepth);
		Right.transform.localPosition += new Vector3(0, 0, RightDepth);*/
	}

	private void EnableColour() {
		LeftRenderer.material = ColourMaterial;
		RightRenderer.material = ColourMaterial;
	}

	private void EnableTransparency() {
		Color leftCol = LeftRenderer.material.color;
		Color rightCol = RightRenderer.material.color;

		leftCol.a = LeftOpacity;
		rightCol.a = RightOpacity;

		LeftRenderer.material.color = leftCol;
		RightRenderer.material.color = rightCol;
	}

	private void EnableSize() {
		Left.transform.localScale = new Vector3(LeftScale, LeftScale, 0);
		Right.transform.localScale = new Vector3(RightScale, RightScale, 0);
	}

	public void EnableEffect() {
		switch(Condition) {
			case ConditionType.Hypercolor:
				EnableHypercolor();
				break;
			case ConditionType.Depth:
				EnableDepth();
				break;
			case ConditionType.Transparency:
				EnableTransparency();
				break;
			case ConditionType.Size:
				EnableSize();
				break;
			case ConditionType.Colour:
				EnableColour();
				break;
		}
	}

	public void DisableEffect() {
		LeftRenderer.material = DefaultMaterial;
		RightRenderer.material = DefaultMaterial;

		Left.transform.localPosition = new Vector3(Left.transform.localPosition.x, Left.transform.localPosition.y, 0);
		Right.transform.localPosition = new Vector3(Right.transform.localPosition.x, Right.transform.localPosition.y, 0);

		Left.transform.localScale = new Vector3(2, 2, 0);
		Right.transform.localScale = new Vector3(2, 2, 0);
	}
}
