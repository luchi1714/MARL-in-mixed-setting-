using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class Player_State
{
    public int playersIndex;
    [FormerlySerializedAs("agentRigidBody")]
    public Rigidbody agentRigidBody;
    public Vector3 startingPosition;
    public Agents agent_Script;
    public float ballPosReward;

}


[System.Serializable]
public class Builder_State
{
    public int builderIndex;
    [FormerlySerializedAs("builder_rigid_body")]
    public Rigidbody builder_rigid_body;
    public Vector3 startingPosition;
    public Builders builders_Script;
    public int total_coins_touched;
    public bool m_Frozen;
}


public class PlayArea : MonoBehaviour
{

    // STUFF MOSTLY FOR THE CHASER 
    public GameObject ballPrefab;
    [FormerlySerializedAs("ballRigidbody")]
    [HideInInspector]
    public Rigidbody ballRigidbody;
    public GameObject ground;
    public List<Player_State> player_States = new List<Player_State>();
    [HideInInspector]
    public Vector3 ballStartingPosition;
    public GameObject goalTextUI;
    [HideInInspector]
    public bool canResetBall;
    EnvironmentParameters e_ResetParams;
    BallController ballRemote; // SCRIPT TO ADD LATER 



    //  variables that have to do with builder 
    public int total_coins = 7;
    public int coins_In_Target = 0;
    public List<Builder_State> builder_States = new List<Builder_State>();
    bool m_killed;
    public GameObject Target;
    public GameObject greenAlertPrefab;
    public GameObject redAlertPrefab;



    [Tooltip("Prefab of the coin")]
    public GameObject goldPrefab;
    public List<GameObject> goldList;


    // GENERAL STUFF
    public int buildersOnField = 2;
    public int numberOfBuilders = 2;
    // public bool EndForChaser = false;



    private void Awake()
    {

        canResetBall = true;
        if (goalTextUI) { goalTextUI.SetActive(false); }
        ballRigidbody = ballPrefab.GetComponent<Rigidbody>();
        ballRemote = ballPrefab.GetComponent<BallController>();
        ballRemote.area1 = this;
        ballStartingPosition = new Vector3(0, 0.5f, 0);
        e_ResetParams = Academy.Instance.EnvironmentParameters;
        Debug.Log(" Ball position is " + ballPrefab.transform.position);

    }

    IEnumerator ShowGoalUI()
    {
        if (goalTextUI) goalTextUI.SetActive(true);
        yield return new WaitForSeconds(.25f);
        if (goalTextUI) goalTextUI.SetActive(false);
    }



    /// <summary>
    /// called when the ball touches an agent, then makes the apporiate decision 
    /// </summary>
    /// <param name="teamHit"></param>
    /// <param name="builderteamhit"> the team whome the ball touched </param>
    public void HitBuilder(Agents.Team teamHit, Builders.BuilderTeam builderhit)
    {

        foreach (var ps in player_States)
        {
            foreach (var bs in builder_States)
            {
                // Adding a point for the members of the opposing team that were hit out 
                if (ps.agent_Script.team == teamHit)
                {
                    if (bs.builders_Script.builderTeam == builderhit)
                    {

                        if (coins_In_Target >= total_coins)
                        {
                            Debug.Log("All COins in Target");
                            ps.agent_Script.EndEpisode();
                            bs.builders_Script.EndEpisode();
                        }



                        // All Builders Eliminated 
                        else if (buildersOnField <= 0)
                        {
                            Debug.Log("Builders Eliminated");
                            ps.agent_Script.EndEpisode();
                            bs.builders_Script.EndEpisode();

                        }
                        ps.agent_Script.AddReward((0.5f) + ps.agent_Script.timePenalty);
                        Debug.Log("Adding Reward for hitting builder......=  " + buildersOnField);


                    }

                }

            }

        }

    }







    /// <summary>
    ///  Called when the agent touches the basket , checks if the basket is full then takes apporiate step
    /// </summary>
    /// <param name="teamHit"></param>
    /// <param name="builderteamhit"></param>
    public void BasketCheck(Agents.Team teamHit, Builders.BuilderTeam builderhit)
    {

        foreach (var ps in player_States)
        {
            foreach (var bs in builder_States)
            {
                // Adding a point for the members of the opposing team that were hit out 
                if (ps.agent_Script.team == teamHit)
                {
                    if (bs.builders_Script.builderTeam == builderhit)
                    {

                        if (coins_In_Target >= total_coins)
                        {
                            Debug.Log("All COins in DA  BASKETTTTTT");
                            ps.agent_Script.EndEpisode();
                            bs.builders_Script.EndEpisode();
                        }



                        // All Builders Eliminated 
                        else if (buildersOnField <= 0)
                        {
                            Debug.Log("Builders Eliminated");
                            ps.agent_Script.EndEpisode();
                            bs.builders_Script.EndEpisode();

                        }
                        /*                        ps.agent_Script.AddReward((0.5f) + ps.agent_Script.timePenalty);
                                                Debug.Log("Adding Reward for hitting builder......=  " + buildersOnField);
                        */

                    }

                }

            }

        }

    }






    // METHODS  THAT CONCERNS  BUILDER 

    public void SpawnGold(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Spawn and place the Gold
            GameObject goldObject = Instantiate<GameObject>(goldPrefab.gameObject);
            goldObject.transform.position = new Vector3(UnityEngine.Random.Range(-6, 6),
                                           1f,
                                           UnityEngine.Random.Range(-9, 9));
        }
    }






    /// <summary>
    /// Put target on the scene 
    /// </summary>
    public void PlaceTarget()
    {
        Rigidbody target_rb = Target.GetComponent<Rigidbody>();
        target_rb.velocity = Vector3.zero;
        target_rb.angularVelocity = Vector3.zero;
        target_rb.transform.position = new Vector3(-5f,
                                           0.2f,
                                           -11f);
        target_rb.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

    }

    /// <summary>
    /// Remove all gold from the scene 
    /// </summary>
    public void RemoveAllGold()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("coin"));
        goldList = new List<GameObject>();
    }


    /// <summary>
    /// Clear the scene 
    /// </summary>
    /// <param name="objects">All game objects eg coins </param>
    void ClearObjects(GameObject[] objects)
    {
        foreach (var gold in objects)
        {
            Destroy(gold);
        }
    }


    /// <summary>
    /// Resets the ball 
    /// </summary>
    public void ResetBall()
    {
        // Stuff to do with ball 
        Debug.Log(" Reset ball  method ");
        ballPrefab.transform.position = ballStartingPosition;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.transform.localScale = new Vector3(1, 1, 1);



        // Stuff to do with builder 

        RemoveAllGold();
        SpawnGold(total_coins);
        PlaceTarget();
        buildersOnField = numberOfBuilders;
        coins_In_Target = 0;




    }



}

