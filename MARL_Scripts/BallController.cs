using Unity.Barracuda;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [HideInInspector]
    public PlayArea area1;
    public string builderTag; //will be used to check if collided with builder 
    [HideInInspector]





   
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("builder")) //ball touched purple goal
        {
            area1.HitBuilder(Agents.Team.Chaser, Builders.BuilderTeam.Builder1);
            //area1.buildersOnField -= 1;
/*            Debug.Log(" HIT BUILDER  and minused");
*/              

        }

    }




}