using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tanknarok.Menu;
using UnityEngine;

public class UnityScript : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void UnityPluginRequestJs();
    public GameObject goControl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestJs() // вызываем из событий unity
    {
        //UnityPluginRequestJs();
        goControl.GetComponent<MainMenuUI>().ResponseFromJsOk();
    }
}
