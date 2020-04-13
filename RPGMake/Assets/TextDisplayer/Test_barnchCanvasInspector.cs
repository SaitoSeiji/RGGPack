using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_barnchCanvasInspector : MonoBehaviour
{
    [SerializeField]BranchDisplayer br;
    [SerializeField] List<string> branchData;

    [ContextMenu("branchTest")]
    public void BranchTest()
    {
        br.StartBranch(branchData);
    }
}
