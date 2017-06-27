using UnityEngine;
using System.Collections;

public class TabManager : MonoBehaviour {

	public GameObject[] Tabs;
	RectTransform[] TabsContent;
	public int startingTab;
	public RectTransform tabSelectedBG;
	public float proximitySesitivity = 0.1f;
	public float tabAnimSpeed = 0.1f;
	public float outsideOffset; //TODO calculate that automaticaly

	public const int trainingTabNum = 1,
					 balanceTabNum = 2,
					 difficultyTabNum=0;

	[HideInInspector] public static int currentTab , prevTab;
	Vector3 outPosLeft, outPosRight;
	Vector3 ContentZeroPos;


	void Start () {
		currentTab = startingTab;
		prevTab = 0;
		outPosLeft = new Vector3(-1*outsideOffset,Tabs[0].GetComponentsInChildren<Transform>()[2].position.y); //sets outside position on the left side of the screen
		outPosRight = new Vector3(outsideOffset,Tabs[0].GetComponentsInChildren<Transform>()[2].position.y); //sets outside position on the right side of the screen
		ContentZeroPos = gameObject.GetComponentsInChildren<Transform>()[3].transform.position; //im not actually sure why its 3 here but that how it works
		TabsContent = new RectTransform[Tabs.Length];

		for (int i=0;i<Tabs.Length;i++) {
			foreach(RectTransform child in Tabs[i].GetComponentsInChildren<RectTransform>()){
             if(child.CompareTag("tabContent"))
				TabsContent[i] = child;
		    }
			if (i<currentTab)
				TabsContent[i].transform.position = outPosLeft;
			else if (i>currentTab)
				TabsContent[i].transform.position = outPosRight;
		}

		tabSelectedBG.transform.position = Tabs[currentTab].GetComponent<Transform>().position;

	}

	void Update () {
		if ((tabSelectedBG.position-Tabs[currentTab].GetComponent<Transform>().position).magnitude > proximitySesitivity) //if tab havent reacehd destination - move new selected tab to its new location (selected tab) 
			if (currentTab!=difficultyTabNum) //the difficulty tab is invisible (we only use its content)
				tabSelectedBG.position = Vector3.Lerp(tabSelectedBG.position, Tabs[currentTab].GetComponent<Transform>().position , tabAnimSpeed);
		if ((TabsContent[currentTab].transform.position-ContentZeroPos).magnitude > proximitySesitivity) //if content havent reacehd destination - move new current Tab content to center
			TabsContent[currentTab].transform.position = Vector3.Lerp(TabsContent[currentTab].transform.position, ContentZeroPos , tabAnimSpeed);
		if (((TabsContent[currentTab].transform.position-outPosLeft).magnitude > proximitySesitivity) && ((TabsContent[currentTab].transform.position-outPosRight).magnitude > proximitySesitivity)) //if content havent reacehd destination - move old current Tab to the side
			TabsContent[prevTab].transform.position = Vector3.Lerp(TabsContent[prevTab].transform.position, (currentTab > prevTab) ? outPosLeft : outPosRight , tabAnimSpeed);

		//if ((tabSelectedBG.sizeDelta - TabsContent[currentTab].sizeDelta).magnitude > proximitySesitivity )
			tabSelectedBG.sizeDelta = Vector2.Lerp(tabSelectedBG.sizeDelta, Tabs[currentTab].GetComponents<RectTransform>()[0].sizeDelta,tabAnimSpeed);
	}

	public void tabPress(int tabNum) {
		if (tabNum!=currentTab) {
			prevTab = currentTab;
			currentTab = tabNum;
			TabsContent[currentTab].transform.position = (prevTab>currentTab) ? outPosLeft : outPosRight; //fix a bug where sometime the ingoing tab comes from the wrong side
			if (tabNum==trainingTabNum)
				MainMenuManager.currentGameType = MainMenuManager.gameType.training;
			else if (tabNum==balanceTabNum)
				MainMenuManager.currentGameType = MainMenuManager.gameType.balance;
		}
	}

	public void chooseDifficulty() {
		//TODO title
        Tabs[difficultyTabNum].GetComponentInChildren<ChooseDifficultyBtn>().ResetBtns();
        if (Tabs[difficultyTabNum].GetComponentInChildren<StartTrainingBtn>() !=null)
            Tabs[difficultyTabNum].GetComponentInChildren<StartTrainingBtn>().ResetBtns();
		tabPress(difficultyTabNum);
	}
}
