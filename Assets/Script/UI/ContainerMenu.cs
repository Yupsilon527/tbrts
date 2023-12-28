using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerMenu : MonoBehaviour
{
    [Header("Components")]
    public RectTransform transformParent;
    public Scrollbar hSlider;
    public GameObject copyObject;

    [Header("Settings")]
    public bool Horizontal = false;
    public int HorizontalOverride = 0;
    public int VerticalOverride = 0;

    protected virtual void Awake()
    {
        if (transformParent == null)
            transformParent = GetComponent<RectTransform>();
        if (copyObject == null)
            copyObject = transformParent.GetChild(0).gameObject;

        if (hSlider == null)
            hSlider = GetComponentInChildren<Scrollbar>();
        hSlider.onValueChanged.AddListener((float nValue) =>
        {
            ScrollList(nValue);
        });


        CalcMaxEntries();
        Refresh();
    }
    #region Max Entries
    int xrows = 0;
    int ycollums = 0;
    int totalEntries = 0;
    protected virtual void CalcMaxEntries()
    {
        if (copyObject.TryGetComponent(out RectTransform rect))
        {
            xrows = HorizontalOverride> 0 ? HorizontalOverride :Mathf.FloorToInt(GetComponent<RectTransform>().rect.width / rect.rect.width  - 1);
            ycollums = VerticalOverride > 0 ? VerticalOverride : Mathf.FloorToInt(GetComponent<RectTransform>().rect.height / rect.rect.height - 1 );
        }
    }
    #endregion
    #region List Contents
    protected void Refresh()
    {
        ClearList();
        PopulateList();        
            EnableDisableSlider(totalEntries > GetTotalButtons());
        
    }
    protected virtual bool PopulateList()
    {
        totalEntries = 30 + Random.Range(0, 20);
        for (int i = 0; i < totalEntries; i++)
        {
            GameObject btn = PoolEmptyContainer();
            btn.SetActive(true);
        }
        return true;
    }
    public int GetTotalButtons()
    {
        return xrows * ycollums;
    }
    protected virtual GameObject PoolEmptyContainer()
    {
        foreach (Transform child in transformParent)
        {
            if (!child.gameObject.activeSelf)
            {
                return child.gameObject;
            }
        }
        GameObject nGO = GameObject.Instantiate(copyObject);
        nGO.name = "Entry " + transformParent.childCount;
        nGO.transform.SetParent(transformParent);
        if (nGO.TryGetComponent(out RectTransform rectTransform)) //TODO recalc
        {
            int index = transformParent.childCount - 1;
            int iY = Mathf.CeilToInt(index / xrows);
            int iX = index % xrows;

            if (Horizontal)
            {
                iX = Mathf.FloorToInt(index / ycollums);
                iY = index % ycollums;
            }

            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y);
            rectTransform.localPosition = copyObject.GetComponent<RectTransform>().localPosition + new Vector3(rectTransform.rect.width * iX, -rectTransform.rect.height * iY,0);
            rectTransform.localScale = Vector3.one;
        }
        return nGO;
    }
    public virtual void ClearList()
    {
        foreach (Transform child in transformParent)
        {
            child.gameObject.SetActive(false);
        }
    }
    #endregion
    #region Slider
    float scrollListStep = 0;
    float listScroll = 0;
    protected virtual void EnableDisableSlider(bool valeu)
    {
        hSlider.value = 0;
        hSlider.gameObject.SetActive(valeu);

        listScroll = Mathf.CeilToInt(totalEntries / (Horizontal ?  ycollums : xrows));
        if (listScroll > 0)
        {
            listScroll = listScroll - (Horizontal ? xrows : ycollums);
            scrollListStep = 1 / listScroll;
            hSlider.numberOfSteps = Mathf.CeilToInt(1f / scrollListStep);
        }
    }
    public virtual void ScrollList(int entry)
    {
        ScrollList(scrollListStep * entry);
    }
    public virtual void ScrollList(float nValue)
    {
        if (Horizontal)
            transformParent.anchoredPosition = new Vector2(-copyObject.GetComponent<RectTransform>().rect.width * (nValue * listScroll), transformParent.anchoredPosition.y);
        else
            transformParent.anchoredPosition = new Vector2(transformParent.anchoredPosition.x, copyObject.GetComponent<RectTransform>().rect.height * (nValue * listScroll));
    }
    #endregion
}
