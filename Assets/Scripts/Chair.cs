using UnityEngine;
using System.Collections;
using System;

public class Chair : MonoBehaviour {

    public int id;
    public Material freeMaterial;
    public Material busyMaterial;
    public GameObject chop0;
    public GameObject chop1;

    private Vector3 chop0StartPos;
    private Vector3 chop1StartPos;
    private bool isAvail = true;
    private GameObject table;
    private GameObject actualOwner;


    public void Start()
    {
        ChangeColor("Free");
        chop0StartPos = chop0.transform.position;
        chop1StartPos = chop1.transform.position;
        table = GameObject.Find("Table");
    }

    public bool IsAvailable()
    {
        return isAvail;
    }

    public void setAvailable(bool newAvail)
    {
        isAvail = newAvail;
        if (isAvail) ChangeColor("Free");
        else ChangeColor("Busy");
    }

    public int getID()
    {
        return id;
    }

    private void ChangeColor(string mat)
    {
        Material surface = GetComponentsInChildren<Renderer>()[0].material;

        switch (mat)
        {
            case "Free":
                surface.color = freeMaterial.color;
                break;
            case "Busy":
                surface.color = busyMaterial.color;
                break;
            default:
                surface.color = freeMaterial.color;
                break;
        }

    }

    public void IWantToEat()
    {
        Message myMsg = new Message(id, this);
        table.SendMessage(Message.msgTypes.IWantToEat.ToString(), myMsg);
    }

    public void IAmDoneEating()
    {
        Message myMsg = new Message(id, this);
        table.SendMessage(Message.msgTypes.IAmDoneEating.ToString(), myMsg);

        chop0.transform.position = chop0StartPos;
        chop1.transform.position = chop1StartPos;
    }

    public void YouCanEat()
    {
        Vector3 vect = chop0.transform.position - chop1.transform.position;
        chop0.transform.position -= vect*0.5f;
        chop1.transform.position += vect*0.5f;

        actualOwner.SendMessage(Message.msgTypes.YouCanEat.ToString());
    }

    public void setOwner(GameObject owner)
    {
        actualOwner = owner;
    }

}
