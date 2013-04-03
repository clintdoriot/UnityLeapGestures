using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// This singleton manager exposes Leap events (new frame, found hand, 
/// gestures, etc.) to those classes that register for the information using
/// delegates
/// </summary>
public class LeapManager : MonoBehaviour
{

	// TODO: enable on-the-fly gesture enable / disable (based on registration?)
	
    /*-------------------------------------------------------------------------
     * Singleton Implementation
     * ----------------------------------------------------------------------*/
    private static LeapManager _instance;
    public static LeapManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("LeapManager").AddComponent<LeapManager>();
            }
            return _instance;
        }
    }


    /*-------------------------------------------------------------------------
     * Class Properties & Variables
     * ----------------------------------------------------------------------*/
    // Private Variables
    private static Controller _controller = new Leap.Controller();
    private static Frame _frame = null;

    // Class properitess
    public static Leap.Frame frame
    {
        get
        {
            return _frame;
        }
    }

    // Public properties
    public Vector3 LeapScaling = new Vector3(0.02f, 0.02f, 0.02f);
    public Vector3 LeapOffset = new Vector3(0, 0, 0);

    public bool UseFixedUpdate = false;

    // Leap Event Delegates

    /// <summary>
    /// Delegates for the Leap events to be dispatched.  
    /// </summary>
    public delegate void ObjectLostHandler(int id);
    public delegate void HandFoundHandler(Hand h);
    public delegate void PointableFoundHandler(Pointable p);
    public delegate void HandUpdatedHandler(Hand h);
    public delegate void PointableUpdatedHandler(Pointable p);
	
	public delegate void KeyTapGestureHandler(Gesture g);
	public delegate void ScreenTapGestureHandler(Gesture g);
	
    public delegate void SwipeGestureStartedHandler(Gesture g);
    public delegate void SwipeGestureUpdatedHandler(Gesture g);
    public delegate void SwipeGestureStoppedHandler(Gesture g);
	
    public delegate void CircleGestureStartedHandler(Gesture g);
    public delegate void CircleGestureUpdatedHandler(Gesture g);
    public delegate void CircleGestureStoppedHandler(Gesture g);

    /// <summary>
    /// Events dispatched in the following order:
    ///   HandLost, PointableLost
    ///   HandFound, PointableFound
    ///   HandUpdated, PointableUpdated
    ///   KeyTapGesture, ScreenTapGesture
    ///   SwipeGestureStarted / CircleGestureStarted
    ///   SwipeGestureUpdated / CircleGestureUpdated
    ///   SwipeGestureStopped / CircleGestureStopped
    /// </summary>
    public static event ObjectLostHandler HandLost;
    public static event ObjectLostHandler PointableLost;
    public static event HandFoundHandler HandFound;
    public static event PointableFoundHandler PointableFound;
    public static event HandUpdatedHandler HandUpdated;
    public static event PointableUpdatedHandler PointableUpdated;
	
    public static event KeyTapGestureHandler KeyTapGestureEvent;
    public static event ScreenTapGestureHandler ScreenTapGestureEvent;
	
    public static event SwipeGestureStartedHandler SwipeGestureStartedEvent;
    public static event SwipeGestureUpdatedHandler SwipeGestureUpdatedEvent;
    public static event SwipeGestureStoppedHandler SwipeGestureStoppedEvent;
	
    public static event CircleGestureStartedHandler CircleGestureStartedEvent;
    public static event CircleGestureUpdatedHandler CircleGestureUpdatedEvent;
    public static event CircleGestureStoppedHandler CircleGestureStoppedEvent;

	private Leap.Frame firstFrame = null;
	
    /*-------------------------------------------------------------------------
     * Unity Lifecycle Functions
     * ----------------------------------------------------------------------*/
    public void Awake()
    {
        // Singleton implementation
        if (_instance != null)
        {
            Debug.LogError(this.ToString() + ": Singleton already exists. Destroying.");
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        // Set Leap Unity Extension Properties
        Leap.UnityVectorExtension.InputScale = LeapScaling;
        Leap.UnityVectorExtension.InputOffset = LeapOffset;

        // Set up Leap Controller
        _controller = new Controller();
		_controller.EnableGesture(Leap.Gesture.GestureType.TYPECIRCLE);
		_controller.EnableGesture(Leap.Gesture.GestureType.TYPEKEYTAP);
		_controller.EnableGesture(Leap.Gesture.GestureType.TYPESCREENTAP);
		_controller.EnableGesture(Leap.Gesture.GestureType.TYPESWIPE);
    }

    public void Update()
    {
        if (!UseFixedUpdate)
            UpdateLeap();
    }

    public void FixedUpdate()
    {
        if (UseFixedUpdate)
            UpdateLeap();
    }

    void OnDestroy()
    {
        // Singleton implementation
        _instance = null;
    }


    /*-------------------------------------------------------------------------
     * Leap Event Management Functions
     * ----------------------------------------------------------------------*/
    
	private void UpdateLeap()
    {
        if (_controller != null)
        {

            Frame lastFrame = _frame == null ? Frame.Invalid : _frame;
            _frame = _controller.Frame();
			
            // Only rocess new frames
            if (lastFrame.Id != _frame.Id)
            {
                DispatchLostEvents(frame, lastFrame);
                DispatchFoundEvents(frame, lastFrame);
                DispatchUpdatedEvents(frame, lastFrame);
				DispatchGestureEvents(frame, lastFrame);
            }
        }
    }
	
	
    private static void DispatchLostEvents(Frame newFrame, Frame oldFrame)
    {
		// Hands
        foreach (Hand h in oldFrame.Hands)
        {
            if (!h.IsValid)
                continue;
            if (!newFrame.Hand(h.Id).IsValid && HandLost != null)
                HandLost(h.Id);
        }
		
		// Pointables
        foreach (Pointable p in oldFrame.Pointables)
        {
            if (!p.IsValid)
                continue;
            if (!newFrame.Pointable(p.Id).IsValid && PointableLost != null)
                PointableLost(p.Id);
        }
		
		/*
		// Gestures
		foreach (Gesture g in newFrame.Gestures(_instance.firstFrame))
		{
			if (!g.IsValid)
				continue;
			// TODO: double check logic. Other options: compare old and new frames or check list of recently stopped gestures
			if (g.State == Gesture.GestureState.STATESTOP && GestureStopped != null)
				GestureStopped(g, newFrame.Id);
		}*/
    }
	
	
    private static void DispatchFoundEvents(Frame newFrame, Frame oldFrame)
    {
		// Hands
        foreach (Hand h in newFrame.Hands)
        {
            if (!h.IsValid)
                continue;
            if (!oldFrame.Hand(h.Id).IsValid && HandFound != null)
                HandFound(h);
        }
		
		// Pointables
        foreach (Pointable p in newFrame.Pointables)
        {
            if (!p.IsValid)
                continue;
            if (!oldFrame.Pointable(p.Id).IsValid && PointableFound != null)
                PointableFound(p);
        }
		
		/*
		// Gestures
		foreach (Gesture g in newFrame.Gestures(_instance.firstFrame))
		{
			if (!g.IsValid)
				continue;
			// TODO: double check logic. Other options: compare old and new frames or check list of recently started gestures
			if (g.State == Gesture.GestureState.STATESTART && GestureStarted != null) 
			//if (!oldFrame.Gesture(g.Id).IsValid && GestureStarted != null)
				GestureStarted(g, newFrame.Id);
		}*/
    }

    private static void DispatchUpdatedEvents(Frame newFrame, Frame oldFrame)
    {
		// Hands
        foreach (Hand h in newFrame.Hands)
        {
            if (!h.IsValid)
                continue;
            if (oldFrame.Hand(h.Id).IsValid && HandUpdated != null)
                HandUpdated(h);
        }
		
		// Pointables
        foreach (Pointable p in newFrame.Pointables)
        {
            if (!p.IsValid)
                continue;
			
            if (oldFrame.Pointable(p.Id).IsValid && PointableUpdated != null)
                PointableUpdated(p);
        }
		
		/*
		// Gestures
		foreach (Gesture g in newFrame.Gestures(_instance.firstFrame))
		{
			if (!g.IsValid)
				continue;
			// TODO: double check logic. Other options: see if in new frame or check list of recently started / stopped gestures
			if (g.State == Gesture.GestureState.STATEUPDATE && GestureUpdated != null)
				GestureUpdated(g, newFrame.Id);
		}*/
    }
	
	
	private static void DispatchGestureEvents(Frame newFrame, Frame oldFrame) 
	{
		foreach (Gesture g in newFrame.Gestures(oldFrame))
		{
			// filter invalid events
			if (!g.IsValid || (g.Type == Gesture.GestureType.TYPEINVALID))
				continue;

			// process valid events based on types
			switch (g.Type)
			{
				case Gesture.GestureType.TYPEKEYTAP:
					if (KeyTapGestureEvent != null)
						KeyTapGestureEvent(g);
					break;
				
				case Gesture.GestureType.TYPESCREENTAP:
					if (ScreenTapGestureEvent != null)
						ScreenTapGestureEvent(g);
					break;
				
				case Gesture.GestureType.TYPECIRCLE:
					break;
				
				case Gesture.GestureType.TYPESWIPE:
					break;
				
				default:
					break;
			}
		}
	}

}