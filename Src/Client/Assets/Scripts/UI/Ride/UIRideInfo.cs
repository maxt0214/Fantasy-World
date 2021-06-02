using UnityEngine;
using UnityEngine.UI;

public class UIRideInfo : MonoBehaviour
{
    public Text rideName;
    public Text description;

    public void SetRideInfo(string name, string des)
    {
        rideName.text = name;
        description.text = des;
    }
}
