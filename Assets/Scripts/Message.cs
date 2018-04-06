using UnityEngine;

public class Message
{
    public enum msgTypes {IWantToEat, IAmDoneEating, YouCanEat }

    private int id;
    private MonoBehaviour sender;

    public Message(int id, MonoBehaviour sender)
    {
        this.id = id;
        this.sender = sender;
    }

    public int getSenderID()
    {
        return id;
    }

    public MonoBehaviour getSender()
    {
        return this.sender;
    }
}
