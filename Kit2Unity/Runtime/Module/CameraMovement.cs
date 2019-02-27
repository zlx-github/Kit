// ----------------------------------------------------------------------------------------------------
// Copyright © Guo jin ming. All rights reserved.
// Homepage: https://kylin.app/
// E-Mail: kevin@kylin.app
// ----------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System;

public class CameraMovement : MonoBehaviour
{

    //摄像机跟踪速度
    public float smooth = 1.5f;

    public  Transform player;
    private Vector3 relCameraPos;
    private float relCameraPosMag;
    private Vector3 newPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        relCameraPos = transform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f;
    }

    void FixedUpdate()
    {
        //初始位置
        Vector3 standardPos = player.position + relCameraPos;
        //俯视位置
        Vector3 abovePos = player.position + Vector3.up * relCameraPosMag;
        Vector3[] checkPoints = new Vector3[1];

        checkPoints[0] = standardPos;
        //checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
        //checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
        //checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);
        //checkPoints[4] = abovePos;

        //for (int i = 0; i < checkPoints.Length; i++)
        //{
        //    if (ViewingPosCheck(checkPoints[i]))
        //        break;
        //}
        transform.position = Vector3.Lerp(transform.position, standardPos, smooth * Time.deltaTime);
     //   SmoothLookAt();

    }

    void SmoothLookAt()
    {
        Vector3 relPlayerPosition = player.position - transform.position;

        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }

    //bool ViewingPosCheck(Vector3 checkPos)
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(checkPos, player.position - checkPos, out hit, relCameraPosMag))
    //        if (hit.transform != player)
    //            return false;

    //    newPos = checkPos;
    //    return true;
    //}

}
