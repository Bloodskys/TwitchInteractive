using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        TwitchListener.Instance.AddListener("!endless", 3, CommandTrigger, true);
        TwitchListener.Instance.AddListener("!test", 5, CommandTrigger, false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CommandTrigger()
    {
        Debug.Log("TRIGGERED!");
    }
}
