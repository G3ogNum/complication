using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoreCalcular : MonoBehaviour
{
    public static ScoreCalcular instance;

    public int CTscore = 0;
    public int Tscore = 0;

    private void Start()
    {
        instance = this;
    }

    public delegate void UpdateTeamScore();
    public UpdateTeamScore uts;

    [PunRPC]
    void CTscorePlus()
    {
        CTscore++;
        uts();
    }
    [PunRPC]
    void TscorePlus()
    {
        Tscore++;
        uts();
    }
}
