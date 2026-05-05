using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    // Этот публичный метод будет вызван через Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}