using UnityEngine;

public class CapytaPokeReaction : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        // Automatically find Animator on the same object
        animator = GetComponent<Animator>();
    }

    public void OnPoked()
    {
        if (animator != null)
        {
            animator.SetTrigger("Poked");
        }
    }
}
