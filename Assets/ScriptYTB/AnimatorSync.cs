using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class AnimatorSync : MonoBehaviourPunCallbacks, IPunObservable
{

    public AimIK aimIK;
    public float aimWeight=0f;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)

        {

            stream.SendNext(aimWeight);

        }
        else

        {

            aimWeight = (float)stream.ReceiveNext();

        }
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            aimIK.solver.SetIKPositionWeight(Input.GetKey(KeyCode.W) ?
          Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 0f, Time.deltaTime * 4)
          : Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 1f, Time.deltaTime * 4));
            aimWeight = aimIK.solver.GetIKPositionWeight();
        }
        else
        {
            UpdateLogic();
        }
    }

    void UpdateLogic()
    {
        aimIK.solver.SetIKPositionWeight(aimWeight);

    }
}
