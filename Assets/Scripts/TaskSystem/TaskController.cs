using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;

public class TaskController : MonoBehaviour
{
    public TaskData Data { get; private set; }
    private float timer = 0;
    [SerializeField] private TextMeshProUGUI tipStyle;

    // Start is called before the first frame update
    void Start()
    {
        // TaskDataInit(2000);
    }

    // Update is called once per frame
    void Update()
    {
        TimeOutProcess();
    }

    public void ClearInfo()
    {
        Debug.Log($"ClearInfo");
        Data = null;
    }

    void TimeOutProcess()
    {
        timer += Time.deltaTime;
        GetComponent<Image>().fillAmount = 1.0f - Mathf.Lerp(0, 1, timer / Data.timeout);
        if (GetComponent<Image>().fillAmount <= 0)
        {
            TaskViewController.Instance.ClearView();
            // TaskViewController.Instance.AddToPool(Data.caseId);
            Destroy(gameObject);
        }
    }

    public void TaskDataInit(int taskId)
    {
        Debug.Log($"taskDataInit:{taskId}");
        ClearInfo();
        var fileContent = Resources.Load<TextAsset>($"Jsons/task_{taskId}");
        Data = JsonUtility.FromJson<TaskData>(fileContent.text);
        // Debug.Log(Data.color);
        // ColorUtility.TryParseHtmlString(Data.color, out Color color);
        // GetComponent<Image>().color = color;
        tipStyle.text = Data.tipStyle;

        // signal.sprite = SpriteUtils.GettaskSignalSprite(Data.signal);
    }

    public void Click()
    {
        Debug.Log("Click");
        if (TaskViewController.Instance == null)
        {
            Debug.Log("TaskViewController.Instance == null");
            return;
        }
        if (Data == null)
        {
            Debug.Log("Data == null");
            return;
        }
        TaskViewController.Instance.CurrentObj = gameObject;
        TaskViewController.Instance.UpdateView();
        // Debug.Log($"add case {gameObject.GetComponent<CaseController>().Data.caseId}");
        // Debug.Log($"add case {View.CurrentObj.GetComponent<CaseController>().Data.caseId}");
    }
}
