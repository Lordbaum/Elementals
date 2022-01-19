using System;
using Unity.Netcode;
using UnityEngine;

public class RigidibodySync : NetworkBehaviour

{
    public NetworkVariable<Vector3> velocity, angularVelocity, centerOfMass;
    public NetworkVariable<Vector3> position, rotation;
    public NetworkVariable<float> angularDrag, drag;
    public NetworkVariable<bool> useGravity;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (RigidbodyNeedSync(rigidbody))
        {
            SendRigidbodyInformationServerRpc();
        }
    }

    private bool RigidbodyNeedSync(Rigidbody rb)
    {
        //checks if the Angular and Normal Velocity is equals to the Networked data
        bool velocityEquality = rb.velocity == velocity.Value;
        bool angularVelocityEquality = rb.angularVelocity == angularVelocity.Value;

        //if they are true it returns false because than the Rb didn't changed
        bool isManger;
        try
        {
            if (transform.parent.parent.parent.TryGetComponent(out NetworkObject netobj))
            {
                isManger = NetworkObject.IsLocalPlayer || netobj.IsLocalPlayer || NetworkManager.Singleton.IsServer ||
                           NetworkManager.Singleton.IsHost;
            }
            else isManger = NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;
        }
        catch (NullReferenceException e)
        {
            isManger = NetworkManager.Singleton.IsServer ||
                       NetworkManager.Singleton.IsHost || NetworkObject.IsLocalPlayer;
        }

        return !(velocityEquality && angularVelocityEquality) && isManger;
    }

    [ServerRpc]
    private void SendRigidbodyInformationServerRpc()
    {
        //sync Vector3
        velocity.Value = rigidbody.velocity;
        angularVelocity.Value = rigidbody.angularVelocity;
        centerOfMass.Value = rigidbody.centerOfMass;
        //sync Transform
        position.Value = rigidbody.transform.position;
        rotation.Value = rigidbody.transform.eulerAngles;
        //sync float
        angularDrag.Value = rigidbody.angularDrag;
        drag.Value = rigidbody.drag;
        //sync bool
        useGravity.Value = rigidbody.useGravity;

        //ruft ein Client Rpc zum syncronisiren auf
        ApplyRigidbodyInformationClientRpc();
    }

    [ClientRpc]
    private void ApplyRigidbodyInformationClientRpc()
    {
//sync Vector3
        rigidbody.velocity = velocity.Value;
        rigidbody.angularVelocity = angularVelocity.Value;
        rigidbody.centerOfMass = centerOfMass.Value;
//sync Transform
        transform.position = position.Value;
        transform.eulerAngles = rotation.Value;
//sync float
        rigidbody.angularDrag = angularDrag.Value;
        rigidbody.drag = drag.Value;
//sync bool
        rigidbody.useGravity = useGravity.Value;
    }
}