using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement;

public class PawaBot : Agent
{
    public BotConfig config;
    private string behaviourType;
    private float prevTimestamp;
    private float timer;

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		Debug.Log("Bot:Heuristic: ");
        config.Hero.SendMessage("jump", Convert.ToInt32(Input.GetButton("jump")));
    }

	void Start()
	{
		Debug.Log("Bot:Start: ");

		behaviourType = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorType.ToString();

	}

	void Update()
    {
        Debug.Log("Bot:Update: ");
        maybeAddRewardPerTime();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log("Bot:OnActionReceived: ");

		if (behaviourType != "HeuristicOnly")
		{
            for (int i=0; i< actionBuffers.DiscreteActions.Length; i++)
            {
                int action = actionBuffers.DiscreteActions[i];
                string methodName = config.discreteActions[i].name;
                config.Hero.SendMessage(methodName, action);
            }

            for (int i=0; i< actionBuffers.ContinuousActions.Length; i++)
            {
                float value = actionBuffers.ContinuousActions[i];
                string methodName = config.continiousActions[i].name;
                config.Hero.SendMessage(methodName, value);
            }
        }
    }


//todo 3d or on CollisionEnter

    public void OnCollisionEnter2D(Collision2D other)
    {
       handleCollision (other.gameObject.tag);
    }

    public void OnTriggerEnter2D(Collider2D other)
	{
		handleCollision (other.gameObject.tag);
	}


    void handleCollision(string otherTag){
        foreach (BotConfig.Reward reward in config.rewards)
        {
            if (reward.entity.tag == otherTag)
            {
                Debug.Log("handleCollision: Reward value " + reward.value + " Collision type " + otherTag);

                AddReward(reward.value);

                if (reward.isFinish)
                {
                    EndEpisode();
                    restartCurrentLevel();
                }
            }
        }
    }

    void startRandomLevel()
    {
        int sceneIdx = UnityEngine.Random.Range(0, SceneManager.sceneCountInBuildSettings);
        SceneManager.LoadScene (sceneIdx);
    }

    void restartCurrentLevel(){
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name); 
    }

    void maybeAddRewardPerTime(){
            // add negative reward per each second of game
        timer += Time.deltaTime;
        float timeDelta = timer - prevTimestamp;
        if ( timeDelta >= 1)    
        {
            AddReward(-timeDelta);
            prevTimestamp = timer;
        }
    }
}
