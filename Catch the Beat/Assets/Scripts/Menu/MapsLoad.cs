﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapsLoad : MonoBehaviour
{
    public class fruit_point
    {
        public fruit_point(int X, int Y, int Time, Fruit.types _type, Color32 c)
        {
            type = _type;
            x = X;
            y = Y;
            color = c;
            time = Time;
        }
        public int time;
        public Fruit.types type;
        public Color32 color;
        public int x;
        public int y;
    }
    [SerializeField]
    public int loadType;
    [SerializeField]
    logo Logo;
    [SerializeField]
    AudioLoad audioLoad;
    Fruit fruit, drop, little;
    private Color32[] colors;
    private AudioSource audioSource;
    private ArrayList bitmap;
    private StreamReader input;
    private String background;
    public static Canvas bg;
    public static string currentMap;
    public static float HPDrainRate;
    public static float CircleSize;
    public static float OverallDifficulty;
    public static float ApproachRate;
    private bool isPlaying = false;
    private String inputText,map;
    bool isNotPlaying = true;
    Vector2 min, max;
    private float lenX, lenY, maxY;
    public static Vector3 scale = new Vector3(1, 1, 1);

    void infLoad()
    {
        colors = new Color32[4];
        colors[0] = new Color32(158, 47, 255, 255);
        colors[1] = new Color32(255, 76, 185, 255);
        colors[2] = new Color32(36, 166, 101, 255);
        colors[3] = new Color32(46, 132, 164, 255);

        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        lenX = max.x - min.x - 3f * Player.sprite.size.x;
        maxY = max.y;

        fruit = Resources.Load<Fruit>("Prefabs/fruit");
        little = Resources.Load<Fruit>("Prefabs/Little Fruit");
        drop = Resources.Load<Fruit>("Prefabs/drop");

        
    }

    void fileParse()
    {
        input = File.OpenText(Application.persistentDataPath + '/' + MenuLoad.folder + '/' + MenuLoad.map);
        while ((inputText = input.ReadLine()) != null)
        {
            if (inputText == "[Difficulty]") break;
        }
        inputText = input.ReadLine();
        HPDrainRate = float.Parse(inputText.Substring(12));
        inputText = input.ReadLine();
        CircleSize = float.Parse(inputText.Substring(11));
        inputText = input.ReadLine();
        HPDrainRate = float.Parse(inputText.Substring(18));
        inputText = input.ReadLine();
        ApproachRate = float.Parse(inputText.Substring(13));

        while ((inputText = input.ReadLine()) != null)
        {
            if (inputText == "[Events]") break;
        }
        inputText = input.ReadLine();
        inputText = input.ReadLine();
        if (inputText[0] == 'V') inputText = input.ReadLine();
        background = inputText.Split(',')[2];
    }

    void bitLoad()
    {
        while ((inputText = input.ReadLine()) != null)
        {
            if (inputText == "[HitObjects]") break;
        }
        bitmap = new ArrayList();
        while ((inputText = input.ReadLine()) != null)
        {
            parse(inputText, ref bitmap);
        }
    }

    void settings()
    {
        scale = new Vector3(0.4f + 1 / CircleSize, 0.4f + 1 / CircleSize, 1);
        Fruit.speed = ApproachRate * max.y / 2.6f;
        fruit.transform.localScale = scale;
        Player.score = Instantiate(Resources.Load<playerScore>("Prefabs/Score"));
    }

    void parse(string str, ref ArrayList array)
    {
        int x1 = 0, x2 = 0, y1 = 0, y2 = 0, time = 0, repeat = 0, length = 0;
        string[] a = str.Split(',');

        x1 = int.Parse(a[0]);
        y1 = int.Parse(a[1]);
        time = int.Parse(a[2]);
        colors = new Color32[4];
        colors[0] = new Color32(158, 47, 255, 255);
        colors[1] = new Color32(255, 76, 185, 255);
        colors[2] = new Color32(36, 166, 101, 255);
        colors[3] = new Color32(46, 132, 164, 255);
        Color32 randColor = colors[UnityEngine.Random.Range(0, 4)];
        array.Add(new fruit_point(x1, y1, time, Fruit.types.FRUIT, randColor));
        for (int i = 0; i < str.Length; i++)
        {

            if (str[i] == 'B' || str[i] == 'P' || str[i] == 'L' || str[i] == 'C')
            {
                a = str.Substring(i + 2).Split(':');

                x2 = int.Parse(a[0]);
                y2 = int.Parse(a[1].Split(',')[0].Split('|')[0]);
                a = str.Split(',');
                repeat = int.Parse(a[6]);
                length = int.Parse(a[7].Split('.')[0]);

                for (int j = 0; j < repeat; j++)
                {
                    float t = time + length * Fruit.speed / 8f * j;
                    if (j % 2 == 0) createSlider(x1, x2, y1, y2, t, ref array, randColor, length);
                    else createSlider(x2, x1, y1, y2, t, ref array, randColor, length);
                }
                break;
            }
        }

    }

    private void createSlider(int x1, int x2, int y1, int y2, float time, ref ArrayList array, Color32 color, int length)
    {
        int len = (int)(length * Fruit.speed / 600f);
        len--;
        for (int j = 1; j < len; j++)
        {
            int dx = (x2 - x1) / len;
            float dt = (length * Fruit.speed / 8f) / len;
            if ((j + 2) % 4 == 0)
                array.Add(new fruit_point(x1 + dx * j, y2, (int)(time + dt * j), Fruit.types.DROPx2, color));
            else
                array.Add(new fruit_point(x1 + dx * j, y2, (int)(time + dt * j), Fruit.types.DROP, color));
        }
        array.Add(new fruit_point(x2, y2, (int)(time + length * Fruit.speed / 8f), Fruit.types.FRUIT, color));
    }

    public void loadRandomMap()
    {
        string[] directories = Directory.GetFiles(Application.persistentDataPath, "*.osu", SearchOption.AllDirectories);
        map = directories[UnityEngine.Random.Range(0, directories.Length)];
        map = map.Replace('\\', '/');
        input = File.OpenText(map);
        string[] temp = map.Split('/');
        
        MenuLoad.map = temp[temp.Length - 1];
        map = map.Substring(0, map.Length - temp[temp.Length - 1].Length);
        temp = map.Split('/');
        MenuLoad.folder = temp[temp.Length - 2];
        bitLoad();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        MenuLoad.timeBegin = Time.time;
        if (loadType == 0)
        {
            infLoad();
            fileParse();
            bgLoad(background.Substring(1, background.Length - 2));
            bitLoad();
            settings();
        }
        if (loadType == 1)
        {
            loadRandomMap();
            fileParse();
            bitLoad();
        }
        audioLoad.load();
    }

    void bgLoad(string s)
    {
        currentMap = s;
        SpriteRenderer image = bg.GetComponentInChildren<SpriteRenderer>();
        String path = Application.persistentDataPath + '/' + MenuLoad.folder + '/' + s;
        WWW www = new WWW("file://" + path);
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(tex);
        image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        var width = image.sprite.bounds.size.x;
        var height = image.sprite.bounds.size.y;
        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        image.transform.localScale = new Vector2((float)worldScreenWidth / width, (float)worldScreenHeight / height);
        image.color = new Color(1, 1, 1, 0.25f);
    }
    public void restartMusic()
    {
        audioLoad.load();
    }
    void gameUpdate()
    {
        if (isPlaying && !AudioLoad.audioSource.isPlaying && bitmap.Count == 0)
        {
            SceneManager.LoadScene("mapEnd");
        }
        if (!isPlaying && 1f <= (Time.time - MenuLoad.timeBegin) && !AudioLoad.audioSource.isPlaying && AudioLoad.audioSource.clip.isReadyToPlay && isNotPlaying)
        {
            AudioLoad.audioSource.Play();
            AudioLoad.audioSource.volume = 0.3f;
            isPlaying = true;
        }
        foreach (fruit_point f in bitmap)
        {
            if (f.time <= ((Time.time - MenuLoad.timeBegin - 1f) * 1000))
            {


                Vector3 pos = Vector3.zero;
                pos.x = f.x * lenX / 512f - lenX / 2f;
                pos.y = maxY;
                if (f.type == Fruit.types.FRUIT)
                {
                    Fruit newFruit = Instantiate(fruit, pos, transform.rotation);
                    newFruit.initialize(f.color, f.type);
                }
                if (f.type == Fruit.types.DROP)
                {
                    Fruit newDrop = Instantiate(drop, pos, transform.rotation);
                    newDrop.initialize(f.color, f.type);
                    newDrop.transform.localScale = scale;
                }
                if (f.type == Fruit.types.DROPx2)
                {
                    Fruit newDrop = Instantiate(little, pos, transform.rotation);
                    newDrop.initialize(f.color, f.type);
                    newDrop.transform.localScale = scale;
                }
                bitmap.Remove(f);
                break;
            }
        }
    }

    void menuUpdate()
    {
        foreach (fruit_point f in bitmap)
        {
            if (f.time <= ((Time.time - MenuLoad.timeBegin) * 1000))
            {
                Logo.beat();
                bitmap.Remove(f);
                break;
            }
        }
    }
    void Update()
    {
        if (loadType == 0)
        {
            gameUpdate();
        }
        if (loadType == 1)
        {
            menuUpdate();
        }
    }
}