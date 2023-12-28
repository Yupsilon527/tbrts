
using UnityEngine;

public class TagEventTrigger : BaseEventTrigger
{
    public string CheckTag = "";
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (CheckTag == "" || other.gameObject.CompareTag( CheckTag))
        {
            OnEnter();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (CheckTag == "" || other.gameObject.CompareTag(CheckTag))
        {
            OnExit();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (CheckTag == "" || other.gameObject.CompareTag(CheckTag))
        {
            OnStay();
        }
    }
    
}
