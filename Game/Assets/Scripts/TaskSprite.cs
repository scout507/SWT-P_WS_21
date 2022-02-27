// using System.Collections;
// using System.Collections.Generic;
// using TaskManager;
// using UnityEngine;

// public class TaskSprite : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         List<string[]> info = TaskManager.GetTaskInfo();
//         for (int i = 0; i < info.Count; i++)
//         {
//             GameObject newCanvas = new GameObject("Canvas");
//             Canvas c = newCanvas.AddComponent<Canvas>();
//             c.renderMode = RenderMode.ScreenSpaceOverlay;
//             newCanvas.AddComponent<CanvasScaler>();
//             newCanvas.AddComponent<GraphicRaycaster>();
//             GameObject panel = new GameObject("Panel");
//             panel.AddComponent<CanvasRenderer>();
//             Image i = panel.AddComponent<Image>();
//             i.color = Color.red;
//             panel.transform.SetParent(newCanvas.transform, false);
//         }
//     }
// }
