using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public void Select()
    {
        Debug.Log(transform.parent.gameObject.name);
    }
}