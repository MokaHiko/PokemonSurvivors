using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [SerializeField] 
    private List<Menu> _menus;

    public T FindMenu<T>()
    {
        foreach(Menu m in _menus)
        {
            if (m is T)
            {
                return (T)(object)m;
            }
        }

        return default(T);
    }
    public void OpenMenu<T>(bool closeOthers = false)
    {
        foreach(Menu m in _menus)
        {
            if (m is T)
            {
                if(m.FirstOpen)
                {
                    m.OnFirstOpen();
	        	}
                else
                { 
                    m.OnOpen();
	        	}
                m.gameObject.SetActive(true);
            }
            else if(closeOthers)
            {
                m.gameObject.SetActive(false);
                m.OnClose();
	        }
        }
    }

    public void OpenMenu(Menu menu, bool closeOthers = false)
    {
        foreach(Menu m in _menus)
        {
            if(m == menu)
            {
                if (m.FirstOpen)
                {
                    m.OnFirstOpen();
                    m.FirstOpen = false;
                }
                else
                {
                    m.OnOpen();
                }
                m.gameObject.SetActive(true);
            }
            else
            {
                m.OnClose();
                m.gameObject.SetActive(false);
            }
        }
    }

    public void OpenMenu(string menuName, bool closeOthers = false)
    {
        foreach(Menu m in _menus)
        {
            if(m.name == menuName)
            {
                if (m.FirstOpen)
                {
                    m.OnFirstOpen();
                    m.FirstOpen = false;
                }
                else
                {
                    m.OnOpen();
                }
                m.gameObject.SetActive(true);
            }
            else if(closeOthers)
            {
                m.OnClose();
                m.gameObject.SetActive(false);
            }
        }
    }
    public void CloseMenu(Menu menu)
    {
        foreach(Menu m in _menus)
        {
            if(m == menu)
            {
                m.OnClose();
                m.gameObject.SetActive(false);
            }
        }
    }

    public void CloseMenu(string menuName)
    {
        foreach(Menu m in _menus)
        {
            if(m.name == menuName)
            {
                m.OnClose();
                m.gameObject.SetActive(false);
            }
        }
    }
}
