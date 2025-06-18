using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileDragAndDrop : MonoBehaviour
{
    List<string> log = new List<string>();
    void OnEnable ()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;

        Application.targetFrameRate = 30;
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.Space))
        {
            List<string> files = new()
            {
                "D:/Projects/UniFiles/Assets/Scripts/123.txt"
            };
            Debug.Log("Invoke drag and drop");

            UnityDragAndDropHook.StartDragFiles(files);

            Debug.Log("Invoked successfully");
        }
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the
        // mouse position within the window where the files has been dropped.
        string str = "Dropped " + aFiles.Count + " files at: " + aPos + "/n/t" +
                     aFiles.Aggregate((a, b) => a + "/n/t" + b);
        Debug.Log(str);
        log.Add(str);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("clear log"))
            log.Clear();
        foreach (var s in log)
            GUILayout.Label(s);
    }
}