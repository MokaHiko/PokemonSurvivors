using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public virtual string GetName() {return "";}
    public virtual void OnFirstOpen() {}
    public virtual void OnClose() {}
    public virtual void OnOpen() {}

    public bool FirstOpen = true;
}
