using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doodle.Game;

public class RolelessUI : MonoBehaviour
{
	public Canvas mCanvas;

	public Text mPurpleScore_Text;
	public Text mYellowScore_Text;
	public Text mTime_Text;

	public void ToggleVisibility(bool isOn)
	{
		mCanvas.gameObject.SetActive(isOn);
	}

	public void SetScore(Team team, int score)
	{
		if (team == Team.Purple)
			mPurpleScore_Text.text = score.ToString();
		else 
			mYellowScore_Text.text = score.ToString();
	}

	public void SetTime(float remainingTime)
	{
		int minutes = (int)remainingTime / 60;
		int seconds = (int)remainingTime % 60;

		if (seconds < 10)
			mTime_Text.text = string.Format("{0}:0{1}", minutes, seconds);
		else
			mTime_Text.text = string.Format("{0}:{1}", minutes, seconds);

		if (minutes == 0 && seconds <= 10)
			mTime_Text.color = Color.red;
		else if (minutes == 0 && seconds >= 11 && seconds <= 30)
			mTime_Text.color = Color.yellow;
		else
			mTime_Text.color = Color.white;
	}
}
