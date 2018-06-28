using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Require a button press and then headtracking for progression
// TODO: Log button presses, head movement, target information etc to files

public class GenerateTargets : MonoBehaviour {
	[Header("Generations Settings")]
	public float fakeWidth = 30f;
	public float fakeHeight = 30f;
	public float width = 15f;
	public float height = 8f;
	public float radius = 0.5f;

	[SerializeField]
	private StateMachine.State State;

	public Transform cam;

	public GameObject mainCanvas;
	public GameObject finishedMessage;
	public CanvasGroup misfireMessage;

	// Use this for initialization
	public GameObject prefab;
	public GameObject dummy;

	[Range(1, 50)]
	public int TrialCount;

	public Target.ConditionType Condition;

	private string SessionName;

	[SerializeField]
	private int visitedTargets = 0;

	private List<GameObject> targets = new List<GameObject>();

	[SerializeField]
	private GameObject currentTarget = null;

	[SerializeField]
	private Target focusedTarget = null;

	private double SessionTime;
	private List<int> grabBag = new List<int>(new int[] {
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2
	});

	public void SetHypercolor() {
		Debug.Log("Hypercolor set");
		this.Condition = Target.ConditionType.Hypercolor;
	}

	public void SetDepth() {
		Debug.Log("Depth set");
		this.Condition = Target.ConditionType.Depth;
	}

	public void SetColour() {
		Debug.Log("Colour set");
		this.Condition = Target.ConditionType.Colour;
	}

	public void SetSize() {
		Debug.Log("Size set");
		this.Condition = Target.ConditionType.Size;
	}

	public void SetTransparency() {
		Debug.Log("Transparency set");
		this.Condition = Target.ConditionType.Transparency;
	}

	public void SetName(InputField input) {
		Debug.Log("Participant is" + input.text);
		SessionName = input.text;
	}

	void Awake () {
		SessionTime = Helper.CurrentTime();
		BoxCollider board = gameObject.GetComponent<BoxCollider>();
		board.size = new Vector3(this.fakeWidth, this.fakeHeight, 0.5f);
		board.center = new Vector3(this.fakeWidth/2, this.fakeHeight/2, 0);
	}

	public void Focus(object focus) {
		Target target = (Target)focus;
		if(this.focusedTarget != target && target.gameObject == currentTarget) {
			this.focusedTarget = (Target)focus;
			this.focusedTarget.LockEffect();
			Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "LOCKED");
			StartCoroutine("PrepareNextTarget");
			StateMachine.CurrentState = StateMachine.State.Locked;
		}
	}

	public void FocusPoint(object focal) {
		if(StateMachine.CurrentState != StateMachine.State.Starting) {
			Vector3 pos = (Vector3)focal;
			Helper.Log(SessionName+"_"+Condition.ToString()+"_GAZE", pos.x.ToString(), pos.y.ToString(), cam.eulerAngles.ToString());
		}
	}

	public void Unfocus() {
		if(StateMachine.CurrentState == StateMachine.State.Locked) {
			Misfire(focusedTarget);
		}
		focusedTarget = null;
	}

	void ClearState() {
		StopAllCoroutines();

		StateMachine.CurrentState = StateMachine.State.Starting;

		Random.InitState((int)System.DateTime.Now.Ticks);

		grabBag = new List<int>(new int[] {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2
		});
	}

	void Misfire(Target misfiredTarget) {
		if(StateMachine.CurrentState == StateMachine.State.Finished) {
			return;
		}
		
		misfireMessage.alpha = 1;
		Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "MISFIRED");
		ClearState();

		foreach(GameObject t in targets) {
			Target target = t.GetComponent<Target>();
			target.GetNeighbours(radius);
			target.DisableEffect();
		}

		currentTarget = misfiredTarget.gameObject;
		currentTarget.GetComponent<Target>().EnableEffect();
	}

	private bool range(float min, float max, float input) {
		return (input < max && input > min);
	}

	public void Initialize() {
		Vector3 oldPos = transform.localPosition;
		transform.localPosition = new Vector3(oldPos.x - fakeWidth/2, oldPos.y - fakeHeight/2, oldPos.z);
		Helper.StartTime = Helper.CurrentTime();

		ClearState();
		targets.Clear();

		Random.InitState((int)System.DateTime.Now.Ticks);

		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}

		PoissonDiscSampler sampler = new PoissonDiscSampler(fakeWidth, fakeHeight, radius);
		foreach (Vector2 sample in sampler.Samples()) {
			if(range(fakeWidth/2 - width/2, fakeWidth/2 + width/2, sample.x) && range(fakeHeight/2 - height/2, fakeHeight/2 + height/2, sample.y)) {
				GameObject target = Instantiate(prefab, new Vector3(sample.x, sample.y, 0), Quaternion.identity, transform);
				target.transform.localPosition = new Vector3(sample.x, sample.y, 0);
				targets.Add(target);
			} else {
				GameObject target = Instantiate(dummy, new Vector3(sample.x, sample.y, 0), Quaternion.identity, transform);
				target.transform.localPosition = new Vector3(sample.x, sample.y, 0);
			}
		}

		foreach(GameObject targ in targets) {
			Target target = targ.GetComponent<Target>();
			target.GetNeighbours(radius);
			target.Condition = Condition;
		}

		Collider[] col = Physics.OverlapBox(transform.TransformPoint(new Vector3(width/2, height/2, 0)), new Vector3(1f, 1f, 0), Quaternion.identity, LayerMask.GetMask("Both Eyes"));
		currentTarget = col[Random.Range(0, col.Length)].gameObject;
		currentTarget.GetComponent<Target>().EnableEffect();

		Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "CONDITION", Condition.ToString());

		Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "STARTED");
	}

	bool ButtonPress() {
		return (Input.GetKeyUp("space") || Input.GetKeyUp("joystick button 0"));
	}

	void Update() {
		if(StateMachine.CurrentState == StateMachine.State.Finished) {
			return;
		}

		State = StateMachine.CurrentState;

		if(StateMachine.CurrentState == StateMachine.State.Locked) {
			if(ButtonPress()) {
				if(currentTarget == focusedTarget.gameObject) {
					Misfire(focusedTarget);
				} else {
					focusedTarget.DisableEffect();
					Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "BUTTON PRESSED");
					StateMachine.CurrentState = StateMachine.State.Unlocked;
				}
			}
		}
	}

	void Finished() {
		foreach(Transform child in mainCanvas.transform) {
			child.gameObject.SetActive(false);
		}

		finishedMessage.SetActive(true);
		mainCanvas.SetActive(true);
		mainCanvas.transform.position = cam.transform.position + cam.transform.forward * 5;
		Vector3 direction = mainCanvas.transform.position - cam.transform.position;
     	mainCanvas.transform.rotation = Quaternion.LookRotation(direction);
		foreach (Transform child in transform) {
     		GameObject.Destroy(child.gameObject);
 		}

		GameObject.Destroy(gameObject);
	}

	IEnumerator PrepareNextTarget() {
		int wait = Random.Range(1, 3);
		yield return new WaitForSeconds(wait);
		currentTarget = NextTarget();
		if(currentTarget == null) {
			StateMachine.CurrentState = StateMachine.State.Finished;
			Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "FINISHED");
			Finished();
		} else {
			Target targ = currentTarget.GetComponent<Target>();
			targ.Consume();
			targ.EnableEffect();
		}
	}

	GameObject NextTarget() {
		visitedTargets += 1;
		if(visitedTargets >= TrialCount) {
			return null;
		}
		Target targ = currentTarget.GetComponent<Target>();
		int grabIndex = Random.Range(0, grabBag.Count);
		int grabValue = grabBag[grabIndex];
		grabBag.RemoveAt(grabIndex);
		GameObject newTarget = targ.GetRandomNeighbourOfR(grabValue);
		Helper.Log(SessionName+"_"+Condition.ToString()+"_ACTIONS", "NEW TARGET", grabValue.ToString(), newTarget.transform.position.x.ToString(), newTarget.transform.position.y.ToString());
		return newTarget;
	}
}
