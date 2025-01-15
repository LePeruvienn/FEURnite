using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using ExitGames.Client.Photon.StructWrapping;

public class DeleteItemDup : MonoBehaviour
{
    [NonSerialized] private NetworkRunner _runner; // Prevent _runner from being serialized

    void Start()
    {
        if (_runner == null)
        {
            _runner = FindObjectOfType<NetworkRunner>();
            Debug.Log("NetworkRunner n'est pas trouv� dans la sc�ne !");
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    void OnTriggerEnter(UnityEngine.Collider other)
    {
        NetworkObject net = other.GetComponent<NetworkObject>();
        Debug.Log(net + "Has Spawn");
        _runner.Despawn(net);
    }
}
