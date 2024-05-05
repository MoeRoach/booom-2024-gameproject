using RoachFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskViewController : MonoSingleton<TaskViewController>
{
    public GameObject CurrentObj { get; set; }
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private TextMeshProUGUI imageTitle;
    [SerializeField] private TextMeshProUGUI buttonText_1;
    [SerializeField] private TextMeshProUGUI buttonText_2;
    [SerializeField] private TextMeshProUGUI buttonText_3;
    [SerializeField] private Transform viewTrans;
    private bool isTelescop = false;
    [SerializeField] private TextMeshProUGUI telescopText;

    // Start is called before the first frame update
    void Start()
    {
        AddTaskController(2001);
        AddTaskController(2003);
        AddTaskController(2009);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateView()
    {
        if (CurrentObj == null)
        {
            Debug.Log("CurrentObj == null");
            return;
        }

        Debug.Log(CurrentObj.GetComponent<TaskController>());
        TaskData data = CurrentObj.GetComponent<TaskController>().Data;
        // Debug.Log(data);
        // Debug.Log(data.image);
        // image.sprite = SpriteUtils.GetTaskImageSprite(data.image);
        // Debug.Log(image.sprite);
        content.text = data.taskContent;
        // imageTitle.text = data.imageTitle;
        buttonText_1.text = data.chooseList[1].text;
        buttonText_2.text = data.chooseList[2].text;
        buttonText_3.text = data.chooseList[3].text;
    }

    public void ClearView()
    {
        Debug.Log($"ClearView");
        if (CurrentObj == null)
        {
            Debug.Log("CurrentObj == null");
            return;
        }

        // Destroy(CurrentObj); //??

        CurrentObj = null;
        // selectIndex = -1;
        content.text = "";
        imageTitle.text = "";
        buttonText_1.text = "";
        buttonText_2.text = "";
        buttonText_3.text = "";
        image.sprite = null;
    }

    public GameObject AddTaskController(int taskId)
    {
        GameObject obj = Resources.Load("TaskSystemPrefabs/Task", typeof(GameObject)) as GameObject;
        Debug.Log(obj);
        if (obj == null) return null;
        obj = Instantiate(obj, viewTrans);
        obj.GetComponent<TaskController>().TaskDataInit(taskId);

        return obj;
    }

    public void TelescopicView()
    {
        float offset = (isTelescop ? -1 : 1) * GetComponent<RectTransform>().rect.width;
        transform.position = transform.position + new Vector3(offset, 0, 0);
        isTelescop = !isTelescop;
        telescopText.text = isTelescop ? "←" : "→";
    }
}
