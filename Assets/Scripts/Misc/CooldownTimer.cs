// Code inspired from this article: https://www.jonathanyu.xyz/2020/01/21/unity-cooldown-timer-script-tutorial/
using UnityEngine;

[System.Serializable]
public class CooldownTimer
{
    public float cooldownLength = 1f;
    private float cooldownCompleteTime;
    public bool cooldownComplete => Time.time > cooldownCompleteTime;

    public void ResetCooldown() 
    {
        cooldownCompleteTime = Time.time + cooldownLength;
    }

    // Implement this class using [SerializeField] protected CooldownTimer timerNameOfYourChoosing
}
