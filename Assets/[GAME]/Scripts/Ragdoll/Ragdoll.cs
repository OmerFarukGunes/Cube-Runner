using UnityEngine;

public class Ragdoll : MonoBehaviour
{

    [Header("Sub-Components")]
    Collider[] subCollider;
    Rigidbody[] subRigidbody;

    [Header("Main Components")]
    Animator animatorOfTransform;
    Rigidbody rigidbodyOfTransform;
    Collider mainColliderOfTransform;


    public void SetUp(Transform ragdollParent, Animator anim, Rigidbody rbTransform, Collider colliderMain)
    {
        animatorOfTransform = anim;
        rigidbodyOfTransform = rbTransform;
        mainColliderOfTransform = colliderMain;

        subCollider = ragdollParent.GetComponentsInChildren<Collider>(true);
        subRigidbody = ragdollParent.GetComponentsInChildren<Rigidbody>(true);

        WholeBodyRagdoll(false);
    }
    public void WholeBodyRagdoll(bool ragdollState)
    {
        foreach (Collider collider in subCollider)
            collider.enabled = ragdollState;

        foreach (Rigidbody npcRB in subRigidbody)
            npcRB.isKinematic = !ragdollState;

        rigidbodyOfTransform.isKinematic = ragdollState; 
        mainColliderOfTransform.enabled = !ragdollState;
        animatorOfTransform.enabled = !ragdollState; 
    }
   
}