using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using TMPro;

public class Builders : Agent
{
    public enum BuilderTeam
    {
        Builder1 = 1,

    }
    public enum BuilderTeamPosition
    {
        generic,
    }


    [HideInInspector]
    public Rigidbody builder_rigid_body;

    [HideInInspector]
    public BuilderTeam builderTeam;
    int b_PlayerIndex;
    public PlayArea playArea;
    public BuilderTeamPosition builderTeamPosition;
    BehaviorParameters b_BehaviorParameters;
    GameSettings b_gameSettings;
    public int total_coins_touched = 0;
    float b_Existential;
    float b_LateralSpeed;
    float b_ForwardSpeed;
    Vector3 b_Transform;
    Agents agents;
    bool m_Frozen;

    public float builder_moveSpeed = 10f;

    void Start()
    {


    }

    public override void Initialize()
    {
        builder_rigid_body = GetComponent<Rigidbody>();
        b_Existential = 1f / MaxStep;
        b_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();


        // SETTING THE INITIAL SARTUP POSITION GOR BUILDER 
        if (b_BehaviorParameters.TeamId == (int)BuilderTeam.Builder1)
        {
            builderTeam = BuilderTeam.Builder1;
            b_Transform = new Vector3(transform.position.x - 4f, .5f, transform.position.z);
        }




        //builder position in this scene

        if (builderTeamPosition == BuilderTeamPosition.generic)
        {
            b_LateralSpeed = 4.0f;
            b_ForwardSpeed = 4.0f;
        }


        // Connecting to the game settings  and getting rigid body 
        b_gameSettings = FindObjectOfType<GameSettings>();
        builder_rigid_body.maxAngularVelocity = 500;


       
        var builder_State = new Builder_State
        {
            builder_rigid_body = builder_rigid_body,
            startingPosition = transform.position,
            builders_Script = this,
            total_coins_touched = total_coins_touched,
            m_Frozen = m_Frozen,

        };
        playArea.builder_States.Add(builder_State);
        b_PlayerIndex = playArea.builder_States.IndexOf(builder_State);
        builder_State.builderIndex = b_PlayerIndex;
    }

    public override void OnEpisodeBegin()
    {

        Debug.Log(" EPISODE BEGIN BUILDER");


        total_coins_touched = 0;
        Unfreeze();
        ResetEnv();

    }


    /*      public void placeAgent()
        {
            builder_rigid_body = GetComponent<Rigidbody>();
            // Move the agent to a new spot
            this.transform.localPosition = new Vector3(0f,
                                               1f,
                                               0f);
        }
    */




    /// <summary>
    /// Picks up gold and adds it to wallet
    /// </summary>
    /// <param name="gold"></param>
    private void EatGold(GameObject gold)
    {

        RemoveSpecificGold(gold);

    }

    public void RemoveSpecificGold(GameObject goldPrefab)
    {

        if (goldPrefab.gameObject.CompareTag("coin"))
        {

            playArea.goldList.Remove(goldPrefab);
            Destroy(goldPrefab);

            //THIS part
            total_coins_touched += 1;

        }
    }

    public void Freeze()
    {

        m_Frozen = true;
    }
    public void Unfreeze()
    {
        m_Frozen = false;
    }


    public void HitByBall()
    {
        if (!m_Frozen)
        {
            Freeze();
            playArea.buildersOnField -= 1;
            Debug.Log("minusing the builders on field");
            AddReward(-0.5f);
            if (total_coins_touched > 0)
            {
                playArea.SpawnGold(total_coins_touched);

            }
        }
    }

    public void PickedUpGold()
    {
        AddReward(+1 / playArea.total_coins);
        // ADD FUNCTION TO SPAWN PICKEDUP GOLD
    }





    /// <summary>
    /// When the agent collides with something, take action
    /// </summary>
    /// <param name="collision">The collision info</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("coin"))
        {
            //touching gold
            EatGold(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("ball"))
        {
            HitByBall();
            Debug.Log("builder is  now hit by ball on collision");
        }

    }


    /// <summary>
    /// When the agent collides with something, take action
    /// </summary>
    /// <param name="collision">The collision info</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("target"))
        {



            if (total_coins_touched > 0)
            {
                playArea.coins_In_Target += total_coins_touched;

                AddReward(+1 * total_coins_touched);


                Debug.Log(" coins in basket = " + playArea.coins_In_Target);
                // Spawn green Alert 
                GameObject greenAlert = Instantiate<GameObject>(playArea.greenAlertPrefab);
                greenAlert.transform.parent = transform.parent;
                greenAlert.transform.position = playArea.Target.transform.position;
                Destroy(greenAlert, 6f);

                total_coins_touched = 0;

            }

            if (total_coins_touched == 0)
            {

                // Spawn red Alert 
                GameObject redAlert = Instantiate<GameObject>(playArea.redAlertPrefab);
                redAlert.transform.parent = transform.parent;
                redAlert.transform.position = playArea.Target.transform.position;
                Destroy(redAlert, 4f);
            }

            playArea.BasketCheck(Agents.Team.Chaser, Builders.BuilderTeam.Builder1);
        }


        /*if (other.gameObject.CompareTag("ball"))
        {
            HitByBall();
            Debug.Log("builder is  now hit by ball");
        }*/
    }




    // to feed into the neural network 
    public override void CollectObservations(VectorSensor sensor)
    {
        // coin and Agent positions

        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(builder_rigid_body.velocity.x);
        sensor.AddObservation(builder_rigid_body.velocity.z);
    }




    /// <summary>
    /// Getting the AI to control the scene 
    /// </summary>
    /// <param name="actionsOut"></param>
    public void MoveAgent(float[] actionsOut)
    {

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        if (!m_Frozen)
        {

            var forwardAxis = (int)actionsOut[0];
            var rightAxis = (int)actionsOut[1];
            var rotateAxis = (int)actionsOut[2];

            switch (forwardAxis)
            {
                case 1:
                    dirToGo = transform.forward * b_ForwardSpeed;
                    break;
                case 2:
                    dirToGo = transform.forward * -b_ForwardSpeed;
                    break;
            }

            switch (rightAxis)
            {
                case 1:
                    dirToGo = transform.right * b_LateralSpeed;
                    break;
                case 2:
                    dirToGo = transform.right * -b_LateralSpeed;
                    break;
            }

            switch (rotateAxis)
            {
                case 1:
                    rotateDir = transform.up * -1f;
                    break;
                case 2:
                    rotateDir = transform.up * 1f;
                    break;
            }

            transform.Rotate(rotateDir, Time.deltaTime * 100f);
            builder_rigid_body.AddForce(dirToGo * builder_moveSpeed,
                ForceMode.VelocityChange);

        }

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (!m_Frozen)
        {
            MoveAgent(vectorAction);
        }

        /*
                if (playArea.coins_In_Target >= playArea.total_coins)
                {
                    Debug.Log("All COins in Target");
               //     playArea.EndChaser();
                    EndEpisode();
                }*/

        /* if(playArea.buildersOnField == 0)
         {
             Debug.Log("all builders Dead");
         //    playArea.EndChaser();
             EndEpisode();
         }*/

        /* if (playArea.RestartEverything == true)
         {
             Debug.Log("RESTARTING ERRRRRRRRRRRRRRTANG");
             EndEpisode();
         }*/
        /*
                // Penalty given each step to encourage agent to finish task quickly.
                AddReward(-1f / MaxStep);*/


    }


    /// <summary>
    /// manual controls of  the scene 
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        Array.Clear(actionsOut, 0, actionsOut.Length);
        //forward
        if (Input.GetKey(KeyCode.I))
        {
            actionsOut[0] = 1f;
        }
        if (Input.GetKey(KeyCode.K))
        {
            actionsOut[0] = 2f;
        }
        //rotate
        if (Input.GetKey(KeyCode.J))
        {
            actionsOut[2] = 1f;
        }
        if (Input.GetKey(KeyCode.L))
        {
            actionsOut[2] = 2f;
        }
        //right
        if (Input.GetKey(KeyCode.O))
        {
            actionsOut[1] = 1f;
        }
        if (Input.GetKey(KeyCode.U))
        {
            actionsOut[1] = 2f;
        }

    }

    public void ResetEnv()
    {
        Unfreeze();
        playArea.ResetBall();
    }

}