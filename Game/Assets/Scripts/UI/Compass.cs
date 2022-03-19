using UnityEngine;
using UnityEngine.UI;

/* created by: SWT-P_WS_21/22 */

/// <summary>
/// This script changes the direction of the Compass, based on a Transform Objects-Movement.
/// </summary>
public class Compass : MonoBehaviour
{
    /// <summary>
    /// sprite of the compass directions
    /// </summary>
    public RawImage compassImage;

    /// <summary>
    /// the players transformation info
    /// </summary>
    public Transform player;

    /// <summary>
    /// changes the image of the compass based on the direction the playerController is looking.
    /// </summary>
    private void Update()
    {
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);
    }
}
