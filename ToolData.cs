using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Tool Data" ,menuName ="Data/Tool Data")]
public class ToolData : ScriptableObject
{
    public List<ToolDataSO> toolDataList;
}

[Serializable]
public class ToolDataSO
{
    public Tool tool;

    public ToolType type;
    
    public int damage;
}

