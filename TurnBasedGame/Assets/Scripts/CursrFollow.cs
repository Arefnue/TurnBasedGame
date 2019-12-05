using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursrFollow : MonoBehaviour
{
    private void Start() {
        
        Cursor.visible = false;
    }
   
    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
    }
}
