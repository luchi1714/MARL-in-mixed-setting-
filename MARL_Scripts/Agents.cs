using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System.Runtime.InteropServices;

public class Agents : Agent
{
    // Make notes on Decetable tags 



    public enum Team
    {
        Chaser = 0,

    }
    public enum Position
    {

        shooter,

    }


    [HideInInspector]
    public Team team;
    float m_KickPower;
    int m_PlayerIndex;
    public PlayArea playArea;
    
    float m_BallTouch;
    public Position position;



    const float k_Power = 200f;
    float m_Existential;
    float m_LateralSpeed = 1.0f;
    float m_ForwardSpeed = 1.0f;

    [HideInInspector]
    public float timePenalty;

    [HideInInspector]
    public Rigidbody agentRigidBody;
    GameSettings m_gameSettings;
    BehaviorParameters m_BehaviorParameters;
    Vector3 m_Transform;

    EnvironmentParameters m_ResetParams;



    public override void Initialize()
    {
        m_Existential = 1f / MaxStep;
        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();


        // SETTING THE INITIAL SARTUP POSITION GOR BUILDER 
        if (m_BehaviorParameters.TeamId == (int)Team.Chaser)
        {
            team = Team.Chaser;
            m_Transform = new Vector3(transform.position.x - 4f, .5f, transform.position.z);
        }




        // Connecting to the game settings  and getting rigid body 
        m_gameSettings = FindObjectOfType<GameSettings>();
        agentRigidBody = GetComponent<Rigidbody>();
        agentRigidBody.maxAngularVelocity = 500;


        ///     COME BACK TO THIS !!!!!!!!!!!!!!!!!!!
        var player_State = new Player_State
        {
            agentRigidBody = agentRigidBody,
            startingPosition = transform.position,
            agent_Script = this,

        };
        playArea.player_States.Add(player_State);
        m_PlayerIndex = playArea.player_States.IndexOf(player_State);
        player_State.playersIndex = m_PlayerIndex;

        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }



    /*    STUFF THAT HAS TO DO WITH MOVEMENT 
     *    
     *    
     *          MOVEMENT
     *    
     *    
    */



    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = (int)act[0];
        var rightAxis = (int)act[1];
        var rotateAxis = (int)act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                m_KickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -m_LateralSpeed;
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
        agentRigidBody.AddForce(dirToGo * m_gameSettings.agentRunSpeed,
            ForceMode.VelocityChange);
    }

    public override void OnActionReceived(float[] vectorAction)
    {


        if (position == Position.shooter)
        {
            // Existential penalty for Strikers
            AddReward(-m_Existential);
        }

        if (playArea.buildersOnField > 0)
        {
            MoveAgent(vectorAction);
        }



    }

    public override void Heuristic(float[] actionsOut)
    {
        Array.Clear(actionsOut, 0, actionsOut.Length);
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2f;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[2] = 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[2] = 2f;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            actionsOut[1] = 1f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            actionsOut[1] = 2f;
        }
    }






    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        if (position == Position.shooter)
        {
            force = k_Power;
        }
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(.2f * m_BallTouch);
            var dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }

    /*    public void EndNow()
        {
            EndEpisode();
        }*/

    public override void OnEpisodeBegin()
    {

        timePenalty = 0.005f;
        m_BallTouch = 0.005f;    // m_ResetParams.GetWithDefault("ball_touch", 0);


        Debug.Log(" CHASER starting up");

        transform.position = m_Transform;
        agentRigidBody.velocity = Vector3.zero;
        agentRigidBody.angularVelocity = Vector3.zero;
        SetResetParameters();
    }




    public void SetResetParameters()
    {
        playArea.ResetBall();

    }
}
