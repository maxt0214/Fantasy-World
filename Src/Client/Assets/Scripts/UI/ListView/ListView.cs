using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>
{

}

public class ListView : MonoBehaviour
{
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnSelected(selected); }
        }

        public virtual void OnSelected(bool ifSelected)
        {
        }

        public ListView owner;
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!selected)
            {
                Selected = true;
            }
            if(owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }
    }

    public UnityAction<ListViewItem> OnItemSelected;

    List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem;
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        private set
        {
            if(selectedItem != null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            if (OnItemSelected != null)
                OnItemSelected.Invoke(value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        items.Add(item);
    }

    public void Clear()
    {
        foreach(var item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    public void ClearSelection()
    {
        selectedItem.Selected = false;
        selectedItem = null;
    }
}
