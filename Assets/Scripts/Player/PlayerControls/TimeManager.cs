/*
using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public float slowdownFactor = 0.005f;
	public float Matrix_Time = 0;
	public float Time_Original = 0;
	public bool Matrix = false;
    public bool Matrix_Instant_Time = false;
	public int Matrix_State = 0;

	void Update ()
	{
		if (Matrix_Time == 0) Matrix_Time = Time.timeScale;
		if (Time_Original == 0) Time_Original = Time.timeScale;
		if (Input.GetButtonDown("Matrix")) Matrix = !Matrix;

        if (Matrix)
        {
            if (Matrix_Instant_Time)
            {
                if (Matrix_State == 0) DoSlowmotion();
            }
            else
            {
                if (Matrix_State == 0 || Matrix_State == 3) DoSlowDown();
                if (Matrix_State == 1) DoSlowmotion();
            }
        }
        else
        {
            if (Matrix_Instant_Time)
            {
                Matrix_Time = Time_Original;
                Time.timeScale = Matrix_Time;
                Matrix_State = 0;
            }
            else DoSpeedUp();
        }
		Time.fixedDeltaTime = Time.timeScale * .02f;
	}

	public void DoSlowmotion ()
	{
		Matrix_State = 2;
		Matrix_Time = slowdownFactor;
		Time.timeScale = Matrix_Time;
	}
	
	public void DoSpeedUp()
    {
		Matrix_State = 3;
		Matrix_Time += (1f / 1f) * Time.unscaledDeltaTime;
		Matrix_Time = Mathf.Clamp(Matrix_Time, 0f, 1f);
		Time.timeScale = Matrix_Time;
		if (Matrix_Time >= Time_Original) Matrix_State = 0;
	}
	
	public void DoSlowDown()
    {
		Matrix_Time -= (1f / 1f) * Time.unscaledDeltaTime;
		Matrix_Time = Mathf.Clamp(Matrix_Time, 0f, 1f);
		Time.timeScale = Matrix_Time;
		if (Matrix_Time < slowdownFactor) Matrix_State = 1;
	}
}
*/
