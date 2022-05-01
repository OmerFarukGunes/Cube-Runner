using DG.Tweening;
using Mechanics;
using System.Collections;
using TMPro;
using UnityEngine;
public class PlayerController : SwipeMecLast
{
    [Header("Camera")]
    [SerializeField] Camera camera;

    [Header("Forward Movement")]
    [SerializeField] public float forwardSpeed;

    [Header("User Control")]
    [HideInInspector] public bool userActive;

    [Header("VFX")]
    [SerializeField] Transform playerVFX;
    [SerializeField] Transform cube;
    [SerializeField] Material cubeMat;
    float lastScaleY = 1;
    [HideInInspector] public float bonusScale = 0;

    [Header("Effects")]
    [SerializeField] GameObject positiveEffect;
    [SerializeField] GameObject negativeEffect;
    [SerializeField] public GameObject windEffect;

    [Header("Point")]
    [SerializeField] TextMeshProUGUI pointText;

    [Header("Player Components")]
    [SerializeField] Animator animator;

    [Header("Power Up UI")]
    [SerializeField] GameObject powerUpUI;

    MaterialPropertyBlock materialProperty;

    Renderer renderer;
    float camFOV = 60;
    float camRotY = 35;
    int x = 0;
    public override void Start()
    {
        cubeMat.color = Color.red;
        materialProperty = new MaterialPropertyBlock();
        SetUp(transform, animator, GetComponent<Rigidbody>(), GetComponent<Collider>());
        base.Start();
    }
    void Update()
    {
        if (!userActive) return;
        transform.position += forwardSpeed * Time.deltaTime * transform.forward;
        Swipe();
        UIManager.instance.UpdateProgressBar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObsNegX2"))
        {
            SetEffect(2);
            RecalculateCube(-.75f, other.gameObject, "Fall");
        }

        else if (other.CompareTag("ObsNeg"))
        {
            SetEffect(2);
            RecalculateCube(-.5f, other.gameObject, "Fall");
        }

        else if (other.CompareTag("Positive"))
        {
            SetEffect(1);
            RecalculateCube(1.25f + bonusScale, other.gameObject, "Jump");
        }
        else if (other.CompareTag("EndGame"))
            StartCoroutine(EndGame());
        else if (other.CompareTag("End"))
        {
            x++;
            renderer = other.GetComponent<Renderer>();
            renderer.GetPropertyBlock(materialProperty);
            materialProperty.SetColor("_Color", Color.cyan);
            renderer.SetPropertyBlock(materialProperty);
        }
    }

    void SetEffect(int state)
    {
        switch (state)
        {
            case 1:
                positiveEffect.SetActive(false);
                positiveEffect.SetActive(true);
                break;
            case 2:
                negativeEffect.SetActive(false);
                negativeEffect.SetActive(true);
                break;
        }
    }
    IEnumerator EndGame() 
    {
        windEffect.SetActive(false);
        powerUpUI.SetActive(false);
        forwardSpeed = 6;
        int makeMoney=0;
        while (lastScaleY>.5f)
        {
            lastScaleY -= .2f;
            camera.fieldOfView -= .2f;
            if (makeMoney % 3 == 0)
                UIManager.instance.MoneyCollectAnim(playerVFX.position);
            makeMoney++;
            playerVFX.DOLocalMoveY(lastScaleY - .95f, .05f).SetEase(Ease.Linear);
            cube.DOScaleY(lastScaleY, .05f).SetEase(Ease.Linear);
            pointText.text = ((int)(lastScaleY * 10)).ToString();
            yield return new WaitForSeconds(.05f);
            if (x == 10)
                lastScaleY = .5f;
        }

        pointText.transform.parent.gameObject.SetActive(false);
        animator.SetTrigger("Win");
        GameManager.instance.EndGame(2);
    }

    void RecalculateCube(float point,GameObject gameObject,string anim)
    {
        gameObject.SetActive(false);

        lastScaleY *= Mathf.Abs(point);

        pointText.text = ((int)(lastScaleY * 10)).ToString();
       
        if (lastScaleY> .5f && lastScaleY <20)
        {
            camFOV += point * 1.1f;
            camRotY += point / 2f;
            camera.transform.DORotate(new Vector3(camRotY,0,0), .3f);
            camera.DOFieldOfView(camFOV, .3f);
            animator.SetTrigger(anim);
            playerVFX.DOLocalMoveY(lastScaleY - .95f, .5f).SetEase(Ease.Linear);
            cube.DOScaleY(lastScaleY, .5f).SetEase(Ease.Linear);
        }
        else if (lastScaleY >=20)
        {
            lastScaleY = 20;
            playerVFX.DOLocalMoveY(20 - .95f, .5f).SetEase(Ease.Linear);
            cube.DOScaleY(20, .5f).SetEase(Ease.Linear);
            pointText.text = "MAX";
        }
        else
        {
            cube.DOScaleY(0, .5f).SetEase(Ease.Linear);
            WholeBodyRagdoll(true);
            GameManager.instance.EndGame(1);
        }

        if (lastScaleY>=10f)
            cubeMat.DOColor(Color.green, .5f);
        else if (lastScaleY > 3f)
            cubeMat.DOColor(Color.yellow, .5f);
        else
            cubeMat.DOColor(Color.red, .5f);
    }
    
    public void UserActiveController(bool desiredVal) => userActive = desiredVal;
}
