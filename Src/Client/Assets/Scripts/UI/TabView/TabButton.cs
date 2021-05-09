using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite highlighted;
    private Sprite normal;

    public TabView tabView;
    public int tabIndex;

    public bool selected = false;

    private Image btnImage;

    void Start()
    {
        btnImage = GetComponent<Image>();
        normal = btnImage.sprite;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Select(bool ifSelect)
    {
        btnImage.overrideSprite = ifSelect ? highlighted : normal;
        selected = ifSelect;
    }

    private void OnClick()
    {
        tabView.SelectTab(tabIndex);
    }
}
