using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public List<Window> openWindows;
    private void Awake()
    {
        openWindows = new List<Window>();
    }
    private void Start()
    {

        CloseAllWindows();
    }
    void CloseAllWindows()
    {
        foreach (Window window in GetComponentsInChildren<Window>())
        { window.Close(); }
    }
    public bool IsMouseOverWindows()
    {
        foreach (Window openWindow in openWindows)
        {
            if (openWindow.isActiveAndEnabled && RectTransformUtility.RectangleContainsScreenPoint(openWindow.GetComponent<RectTransform>(), Input.mousePosition))
            {
                return true;
            }
        }
        return false;
    }
    public bool AreThereOpenWindows()
    {
        foreach (Window openWindow in openWindows)
        {
            if (openWindow.isActiveAndEnabled)
            {
                if (openWindow.OOBclose && !RectTransformUtility.RectangleContainsScreenPoint(openWindow.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    openWindow.Close();
                    continue;
                }
                return true;
            }
        }
        return false;
    }
    public Window FindWindow(string ID, bool open = false)
    {
        foreach (Window openWindow in openWindows)
        {
            if (openWindow.name == ID && (open || !openWindow.isActiveAndEnabled))
                return openWindow;
        }
        return null;
    }
    public void OpenWindow(Window win, bool closeOthers = true)
    {
        if (win == null)
            return;
        if (closeOthers)
            CloseWindows();
        win.Open();
        RegisterWindow(win);
    }
    public wtype OpenWindow<wtype>() where wtype:Window
    {
        wtype[] windows = GetComponentsInChildren<wtype>(true);
        if (windows .Length>0)
        {
            OpenWindow(windows[0]);
            return windows[0];
        }
        else
        {
            Debug.LogError($"Window of type {typeof( wtype)} not found!");
            return null;
        }
    }
    public void RegisterWindow(GameObject gob)
    {
        openWindows.Add(gob.GetComponent<Window>());
    }
    public void RegisterWindow(Window window)
    {
        openWindows.Add(window);
    }
    public void CloseWindows()
    {
        foreach (Window openWindow in openWindows)
        {
            openWindow.Close();
        }
        openWindows.Clear();
    }
    public void CloseWindow(string ID)
    {
        Window openWindow = FindWindow(ID, false);
        if (openWindow != null)
        {
            openWindow.Close();
        }
    }
    public void ToggleWindow(Window w)
    {
        if (w.IsOpen())
        {
            w.Close();
        }
        else
        {
            if (openWindows.Count > 0) CloseWindows();
            OpenWindow(w);
        }
    }
}
