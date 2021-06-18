using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

using System.Reflection;


public class BotConfig : MonoBehaviour
{

    [System.Serializable]
    public class Reward
    {
        public GameObject entity;
        public int value;
        public bool isFinish;
    }

    [System.Serializable]
    public class DiscreteAction
    {
        public string name; 
        public int size;
    }

    [System.Serializable]
    public class ContiniousAction
    {
        public string name; 
    }


    public GameObject Hero;
    public Reward[] rewards;
    public DiscreteAction[] discreteActions;
    public ContiniousAction[] continiousActions;

    public bool CameraGrayscale = true;
    private int NumStackedVectorObservations = 1;
    private int DecisionPeriod = 20;
    private int CameraWidth = 84;
    private int CameraHeight = 84;
    private int CameraObservationStacks = 1;

    

    void  Start(){
        Debug.Log("BotConfig: Start");

        BehaviorParameters bp = Hero.AddComponent(typeof(BehaviorParameters)) as BehaviorParameters;
        bp.BehaviorName = "DefaultBehaviour";
        bp.BehaviorType = BehaviorType.Default;
        // bp.BehaviorType = BehaviorType.HeuristicOnly;

        BrainParameters brp = new BrainParameters();
        brp.NumStackedVectorObservations=NumStackedVectorObservations;
        int[] discreteSizes = new int[discreteActions.Length];
        for (int i=0; i<discreteActions.Length; i++)
        {
            discreteSizes[i] = discreteActions[i].size;   
        }
        

        ActionSpec spec = new ActionSpec(continiousActions.Length, discreteSizes);
        brp.ActionSpec = spec;
        PropertyInfo nameField = typeof (BehaviorParameters).GetProperty("BrainParameters");
        nameField.SetValue (bp, brp);

        PawaBot bot = Hero.AddComponent(typeof(PawaBot)) as PawaBot;
        bot.config = this;

        DecisionRequester dr = Hero.AddComponent(typeof(DecisionRequester)) as DecisionRequester;
        dr.DecisionPeriod = DecisionPeriod;

        CameraSensorComponent cs = Hero.AddComponent<CameraSensorComponent>();
        cs.Camera = Camera.main;
        cs.Width = CameraWidth;
        cs.Height = CameraHeight;
        cs.Grayscale = CameraGrayscale;
        cs.ObservationStacks = CameraObservationStacks;
    }
}
