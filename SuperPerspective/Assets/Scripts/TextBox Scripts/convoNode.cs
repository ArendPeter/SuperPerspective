using UnityEngine;
using System.Collections;

public class convoNode : MonoBehaviour {

    /// <summary>
    /// This is a node in a tree structure meant to mostly load info into the textboxes and ask boxes.
    /// 
    /// 
    /// </summary>

    public enum nodeType {textOnly, question, both }; // An enum we use to determine how our textboxes should react to us.

    public nodeType myType = nodeType.textOnly;


    public convoNode[] nextNodeArray;//The array of other nodes. Meant to make a tree structure out of the nodes.
    public bool hasNext;//Tells whether or not we are at the end of our path.
    public int choiceIndex = 0;//The ID of the next node we want to hea d toward
    public bool hasTextEndEvent = false;//Used in the case that you want an event at the end of a Text convo.
    public string[] convoTextArray;//The ENTIRE text you want the character to say

    public string question;//The question we ponder the the answer to.
    public string[] endQuestionArray;//The Answers you want the question to ask at the end.

    public textEvent[] endEventArray;//The Events you want to happen based on the answer to the question at the end.
    public textEvent endTextEvent;//Event that happens if you have text but want an event.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Returns the default next node
    public convoNode getNextNode()
    {
        return nextNodeArray[0];
    }


    //Returns the node requested by the askBox
    public convoNode getNextNode(int index)
    {
        return nextNodeArray[index];
    }
}
