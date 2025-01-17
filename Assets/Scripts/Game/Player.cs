﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	
    [SerializeField]
    public static float speed;

    private bool isMovingRight = false;
    private bool isMovingLeft = false;
    private bool isHasted = false;
    bool isDoubleHasted = false;
    public static playerScore score;
    private float dt;
    MapsLoad mapsLoad;
    private Score_Numbers_Instance combo_inst;
//	private finalStatistics statistics;
    public static SpriteRenderer sprite;
    public Sprite s;
    public static speedEffect seff, hseff;
    float hasteTime=0;
    float speedTimer = 0f;
    public static BoxCollider2D _collider;
    int useCount=0;
    public int currentFruit;
    public static float combo;
    float[] fruitTime;
    float minSpeed;

    public static int comboEff;
    public static bool isFinishing = false;
    private void Awake()

    {
        GameObject.Find("mapScript").GetComponent<MapsLoad>().loadType = 0;
        mapsLoad = GameObject.Find("mapScript").GetComponent<MapsLoad>();
        finalStatistics.ZeroStats();
        currentFruit = 0;
        health.restart();
        combo_inst = Camera.main.GetComponent<Score_Numbers_Instance>();
//		statistics = Camera.main.GetComponent<finalStatistics> ();
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        minSpeed = (max.x - min.x)/1.4f;
        speed = minSpeed;
        sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<BoxCollider2D>();
        seff = Resources.Load<speedEffect>("Prefabs/speedEffect");
        hseff = Resources.Load<speedEffect>("Prefabs/hastedEffect");
        combo_inst.fruit_counter = 0;
        GameObject.Find("mapScript").GetComponent<MapsLoad>().loadGame();
        if (MapsLoad.DT) Time.timeScale = 1.4f;
        else
        {
            Time.timeScale = 1f;
        }
        if (MapsLoad.HD) Instantiate(Resources.Load<Canvas>("HD"));
     
        fruitTime = new float[mapsLoad.getFruitTime().Length];
        fruitTime = mapsLoad.getFruitTime();
        if (MapsLoad.AD) foreach (EventTrigger button in GameObject.Find("Interface").GetComponentsInChildren<EventTrigger>()) button.enabled = false;
    }

    private void Update()
    {
        speedEffect();
        sprite.transform.localScale = MapsLoad.scale * 1.25f;
        if (MapsLoad.AD == false)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.LeftArrow)) moveLeft();
            if (Input.GetKeyUp(KeyCode.LeftArrow)) stopLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow)) moveRight();
            if (Input.GetKeyUp(KeyCode.RightArrow)) stopRight();
            if (Input.GetKeyDown(KeyCode.LeftShift)) startHaste();
            if (Input.GetKeyUp(KeyCode.LeftShift)) stopHaste();
        }
        else
        {

            autoDrive();
        }
    }
    public void autoDrive()
    {
        Move();
        if (currentFruit == fruitTime.Length) return;
        float dx = sprite.transform.position.x - fruitTime[currentFruit];
        float delta = Mathf.Abs(dx);
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        max.x /= 23f;
        if (delta < max.x && isHasted) stopHaste();

        if (delta > max.x * 2)
        {
            if (dx > 0)
            {
              if (delta > speed / 7f && !isHasted) startHaste();
              moveLeft();
            }
            else if (dx < 0)
            {
              if (delta > speed / 7f && !isHasted) startHaste();
              moveRight();
            }
        }
    }
    void OnTriggerEnter2D (Collider2D col)
    {
        if (MapsLoad.AD == true)
        {
            
            currentFruit++;
            stopLeft();
            stopRight();
            if(isHasted)stopHaste();
        }
        isFinishing = col.gameObject.GetComponent<Fruit>().isFinishing;
        
        isDoubleHasted = col.gameObject.GetComponent<Fruit>().isHasted;
        if (isDoubleHasted)
        {
            hasteTime = Time.time;
        }
        combo = combo_inst.fruit_counter;
        comboEff = (int)combo_inst.fruit_counter;
		finalStatistics.comboCounter = (int)combo_inst.fruit_counter;
		
       
        score.scoreUp();
        finalStatistics.finalScore = score.score;
        Fruit f = col.GetComponent<Fruit>();
        Vector2 pos = transform.position;
        Effects eff;
        combo_inst.fruit_counter++;
        combo_inst.Boom();
        if (f.type == Fruit.types.FRUIT)
        {
            health.add();
            
			finalStatistics.big_fruits_counter++;
            pos.y = transform.position.y + sprite.size.y * MapsLoad.scale.y/1.8f ;
            eff = Instantiate(Resources.Load<Effects>("Prefabs/HitEffect"), pos, transform.rotation);
            eff.initialize(eff, col.gameObject.GetComponentInChildren<SpriteRenderer>().color, (col.gameObject.transform.position.x - sprite.transform.position.x)*0.45f,0);
        }


		if (f.type == Fruit.types.DROPx2) {
			
			finalStatistics.medium_fruits_counter++;
            health.add();
        }

		if (f.type == Fruit.types.DROP) {
		
			finalStatistics.small_fruits_counter++;
            health.add();
        }

        pos.y = transform.position.y + sprite.size.y * MapsLoad.scale.y / 1.57f;
         eff = Instantiate(Resources.Load<Effects>("Prefabs/HitEffect 1"), pos, transform.rotation);
        eff.initialize(eff, col.gameObject.GetComponentInChildren<SpriteRenderer>().color, (col.gameObject.transform.position.x - sprite.transform.position.x) * 0.45f, 1);
//        Destroy(col.gameObject);
    }
    private void Move()
    {

		Vector2 min = Camera.main.ViewportToWorldPoint (new Vector2 (0,0));
		Vector2 max = Camera.main.ViewportToWorldPoint (new Vector2(1,1));

		max.x = max.x - (1.5f*sprite.size.x);

		min.x = min.x + (1.5f*sprite.size.x);


        if (isMovingLeft)
        {
			Vector3 position = transform.position;

            Vector3 dir = transform.right * (-1);

			position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

			position.x = Mathf.Clamp (position.x, min.x, max.x);
			position.y =  Mathf.Clamp (position.y, min.y, max.y);

			transform.position = position;
        }
        if (isMovingRight)
        {
			Vector3 position = transform.position;

			Vector3 dir = transform.right * 1;

			position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

			position.x = Mathf.Clamp (position.x, min.x, max.x);
			position.y =  Mathf.Clamp (position.y, min.y, max.y);

			transform.position = position;
        }
        
    }

    public void moveLeft()
    {
		sprite.flipX = false;
        isMovingLeft = true;
    }

    public void stopLeft()
    {
        isMovingLeft = false;
    }

    public void moveRight()
    {
		sprite.flipX = true;
        isMovingRight = true;
    }

    public void stopRight()
    {
        isMovingRight = false;
    }

    void speedEffect()
    {
        speedTimer += Time.deltaTime;
        if (Time.time - hasteTime > 0.1f) isDoubleHasted = false;
        if (isHasted && useCount>0)
        {
            if (speedTimer > 0.01f)
            {
                sprite.color = new Color(1, 1, 1, sprite.color.a - 0.03f);
                if (isDoubleHasted)
                {
                  speedEffect eff = Instantiate(hseff, transform.position, transform.rotation);
                  eff.setTransparency(sprite, 0.8f);
                }
                else
                {
                  speedEffect eff = Instantiate(seff, transform.position, transform.rotation);
                  eff.setTransparency(sprite, 0.8f);
                }
                speedTimer = 0f;
            }
            
        } else sprite.color = new Color(1, 1, 1, sprite.color.a + 0.03f);
    }
    public void startHaste()
    {
        useCount++;
        dt = Time.time;
        sprite.color = new Color(1, 1, 1, 0f);
        if (isDoubleHasted)
        {
            speed = minSpeed * 3.5f;
        }
        else
        {
            speed = minSpeed * 2f;
        }
        isHasted = true;
    }

    public void stopHaste()
    {
        useCount--;
        if (useCount == 0)
        {
            speed = minSpeed;
            sprite.color = new Color(1, 1, 1, 1f);
            isHasted = false;
        }
    }


}
