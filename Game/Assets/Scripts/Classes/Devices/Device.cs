using UnityEngine;
using Mirror;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This is the abstract classes for devices. It handles the health and destruction of the devices.
/// </summary>
abstract public class Device : NetworkBehaviour
{
    /// <summary>These are the healtpoints of the device.</summary>
    [SyncVar]
    int health = 30;

    /// <summary>
    /// The method TakeDamage is responsible for suffering damage.
    /// </summary>
    /// <param name="amount">The parameter amount indicates how much damage was sustained.</param>
    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }
        else
        {
            health -= amount;
        }

        if (health <= 0)
        {
            TargetDeath();
        }
    }

    /// <summary>
    /// This methode defines what happens, when the device is destroyed. Different devices behave different in case of destruction.
    /// </summary>
    abstract public void TargetDeath();

    /// <summary>
    /// This is called when a melee weapon hits the Device.
    /// </summary>
    /// <param name="other">The collider of the gameobject which hit this gameobject.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<Melee>())
        {
            other.transform.root.GetComponent<Melee>().meleeHit(gameObject);
        }
    }

    /// <summary>
    /// Destroys the device on all clients
    /// </summary>
    /// <param name="device">Device which should be destroyed</param>
    [ClientRpc]
    public void RpcDestroyDevice(GameObject device)
    {
        Destroy(device);
    }
}
