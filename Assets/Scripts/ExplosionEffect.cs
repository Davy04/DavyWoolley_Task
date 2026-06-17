using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null; // aguarda um frame para o Animator inicializar

        Animator anim = GetComponent<Animator>();
        float duration = anim != null
            ? anim.GetCurrentAnimatorStateInfo(0).length
            : 1f;

        Destroy(gameObject, duration);
    }
}
