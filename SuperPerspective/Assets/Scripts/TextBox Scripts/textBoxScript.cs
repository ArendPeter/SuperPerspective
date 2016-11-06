using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class textBoxScript : MonoBehaviour {


	public static textBoxScript instance;

//Visualization variables
	public GameObject textStartPoint; //Tells where our text will begin generating.

    public convoNode currentNode; //This is the node we are currently pulling info from

    public askBoxScript askBoxPartner; //Our textBox's askBox partner to switch between
    public PlayerController player;//Our connection to the player in the scene.


    public string[] convArray; //Holds the strings we want to display in the textbox.
	private char[] charArray; //Holds our strings, converted to characters
	private int conversationPoint = 0;//Tells us how many nodes into the conversation we are.

	public int lineLength = 25; //Number of characters per line
	public float kerning = 1f;//Space between characters
	public float vSpace = 1f;//Space between lines

//Display Speed variables
	public float textSpeed = 15f;
	public float endLineBlinkSpeed = 1f;
	[SerializeField] private float resetTextSpeed;
	[SerializeField] private float tinyTimer = 1f;
	[SerializeField] private float resetTimer;

//Keeps track of our cursor so we know where we are in the string.
	private int horiIndex = 0;
	private int vertIndex = 0;

	//Branching option variables
	public string[] choiceArray; // The strings that make up the answers we can make


	public int questionLine = 0; //Used to track which line we are currently making.
	public int choiceIndex = 0;//Used for indicating what choice we're thinking of making.

//Determines whether or not we should show the blinking line ending.
	private bool showEndLine = false;
//Determines whether or not we have finished displaying the line
	private bool isFinishedDisplaying = false;
//Determine whether or not we should be able to see the textbox at all.
	public bool showBox = false;
//The textbox we want to edit.
	public UnityEngine.UI.Image textbox;
//Text so we have something to edit
	public Text text;


    [SerializeField]
    private CharacterChatter chatter;
    private int chatterTrigger = 2;
    private int chatterCount = 0;

    // Use this for initialization
    void Start () {
        chatter = GameObject.FindObjectOfType<CharacterChatter>();
        //singleton
        if (instance == null){
			instance = this;
		}else{
			Destroy(this);
		}
		//link action
		InputManager.instance.JumpPressedEvent += progressConv;
		//start out disabled
		disableBox();
        //So these don't get overwritten later.
        resetTimer = tinyTimer;
        resetTextSpeed = textSpeed;

        
    }

	// Update is called once per frame
	void Update () {
		printLoop();
	}

    //The initialization function for when our program has started running.
    public void init()
    {
        //So we can make our player stop moving.
        player = GameObject.FindObjectOfType<PlayerController>();

        if (currentNode.myType != convoNode.nodeType.question) {//Make sure to check whether or not we should even be bothering to set up.
            convArray = currentNode.convoTextArray;//Load up our text to sidplay from our node.
            text.text = "";//Make sure our text is empty before we start.

            //We set these next two so that the timers will know what number to reset to.
            resetTimer = tinyTimer;
            resetTextSpeed = textSpeed;

            charArray = new char[convArray[conversationPoint].Length];//We get the length of the string in our current conversationPoint.
            isFinishedDisplaying = false;
            conversationPoint = 0;//So that we start at the BEGINNING of the convo
            prepStrings();
        }
        else//If it's just a question, immediately activate our Ask Box buddy.
        {
            disableBox();
            askQuestion(currentNode);
        }


	}

	//Initialize after the program has started
	public void reInit()
	{
		text.text = "";
		tinyTimer = resetTimer;
		textSpeed = resetTextSpeed;
		conversationPoint = 0;
		charArray = new char[convArray[conversationPoint].Length];//We get the length of the string in our current conversationPoint.
		isFinishedDisplaying = false;
		prepStrings();
		horiIndex = 0;
		vertIndex = 0;
        PlayerController.instance.setCutsceneMode(true);
    }

	//Makes the box show after resetting it.
	public void startConvo()
	{
			startConvo(currentNode);
	}


	//Overloaded enable that allows for new conversations to be loaded in.
	public void startConvo(convoNode newNode)
	{
        
        PlayerController.instance.setCutsceneMode(true);
		currentNode = newNode;
        if (currentNode.myType == convoNode.nodeType.question)
        {
            askQuestion(currentNode);
        }
        else
        {
            convArray = currentNode.convoTextArray;
            reInit();
            enableBox();
        }
	}


	public void endConvo()//Ends the conversation! Use this at the end of EVERYTHING.
	{
        //textSpeed = resetTextSpeed;
        showBox = false;
		disableBox();
	}
	public void enableBox()//This enables the textbox so you can see it.
	{
		textbox.enabled = true;
		text.enabled = true;
        showBox = true;
	}

	public void disableBox()//This disables the textbox so that you can't see it.
	{
		textbox.enabled = false;
		text.enabled = false;
        showBox = false;
		PlayerController.instance.setCutsceneMode(false);
	}

	//Resets the Text so that the next line can display properly.
	public void resetDisplay()
	{
		tinyTimer = resetTimer;
		textSpeed = resetTextSpeed;
		prepStrings();
		text.text = "";
		showEndLine = false;
		isFinishedDisplaying = false;
		conversationPoint++;
		charArray = new char[convArray[conversationPoint].Length];//We get the length of the string in our current conversationPoint.
		prepStrings();
	}

    //This runs constantly to decide whether or not the graphics should be displayed or not.
	public void showTextboxCheck()
	{
		if (!showBox || askBoxPartner.showBox)
		{
			disableBox();
		}
		else if (showBox)
		{
			enableBox();
		}
	}

	//Go 1 step forward in the conversation
	public void progressConv()
	{
		if (showBox) {//If we're actually active....

            /*
            Okay, so if we're actually active, at this point we wanna check what type of node we're handling.
            At this point, there's two cases. Either [it has text] or [it has no text]
                Case 1 happens when we're either [both] or [textOnly]
                Case 2 happens when we're [question]
            */

            //Case 1. WE DEFINITELY HAVE TEXT. We'll check if there's a question at the end later.
            if (currentNode.myType == convoNode.nodeType.textOnly || currentNode.myType == convoNode.nodeType.both)
            {
                if (horiIndex == charArray.Length)
                { //If our line is finished displaying...
                    if (conversationPoint < convArray.Length - 1)
                    {//...And we aren't at the end of our conversation, then do this.
                        resetDisplay();//This moves us to the next text display.

                    }
                    else//If we ARE at the end of our conversation, close the box. OR move to the question at the end.
                    {
                        if (currentNode.myType == convoNode.nodeType.both) //THIS MEANS WE HAVE A QUESTION TO ANSWER. Activate the Askbox.
                        {
                            askQuestion(currentNode);
                        }
                        else //This means we have no questions, and can safely end the conversation once and for all.
                        {
                            endConvo();
                        }
                        if (currentNode.hasTextEndEvent)
                        {
                            currentNode.endTextEvent.eventTrigger();
                        }
                    }
                }
                else//Otherwise, MAKE THE LINE FINISH DISPLAYING.
                {
                    displaySpeedSkip();
                }
            }
            else//This means the currentNode's type is "question" and we can turn on the AskBox.
            {
                askQuestion(currentNode);
            }


        }
	}

    //If the player is impatient and wants the text to display instantly isntead of being shown one character at a time, use this to force the whole line to display at once.
    public void displaySpeedSkip()
    {
        while (horiIndex < charArray.Length)
        {
            text.text = text.text + charArray[horiIndex];
            horiIndex++;
            /*if (horiIndex % lineLength == 0 && horiIndex != charArray.Length)//Check if we've hit the line length by checking if your horiIndex is divisible by our linelength
            {
                switch (charArray[horiIndex])
                {

                    case ' ':
                        text.text = text.text + "\n"; //We've hit an end, NEXT LINE.
                        vertIndex++;//Move down a line if we've hit the end.
                        break;
                    default:
                        text.text = text.text + "-\n"; //We've hit an end, NEXT LINE.
                        vertIndex++;//Move down a line if we've hit the end.
                        break;
                }
            }*/
        }
    }

	public void askQuestion(convoNode q)
	{
        print("trigger");
        askBoxPartner.activate(q);
        disableBox();
	}


	public void prepStrings()
	{
		if (conversationPoint < convArray.Length)
		{
			//Convert all of the strings of our current conversationPoint to characters
			charArray = convArray[conversationPoint].ToCharArray();
			horiIndex = 0;
			vertIndex = 0;
		}
	}

	//This method acts as an update loop for when the text box should be displaying normal conversation text.
	public void printLoop()
	{
		if (showBox) {//If we're even active...

			tinyTimer -= Time.deltaTime * textSpeed;

			//Every time we time down to zero, do the thing.
			if (tinyTimer <= 0)
			{

				if (horiIndex < charArray.Length)
				{
					textSpeed = resetTextSpeed;
					//Create a character at the horizontal length
					text.text = text.text + charArray[horiIndex];
					horiIndex++;
					/*if (horiIndex % lineLength == 0 && horiIndex != charArray.Length)//Check if we've hit the line length by checking if your horiIndex is divisible by our linelength
					{
						switch (charArray[horiIndex])
						{

							case ' ':
								text.text = text.text + "\n"; //We've hit an end, NEXT LINE.
								vertIndex++;//Move down a line if we've hit the end.
								break;
							default:
								text.text = text.text + "-\n"; //We've hit an end, NEXT LINE.
								vertIndex++;//Move down a line if we've hit the end.
								break;
						}
					}*/
					tinyTimer = resetTimer;
                    showEndLine = false;
                    chatterCheck();
                }

				else
				{
					isFinishedDisplaying = true;
					textSpeed = endLineBlinkSpeed;
					showEndLine = !showEndLine;
					if (showEndLine)
					{
						text.text = text.text + " ■";
					}
					else
					{
						text.text = text.text.Remove(text.text.Length - 2);
					}
					tinyTimer = resetTimer;

				}
			}
		}
	}

	public bool IsEnabled(){
			return showBox;
	}

    //Used to check if we should make the chatterNoise
    private void chatterCheck()
    {
        if (chatterCount == chatterTrigger)
        {
            //print("CHATTER");
            chatter.Chatter();
            chatterCount = 0;
        }
        else
        {
            chatterCount++;
        }
    }

}
