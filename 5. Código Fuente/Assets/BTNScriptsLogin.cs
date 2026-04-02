using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BTNScriptsLogin : MonoBehaviour
{
    public GameObject UserId;
    public GameObject UserName;
    public Dropdown myDropdown;
    public GameObject PanelCrearUsuario;
    public static string selectedUserName;
    public static string selectedUserId;
    List<string> users;
    List<string> ids;

    // Start is called before the first frame update
    void Start()
    {
        PopulateDropdown(myDropdown);
    }

    public void BTN_Comenzar()
    {
        selectedUserName = users[myDropdown.value];
        selectedUserId = ids[myDropdown.value];
        SceneManager.LoadScene("MainMenuES");
    }

    public void BTN_salir()
    {
        Application.Quit();
    }


    void PopulateDropdown(Dropdown dropdown)
    {
        DataStorage db = new DataStorage();
        dropdown.ClearOptions();
        users = db.getUsers();
        ids = db.getIds();
        dropdown.AddOptions(users);
        dropdown.value = users.Count-1;

    }

    public void BTN_CrearUsuario()
    {
        DataStorage db = new DataStorage();
        int Id = int.Parse(UserId.GetComponent<UnityEngine.UI.Text>().text);
        string Name = UserName.GetComponent<UnityEngine.UI.Text>().text;
        db.InsertarUsuario(Id,Name);
        PopulateDropdown(myDropdown);
        PanelCrearUsuario.SetActive(false);
    }

    public void BTN_Registrarse()
    {
        PanelCrearUsuario.SetActive(true);
    }

}
