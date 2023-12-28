using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffectController : SpecialEffectController
{
    public float borders = 10f;
    public TextMeshProUGUI tmPro;
    public RectTransform rectTransform;
    protected override void Awake()
    {
        base.Awake();
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (tmPro == null)
            tmPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ChangeTextValue(string t)
    {
        tmPro.text = t;
        rectTransform.sizeDelta = new Vector2(tmPro.preferredWidth + borders * 2, rectTransform.sizeDelta.y);
    }
    private void OnDisable()
    {
        Stop();
    }
}
