using UnityEngine;
using System.Collections.Generic;

public class Table : MonoBehaviour
{

    public int numChairs;

    private List<string> noodlesBowl;
    private List<Message> waitQueue;
    private List<Message> inQueue;

    public void Awake ()
    {
        noodlesBowl = new List<string>();
        waitQueue = new List<Message>();
        inQueue = new List<Message>();

        for(int i = 0; i< numChairs; i++)
        {
            noodlesBowl.Add("C" + i);
        }
    }

    public void Update ()
    {
        ServeWaitQueue();
        ServeInQueue();
	}

    private void ServeInQueue()
    {
        foreach (Message msg in inQueue.ToArray())
        {
            int senderID = msg.getSenderID(); 
            MonoBehaviour sender = msg.getSender();

            if( noodlesBowl.Contains("C"+senderID) && noodlesBowl.Contains("C" + (msg.getSenderID() + 1) % numChairs))
            {
                noodlesBowl.Remove("C" + senderID);
                noodlesBowl.Remove("C" + (senderID+1) % numChairs);
                inQueue.Remove(msg);

                sender.SendMessage(Message.msgTypes.YouCanEat.ToString());
            }
            else
            {
                inQueue.Remove(msg);
                waitQueue.Add(msg);
            }

        }
    }

    private void ServeWaitQueue()
    {
        foreach (Message msg in waitQueue.ToArray())
        {
            int senderID = msg.getSenderID();
            MonoBehaviour sender = msg.getSender();

            if (noodlesBowl.Contains("C" + senderID) && noodlesBowl.Contains("C" + (msg.getSenderID() + 1) % numChairs))
            {
                noodlesBowl.Remove("C" + senderID);
                noodlesBowl.Remove("C" + (senderID + 1) % numChairs);
                waitQueue.Remove(msg);

                sender.SendMessage(Message.msgTypes.YouCanEat.ToString());
            }
        }
    }

    public void IWantToEat(Message msg)
    {
        inQueue.Add(msg);
    }

    public void IAmDoneEating(Message msg)
    {
        noodlesBowl.Add("C" + msg.getSenderID());
        noodlesBowl.Add("C" + (msg.getSenderID()+1) % numChairs);
    }

}
