using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskData
{
    public int taskId;
    public string name;
    public string color;
    public string tipStyle;
    public int timeout;
    public string taskContent;
    public string image;
    public string imageTitle;
    public ChooseData[] chooseList;
}

[System.Serializable]
public class ChooseData
{
    public int index;
    public string text;
    public string[] checkList;
    public string[] Pass;
    public string[] Fail;
}