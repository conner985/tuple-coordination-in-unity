using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PhilosopherV3 : MonoBehaviour {

    public Material thinkingMaterial;
    public Material eatingMaterial;
    public Material waitingMaterial;

    private Vector3 thinkingPosition;

    private GameObject table;
    private Chair chair; 

    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    
    private bool sit;

    void Start()
    {
        thinkingPosition = transform.position;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        table = GameObject.Find("Table");

        StartCoroutine(Think());
    }

    void Update()
    {
        if (sit) FaceTheTable();
    }

    private void FaceTheTable()
    {
        Vector3 dir = table.transform.position - transform.position;
        dir.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2.5f * Time.deltaTime); 
    }

    private IEnumerator WaitToReachThinkingPos()
    {
        bool think = false;
        while (!think)
        {
            if (Vector3.Distance(transform.position, thinkingPosition) < 1)
            {
                think = true;
                anim.SetTrigger("isIdle");
            }
            else { yield return null; }
        }
        StartCoroutine(Think());
    }

    private IEnumerator Think()
    {
        ChangeColor("Think");
        yield return new WaitForSeconds(UnityEngine.Random.Range(5, 15));
        IamHungry();
    }

    private void ChangeColor(string mat)
    {
        Material surface = GetComponentsInChildren<Renderer>()[1].material;

        switch (mat)
        {
            case "Think":
                surface.color = thinkingMaterial.color;
                break;
            case "Eat": 
                surface.color = eatingMaterial.color;
                break;
            case "Wait":
                surface.color = waitingMaterial.color;
                break;
            default:
                surface.color = thinkingMaterial.color;
                break;
        }

    }

    private void IamHungry()
    {
        StartCoroutine(FindAChair());
    }

    private IEnumerator FindAChair()
    {
        bool chairNotFound = true;
        while (chairNotFound)
        {
            RaycastHit[] hit;
            Debug.DrawRay(transform.position, transform.forward * 10, Color.yellow);
            hit = Physics.RaycastAll(transform.position, transform.forward * 10, 20);
            int i;
            for(i = 0; i<hit.Length; i++)
            {
                if (hit[i].collider != null && hit[i].collider.CompareTag("Chair"))
                {
                    chair = hit[i].transform.gameObject.GetComponent<Chair>();
                    if (chair.IsAvailable())
                    {
                        chairNotFound = false;

                        AcquireChair(chair); 
                        MoveTowardTarget(chair.transform.position);

                        StartCoroutine(WaitToSit());
                        break;
                    }
                }
            }
            transform.Rotate(0, -4, 0);
            yield return null; 
        }  
    }

    private void AcquireChair(Chair chair)
    {
        chair.setAvailable(false);
        chair.setOwner(this.gameObject);
    }

    private IEnumerator WaitToSit()
    { 
        sit = false;
        while (!sit)
        {
            if(Vector3.Distance(transform.position, chair.transform.position) < 1)
            { 
                sit = true; 
                anim.SetTrigger("isSitting");
            }
            else { yield return null; }
        }
        IWantToEat();
    }

    private void IWantToEat()
    {
        chair.SendMessage("IWantToEat");
        ChangeColor("Wait");
    }

    public void YouCanEat()
    {
        StartCoroutine(Eat());
    }

    private IEnumerator Eat()
    {
        ChangeColor("Eat");
        yield return new WaitForSeconds(UnityEngine.Random.Range(5,15));
        IAmDoneEating();
    }

    private void IAmDoneEating()
    {
        chair.SendMessage("IAmDoneEating");
        ReleaseChair();

        anim.SetTrigger("isStanding");

        Vector2 onCircle = UnityEngine.Random.insideUnitCircle.normalized*4.5f;
        Vector3 onCircle3D = new Vector3(onCircle.x, 0, onCircle.y); 
        thinkingPosition = onCircle3D;
        MoveTowardTarget(thinkingPosition);
        StartCoroutine(WaitToReachThinkingPos());

        ChangeColor("Think");
    }

    private void ReleaseChair()
    { 
        chair.setAvailable(true);
        chair.setOwner(null);
        chair = null;
        sit = false;
    }

    private void MoveTowardTarget(Vector3 position)
    {
        anim.SetTrigger("isWalking");
        agent.SetDestination(position);
    }

}
