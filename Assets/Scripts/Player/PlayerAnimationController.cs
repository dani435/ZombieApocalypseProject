using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private string currentWeapon;
    private bool isReloading = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Movimento del giocatore
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        // Spara
        if (Input.GetMouseButtonDown(0))
        {
            
        }

        // Ricarica
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
           
        }
    }


}
