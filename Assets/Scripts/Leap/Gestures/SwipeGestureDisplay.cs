using UnityEngine;
using System.Collections;
using Leap;

public class SwipeGestureDisplay : GestureDisplay {
	
	private SwipeGesture _swipeGesture;

	public SwipeGesture swipeGesture
	{
		get
		{
			return _swipeGesture;
		}
		
		set
		{
			_swipeGesture = value;
			gesture = value;
		}
	}
	
	public override void Start()
	{
		if (swipeGesture == null)
			Destroy (this);
	}
	
	public override void Update()
	{
		base.Update();
		
		if (swipeGesture == null)
			Destroy(gameObject);
		
		Vector3 startPosition = swipeGesture.StartPosition.ToUnityTranslated();
		Vector3 endPosition = swipeGesture.Position.ToUnityTranslated();
		Vector3 diff = endPosition - startPosition;
		
		transform.localScale = new Vector3(diff.magnitude, 1, 1);
		transform.position = diff/2;
		
			
		//transform.position = circleGesture.Center.ToUnityTranslated();
		//transform.localScale = circleGesture.Radius * LeapManager.instance.LeapScaling * 2;
	}
}