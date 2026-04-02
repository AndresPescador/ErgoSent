using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScrpts : MonoBehaviour
{
    public GameObject idlePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BTN_debug()
    {
        idlePanel.SetActive(false);
    }

    public void BTN_regresar()
    {
        idlePanel.SetActive(true);
    }

    public void BTN_finalizar()
    {
        SceneManager.LoadScene("MainMenuES");
    }

}
