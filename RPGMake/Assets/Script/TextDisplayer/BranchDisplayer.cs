using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BranchDisplayer : SingletonMonoBehaviour<BranchDisplayer>
{
    [SerializeField] List<string> _branchList;
    [SerializeField] RectTransform selectButtonParent;
    [SerializeField] Button selectButtonPrefab;

    [SerializeField] int? _selectedData;
    public int? _SelectedData { get { return _selectedData; } }
    bool canSelect;


    public bool _branchNow { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _branchNow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSelect && Input.GetKey(KeyCode.Z))
        {
            //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            canSelect = false;
        }
        else if (canSelect && UnityEngine.EventSystems.EventSystem.current == null)
        {
            canSelect = false;
        }
    }

    void DisplayContent()
    {
        for (int i=0;i<_branchList.Count;i++)
        {
            //Debug.Log(data.bName+":"+data.content);
            CreatButton(selectButtonParent, _branchList[i],i);
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectButtonParent.gameObject.transform.GetChild(0).gameObject);
    }

    void CreatButton(RectTransform parent,string data,int num)
    {
        var obj = Instantiate(selectButtonPrefab, Vector2.zero, Quaternion.identity);
        var text = obj.GetComponentInChildren<Text>();
        text.text = data;
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector2.one;
        obj.onClick.AddListener(() => SelectBranch(num));
    }

    void BreakButton(RectTransform tr)
    {
        foreach (Transform t in tr)
        {
            Destroy(t.gameObject);
        }
    }
    void SelectBranch(int selectNum)
    {
        _selectedData = selectNum;
    }

    #region public
    public void StartBranch(List<string> branchList)
    {
        _branchList = branchList;
        _branchNow = true;
        WaitAction.Instance.CoalWaitAction(() => canSelect = true, 0.5f);
        DisplayContent();
    }

    public bool CheckIsSelected()
    {
        return _selectedData != null;
    }

    public void EndBranch()
    {
        _branchNow = false;
        _selectedData = null;
        BreakButton(selectButtonParent);
    }
    #endregion

}
