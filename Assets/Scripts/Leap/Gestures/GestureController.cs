using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
	
	public GameObject keyTapGesturePrefab;
	
	// Use this for initialization
	void Start () {
		LeapManager.KeyTapGestureEvent += new LeapManager.KeyTapGestureHandler(OnKeyTapGesture);
	    //LeapManager.GestureStarted += new LeapManager.GestureStartedHandler(OnGestureStarted);
        //LeapManager.GestureUpdated += new LeapManager.GestureUpdatedHandler(OnGestureUpdated);
        //LeapManager.GestureStopped += new LeapManager.GestureStoppedHandler(OnGestureStopped);
	}
	

	public void OnKeyTapGesture(Gesture g) {
		GameObject go = (GameObject) GameObject.Instantiate(keyTapGesturePrefab);
		KeyTapGestureDisplay keyTap = go.GetComponent<KeyTapGestureDisplay>();
		keyTap.gesture = g;
		Debug.Log("OnKeyTapGesture " + g.Id);
	}
	
	public void OnScreenTapGesture(Gesture g) {
	}
	
	void OnGestureStarted(Gesture g, long fId) {
		Debug.LogWarning("" + fId + "->" + g.Frame.Id + " " +g.Type.ToString() + " Gesture Started: " + g.Id + " " + g.State.ToString() );
	}
	
	
	void OnGestureUpdated(Gesture g, long fId) {
		Debug.Log("" + fId + "->" + g.Frame.Id + " " +g.Type.ToString() + " Gesture Updated: " + g.Id + " " + g.State.ToString());
	}
	
	
	void OnGestureStopped(Gesture g, long fId) {
		Debug.LogError("" + fId + "->" + g.Frame.Id + " " +g.Type.ToString() + " Gesture Stopped: " + g.Id + " " + g.State.ToString());
	}
}
