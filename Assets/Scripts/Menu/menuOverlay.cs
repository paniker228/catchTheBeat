﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuOverlay : MonoBehaviour {

    // Use this for initialization
    int i = 0;
    SpriteRenderer sprite;
	void Start () {
        sprite = GetComponent<SpriteRenderer>();
    }

	void Update () {
        float sc = transform.localScale.x + Time.deltaTime / 1.5f;

        transform.localScale = new Vector3(sc, sc, 1);
        sprite.color = new Color(1, 1, 1, sprite.color.a - Time.deltaTime);
        if (sprite.color.a <= 0) Destroy(gameObject);
        
	}
}
