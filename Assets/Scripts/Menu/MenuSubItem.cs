﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSubItem : MonoBehaviour {

    [SerializeField]
    public string mapName;
    int num;
    Text[] labels;
    MenuLoad load;
    SpriteRenderer[] stars;
	public static string[] song;
	public static string song_name;
    public void initialize(string s, MenuLoad ml, int _num)
    {
        num = _num;
        load = ml;
        mapName = s;
        labels = GetComponentsInChildren<Text>();
        song = mapName.Split('-');
        if (song.Length == 1)
        {
            Destroy(gameObject);
            return;
        }
        string full = mapName.Substring(song[0].Length);
        labels[0].text = full.Substring(2).Split('(')[0];
        labels[1].text = song[0];
        labels[2].text = full.Split('[')[1].Split(']')[0];
        stars = GetComponentsInChildren<SpriteRenderer>();
    }
    public void selectMap()
    {
        MenuLoad.currentDiff = num;
        GameObject.Find("sounds").GetComponent<sounds>().MenuClick();
        GameObject.Find("Menu").GetComponent<MenuLoad>().selectDifficult(num);
        MenuLoad.map = mapName+".osu";
        song_name = mapName;

    }

}
