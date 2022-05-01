using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public bool follow;

    [Header("Follow Method Variables")]
    [SerializeField] Transform target;
    [SerializeField] float speed = 3f;

    [Header("Horizonral Follow")]  
    public float horizontalFollowLerpMult;
    public float horizontalClampVal;

    private Vector3 newPos = Vector3.zero;
    private Vector3 vel = Vector3.zero;

    [HideInInspector] public Vector3 offset;
    
    void Awake()
    {
        follow = true;
        OffsetCalculate();
        newPos.x = transform.position.x;
    }

    void LateUpdate()
    {
        if (follow)
            FollowMethod();
    }

    void FollowMethod()
    {
        if (target)
        {
            NewPosCalculator();
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref vel, speed);
        }
    }

    Vector3 NewPosCalculator()
    {
        NewPosX();
        newPos.y = target.position.y + offset.y;
        newPos.z = target.position.z + offset.z;
        return newPos;
    }

    float NewPosX()
    {
        newPos.x = target.position.x + offset.x;
        return newPos.x;
    }

    public Vector3 OffsetCalculate()
    {
        offset = transform.position - target.position;
        newPos.y = target.position.y + offset.y;
        return offset;
    }
    public void ReTarget() => OffsetCalculate();
}
