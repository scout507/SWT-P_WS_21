using UnityEngine;

// created by: SWT-P_WS_21/22

/// <summary>Needed for a spawn point in the scene, indicates in which direction this looks.</summary>
public class PlayerSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// In the editor, draw an outline for the game object and 
    /// a line in which direction it is facing.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}
