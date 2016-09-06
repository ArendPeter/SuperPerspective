using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class askBoxScript : MonoBehaviour {

	//Visualization variables
	public GameObject textStartPoint; //Tells where our text will begin generating.
    public convoNode currentNode; //This is the node we are currently pulling info from
    public textBoxScript textBoxPartner; //Our askBox's textbox partner to switch between
    //public playerChar player;

    public string question;//The question we ponder the the answer to.
	public string[] choiceArray; //Holds the strings we want to display in the textbox.

   private char[] charArray; //Holds our strings, converted to characters

	public int lineLength = 25; //Number of characters per line
	public float kerning = 1f;//Space between characters
	public float vSpace = 6f;//Space between lines

	//Display Speed variables
	public float textSpeed = 15f;
	public float endLineBlinkSpeed = 1f;
	private float resetTextSpeed;
	private float tinyTimer = 1f;
	private float resetTimer;

	//Keeps track of our cursor so we know where we are in the string.
	private int horiIndex = 0;
	private int vertIndex = 0;
	private int progressIndex = 0;//We use this to check which question we're currently drawing out

	public int choiceID = 0;//How we publically track what choice we're making. This controls where the cursor will be as well.

	[SerializeField] private Text[] textArray;//An array of Text objects spaced evenly. Will eventually be filled with a dynamic amount of choices for us to choose from.

	//Determines whether or not we should show the blinking line ending.
	private bool showEndLine = false;
	//Determines whether or not we have finished displaying the line
	private bool isFinishedDisplaying = false;
	//Determine whether or not we should be able to see the textbox at all.
	public bool showBox = false;
	//The textbox we want to edit.
	public UnityEngine.UI.Image textbox;

	public Text text;//Text that we use to display our cursor
    public Text qText;//Text which we use to display our actual QUESTION

	// Use this for initialization
	void Start () {
		resetTimer = tinyTimer;
		resetTextSpeed = textSpeed;
		init();
	}

	// Update is called once per frame
	void Update () {
		showTextboxCheck();
		printLoop();
		interactLoop();
	}

	public void init()
	{
        //player = (playerChar)GameObject.FindObjectOfType(typeof(playerChar));//DYNAMICALLY GET OUR CHARACTER ON INIT!
        if (currentNode.myType != convoNode.nodeType.textOnly)
        {
            progressIndex = 0;
            choiceID = 0;
		    text.text = "";
            qText.text = currentNode.question;
            choiceArray = currentNode.endQuestionArray;
		    tinyTimer = resetTimer;
		    textSpeed = resetTextSpeed;
		    charArray = new char[choiceArray[progressIndex].Length];//We get the length of the string in our current conversationPoint.
		    isFinishedDisplaying = false;
		    //prepStrings();
		    horiIndex = 0;
		    vertIndex = 0;

		    textArray = new Text[choiceArray.Length];
            charArray = choiceArray[progressIndex].ToCharArray();

            //Our basic Text is used as a cursor. The rest of the texts are stored in the textArray

            for (int i = 0; i < choiceArray.Length; i++)//Here we instantiate ALL of the new text objects
		    {
			    Text temp = (Text)Instantiate(text, textStartPoint.transform.position, transform.rotation);
			    //temp.text = "TEST";
			    temp.transform.SetParent(textStartPoint.transform, false);
			    temp.alignment = TextAnchor.MiddleLeft;
			    temp.transform.localPosition = new Vector3(0, 0 - (vSpace * i), 0);
			    textArray[i] = temp;
		    }
        }
    }

    //Dynamic function to determine whether or not the askbox should be showing based on a variable.
	public void showTextboxCheck()
	{
		if (!showBox)
		{
			disableBox();
		}
		else
		{
			enableBox();
		}
	}

	public void enableBox()//This enables the textbox so you can see it. It also resets the textbox.
	{

		textbox.enabled = true;
		text.enabled = true;
        qText.enabled = true;

        for (int i = 0; i < choiceArray.Length; i++)//Here we instantiate ALL of the new text objects
        {
            textArray[i].enabled = true;
        }
    }

	public void disableBox()//This disables the textbox so that you can't see it.
	{
		textbox.enabled = false;
		text.enabled = false;
        qText.enabled = false;

        deleteText();
    }

    public void activate(convoNode q)
    {
        currentNode = q;
        question = currentNode.question;
        choiceArray = q.endQuestionArray;
        init();
        enableBox();
        showBox = true;
    }

	//This method acts as an update loop for when the text box should be displaying normal conversation text.
	public void printLoop()
	{
		if (showBox)
		{//If we're even active...
			tinyTimer -= Time.deltaTime * textSpeed; //Run the timer.

			//Every time we time down to zero, do the thing.
			if (tinyTimer <= 0)
			{
				if (progressIndex < choiceArray.Length)//This allows us to increment our texts properly.
				{
					if (horiIndex < charArray.Length)//If we haven't filled out to the end of our singular line...
					{
						charArray = choiceArray[progressIndex].ToCharArray();
						textSpeed = resetTextSpeed;
						//Create a character at the horizontal length
						textArray[progressIndex].text = textArray[progressIndex].text + charArray[horiIndex];//This controls which actual TEXT object we're writing to.
						horiIndex++;
						if (horiIndex % lineLength == 0 && horiIndex != charArray.Length)//Check if we've hit the line length by checking if your horiIndex is divisible by our linelength
						{
							switch (charArray[horiIndex])//Special cases for line breaks if we hit the line length with our one-line responses....
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
						}
						else if(horiIndex >= charArray.Length)//If we've finished printing out one choice, reset and go to the next choice.
						{
							horiIndex = 0;
							vertIndex = 0;
							progressIndex++;
						}
					}
				}
				else//We only arrive here AFTER we've printed out all of our options.
				{
					isFinishedDisplaying = true;
				}

				tinyTimer = resetTimer;
			}
		}
	}

	public void makeChoice()
	{
		isFinishedDisplaying = false;
        //First we check if what our answer even corresponds to even exists.
        if (currentNode.hasNext && currentNode.nextNodeArray[choiceID] != null)
        {
            switch (currentNode.myType)
            {

                case convoNode.nodeType.textOnly://Switch to Text Mode
                    textBoxPartner.startConvo(currentNode.nextNodeArray[choiceID]);
                    disableBox();
                    break;
                case convoNode.nodeType.both://Switch to Text Mode
                    textBoxPartner.startConvo(currentNode.nextNodeArray[choiceID]);
                    disableBox();
                    break;

                case convoNode.nodeType.question://Load up and Re-init!
                    currentNode = currentNode.nextNodeArray[choiceID];
                    init();
                    break;

                default:
                    break;
            }
        }
        else if (currentNode.endEventArray[choiceID] != null)
        {
            currentNode.endEventArray[choiceID].eventTrigger();
            disableBox();
            showBox = false;
        }
        else {
            disableBox();
            showBox = false;
        }
	}

	public void deleteText(){

        if (!(textArray == null))
        {
            for (int i = 0; i < textArray.Length; i++)
            {
                Destroy(textArray[i].gameObject);
            }
            textArray = null;
        }
	}

	public void interactLoop()
	{
		if (isFinishedDisplaying) { //If it's finshed displaying, that means it's time to MAKE A CHOICE.
			text.alignment = TextAnchor.MiddleRight;
			text.transform.position = new Vector3(textArray[choiceID].transform.position.x - 1, textArray[choiceID].transform.position.y, 0);
			text.text = "○";

			if (Input.GetKeyUp(KeyCode.Space))
			{
				makeChoice();
			}

			if (Input.GetKeyUp(KeyCode.UpArrow) && choiceID != 0)
			{
				choiceID--;
			}
			if (Input.GetKeyUp(KeyCode.DownArrow) && choiceID < choiceArray.Length-1)
			{
				choiceID++;
			}
		}
	}

    public void prepStrings()
    {

            //Convert all of the strings of our current conversationPoint to characters
            charArray = choiceArray[choiceID].ToCharArray();
            horiIndex = 0;
            vertIndex = 0;

    }

}
