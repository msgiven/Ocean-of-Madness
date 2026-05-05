using System;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    enum ZoneIssue
    {
        None, CannonWaiting, BreachSide, BreachTop
    }

    [SerializeField]
    private ZoneIssue _issue = ZoneIssue.None;

    [Header("Child Gameobjects")]
    [SerializeField]
    private GameObject _cannon;
    [SerializeField]
    private GameObject _holeSide;
    [SerializeField]
    private GameObject _holeTop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // Update is called once per frame
    void Update()
    {
        switch (_issue)
        {
            // case ZoneIssue.None:
            //     break;

            // case ZoneIssue.CannonWaiting:
            //     break;

            // case ZoneIssue.BreachSide:
            //     break;

            // case ZoneIssue.BreachTop:
            //     break;

            default:
                throw new NotImplementedException(
                    "Issue \"" + nameof(_issue) + "\" is not implemented yet");
        }
    }
}
