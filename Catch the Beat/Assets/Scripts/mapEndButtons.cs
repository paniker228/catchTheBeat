﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mapEndButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void backToMenu()
    {
        Debug.Log("SUKA");
        SceneManager.LoadScene("menu");
    }
	void Update () {
		
	}
}
