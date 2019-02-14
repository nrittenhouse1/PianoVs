﻿///Author: Adam Warkentin
///This script is attatched to a "Key" which only has a public method that plays the sound clip of the attatched audio source
///Last Modified By: Noah Rittenhouse
///Last Modified Date: 5-Feb-2019
///Dependencies: Requires an audio source component.If the key is a white key, check the "whiteKey" bool in the editor

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualKeyScript : MonoBehaviour
{
	float timeStamp;
	float heldNotedistance = 0;
	bool stamp;

	public bool AI;
	int AITimer;

	//keymodel is the visual component of this obj
	GameObject keyModel;

    //keyModelRenderer is just the rendered component of keyModel
    Renderer keyModelRenderer;

    //These are the materials for how the key should look when up and down
    Material keyUp;
    Material keyDown;

    //Variable that is set in the editor to determine if the key which this script is attatched to is a whitekey or a black key
    public bool whiteKey;


    RaycastHit hit;//The raycast collision data
    Vector3 keyPos;//The position of the key, just short form for the transform.position because I am lazy
    bool holdingNote;//Variable for looping the code involving held notes
    GameObject heldNote;//Variable to store the held notes gameobject so it can be destroyed after use
    float holdingTimer;

    void Start()
    {
        keyPos = transform.position;//Initialize transform.position of the object with this script
        holdingNote = false;
        holdingTimer = 1.0f;

        keyModel = transform.GetChild(0).gameObject;
        keyModelRenderer = keyModel.GetComponent<Renderer>();

        //Gets the necessary material for the key from the Resources folder
        if (whiteKey)
        {
            keyUp = Resources.Load("Key-Up") as Material;
            keyDown = Resources.Load("Key-Down") as Material;
        }
        else
        {
            keyUp = Resources.Load("KeySharp-Up") as Material;
            keyDown = Resources.Load("KeySharp-Down") as Material;
        }

        keyModelRenderer.material = keyUp;
    }

    public void Update()
    {
		if(!whiteKey)
		Debug.DrawRay(keyPos + new Vector3(0, 1.5f, 0), Vector3.up);
		if(AI)
		{
			if(whiteKey)
			{
				if (Physics.Raycast(keyPos + new Vector3(0, 2.5f, 0), Vector3.up, out hit, .5f) && hit.collider.gameObject.tag == "Note")//Shoots raycast from the tip of note
				{
					StopNote();
					PlayNote();
					AITimer = 0;
				}
				else
				{
					if (!Physics.Raycast(keyPos + new Vector3(0, 2.5f, 0), Vector3.up, out hit, .6f) && AITimer > 5)//Shoots raycast from the tip of note
					{
						StopNote();
					}
					else
					{
						holdingNote = true;
					}
				}
				AITimer++;
			}
			else
			{
				if (Physics.Raycast(keyPos + new Vector3(0, 1.5f, 0), Vector3.up, out hit, .7f) && hit.collider.gameObject.tag == "Note")//Shoots raycast from the tip of note
				{
					StopNote();
					PlayNote();
					AITimer = 0;
				}
				else
				{
					if (!Physics.Raycast(keyPos + new Vector3(0, 1.5f, 0), Vector3.up, out hit, .6f) && AITimer > 5)//Shoots raycast from the tip of note
					{
						StopNote();
					}
					else
					{
						holdingNote = true;
					}
				}
				AITimer++;
			}
		}
		if (stamp)
		{
			TimeStamp();
		}
		if (holdingNote)//If the holding note has been hit
		{
			if (whiteKey)
			{
				if (Physics.Raycast(keyPos + new Vector3(0, 2.5f, 0), Vector3.up, out hit, heldNotedistance + 1f))//Shoots raycast from the tip of note
				{
					if (hit.collider.tag == "HeldNote")//If the raycast hits a note that must be held...
					{
						Debug.Log("HELD!!!!!!");
						holdingNote = true;
						Destroy(hit.collider.gameObject);
					}
					else
					{
						holdingNote = false;
					}
				}
				else
				{
					holdingNote = false;
				}
				holdingTimer -= Time.deltaTime;
				if (holdingTimer <= 0)//Every frame the timer is lowered, once it hits zero it resets to one and gives points
				{
					//5 Points
					holdingTimer = 1.0f;
				}
			}
			else
			{
				if (Physics.Raycast(keyPos + new Vector3(0, 1.5f, 0), Vector3.up, out hit, heldNotedistance + 1f))//Shoots raycast from the tip of note
				{
					if (hit.collider.tag == "HeldNote")//If the raycast hits a note that must be held...
					{
						Debug.Log("HELD!!!!!!");
						holdingNote = true;
						Destroy(hit.collider.gameObject);
					}
					else
					{
						holdingNote = false;
					}
				}
				else
				{
					holdingNote = false;
				}
				holdingTimer -= Time.deltaTime;
				if (holdingTimer <= 0)//Every frame the timer is lowered, once it hits zero it resets to one and gives points
				{
					//5 Points
					holdingTimer = 1.0f;
				}
			}

		}
	}

    //Plays the audio attatched to the key and sets its material to down
    public void PlayNote()
	{
		StopCoroutine("SoundStop");
		GetComponent<AudioSource>().volume = 1;
		stamp = false;
		//Debug.Log("NotePlay");
		GetComponent<AudioSource>().Play();
        keyModelRenderer.material = keyDown;
		
		heldNotedistance = 0;

		if (whiteKey)
		{
			if (Physics.Raycast(keyPos + new Vector3(0, 2.3f, 0), Vector3.up, out hit, 1))//Shoots raycast from the tip of note
			{
				if (hit.collider.tag == "Note" || hit.collider.tag == "SharpNote")//If the raycast hits a regular or sharp note...
				{
					if (hit.distance <= .7f && hit.distance > .45f)//Sweet spot distance
					{
						holdingNote = true;
						Debug.Log("Sweet Spot");
						//Stores the distance hit in order to check forheld notes
						heldNotedistance = hit.distance;
						Destroy(hit.collider.gameObject);
					}
					else if (hit.distance > .7f)//To far distance
					{
						holdingNote = true;
						Debug.Log("Too far");
						heldNotedistance = hit.distance;
						Destroy(hit.collider.gameObject);
					}
					else//Not to far but not in sweet spot
					{
						holdingNote = true;
						Debug.Log("Too close");
						heldNotedistance = hit.distance;
						Destroy(hit.collider.gameObject);
					}
				}
			}
		}
		else
		{
			if (Physics.Raycast(keyPos + new Vector3(0, 1.5f, 0), Vector3.up, out hit, 1))//Shoots raycast from the tip of note
			{
				if (hit.collider.tag == "Note" || hit.collider.tag == "SharpNote")//If the raycast hits a regular or sharp note...
				{
					if (hit.distance <= .7f && hit.distance > .45f)//Sweet spot distance
					{
						holdingNote = true;
						Debug.Log("Super Sweet Spot");
						//Stores the distance hit in order to check forheld notes
						heldNotedistance = hit.distance;
						Destroy(hit.collider.gameObject);
					}
					else if (hit.distance > .7f)//To far distance
					{
						holdingNote = true;
						Debug.Log("Too far");
						heldNotedistance = hit.distance;
						Destroy(hit.collider.gameObject);
					}
					else//Not to far but not in sweet spot
					{
						holdingNote = true;
						Debug.Log("Too close");
						heldNotedistance = hit.distance;
						Destroy(hit.collider.gameObject);
					}
				}
			}
		}
	}
	//Stops the audio attatched to the key and sets its material to up
	public void StopNote()
    {
		Debug.Log("NoteEnd");
		StartCoroutine("SoundStop");
		stamp = true;
		keyModelRenderer.material = keyUp;
		StopHolding();
    }

    public void StopHolding()
    {
        if (holdingNote)//If they were holding a note, destroy the note, stop looping in update, and reset timer
        {
            holdingTimer = 1.0f;
            holdingNote = false;
        }
    }
	public void TimeStamp()
	{
		GetComponent<AudioSource>().volume -= Time.deltaTime * 8;
	}

	IEnumerator SoundStop()
	{
		yield return new WaitForSeconds(.5f);

		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().volume = 1;
		stamp = false;
	}
}