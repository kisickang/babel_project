using UnityEngine;

public class Monster : MonoBehaviour
{
    private float lifeTime = 10f;

    void OnEnable()
    {
        CancelInvoke();
        //Invoke(nameof(Despawn), lifeTime);
    }

    void Despawn()
    {
       // gameObject.SetActive(false);
    }
}
