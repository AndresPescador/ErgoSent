using Azure.Storage.Blobs;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BTN_Scripts : MonoBehaviour
{
    public GameObject BTN_Sendtxt;
    public GameObject statusText;
    public GameObject BTN_Send;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BTN_iniciar()
    {
        SceneManager.LoadScene("ErgoSentMain");
    }

    public void BTN_salir()
    {
        Application.Quit();
    }


    public void BTN_enviarDatos()  
    { 
        BTN_Sendtxt.GetComponent<UnityEngine.UI.Text>().text = "Enviando"; 
        IDbConnection dbcon; 

        //Crear base de datos 
        string connection = "URI=file:" + Application.persistentDataPath + "/My_Database"; 
        Debug.Log("DB Location: " + connection); 

        dbcon = new SqliteConnection(connection); 
        dbcon.Open(); 

    
        IDbCommand cmnd_read = dbcon.CreateCommand(); 
        string query = "SELECT * FROM Incidents"; 
        cmnd_read.CommandText = query; 

        IDataReader reader = cmnd_read.ExecuteReader(); 


        Debug.Log("Generating csv"); 
        CSVManager.DeleteReport(); 

        while (reader.Read()) 
        { 
            string[] auxString = new string[11] 
            { 
                reader[0].ToString(), 
                reader[1].ToString(), 
                reader[2].ToString(), 
                reader[3].ToString(), 
                reader[4].ToString(), 
                reader[5].ToString(), 
                reader[6].ToString(), 
                reader[7].ToString(), 
                reader[8].ToString(), 
                reader[9].ToString(), 
                reader[10].ToString() 
            }; 

            CSVManager.AppendToReport(auxString); 

        } 

        dbcon.Close(); 
    

        // Obtener ruta local del CSV generado 
        string filePath = CSVManager.GetFilePath(); 
        Debug.Log("Ruta del archivo CSV: " + filePath); 

    

        // === SUBIDA A AZURE (Desactivada) === 

        /* 

        Debug.Log("Sending to Azure"); 

    

        string connectionString = "DefaultEndpointsProtocol=https;AccountName=ergosentblobs;AccountKey=Nx/tvCcBZ+EQE0IjuAMbmFjgPwJxNiCX/P7WdOUnyN+f7yL2fQE/MPuqL7Tm+AbdWHhUDDhHuQF1QudGOYIuLA==;EndpointSuffix=core.windows.net"; 

        string containerName = "ergoblob"; 

        string blobName = BTNScriptsLogin.selectedUserId + " - " + BTNScriptsLogin.selectedUserName + ".csv"; 

    

        Debug.Log("Creating connection"); 

        BlobContainerClient container = new BlobContainerClient(connectionString, containerName); 

    

        Debug.Log("Getting pointer"); 

        BlobClient blob = container.GetBlobClient(blobName); 

    

        Debug.Log("Uploading"); 

        blob.Upload(filePath,true); 

        */ 

    

        // === GUARDADO LOCAL TEMPORAL === 

        try 
        { 
            string destino = Path.Combine("C:/ErgoSentReportes/", BTNScriptsLogin.selectedUserId + " - " + BTNScriptsLogin.selectedUserName + ".csv"); 
            Directory.CreateDirectory("C:/ErgoSentReportes/"); 
            File.Copy(filePath, destino, true); 
            Debug.Log("Archivo guardado localmente en: " + destino); 
        } 
        catch(Exception ex) 
        { 
            Debug.LogError("Error al guardar el archivo local: " + ex.Message); 
        } 

    

        // UI Feedback 

        BTN_Sendtxt.GetComponent<UnityEngine.UI.Text>().text = "2. Enviar Datos"; 

        statusText.GetComponent<UnityEngine.UI.Text>().text = "Reporte guardado localmente!"; 

    } 


    /*public void BTN_enviarDatos()
    {
        BTN_Sendtxt.GetComponent<UnityEngine.UI.Text>().text = "Enviando";
        
        IDbConnection dbcon;
        //Crear base de datos
        string connection = "URI=file:" + Application.persistentDataPath + "/My_Database";
        Debug.Log("DB Location: " + "URI=file:" + Application.persistentDataPath + "/My_Database");
        //Inicializar conexión
        dbcon = new SqliteConnection(connection);
        dbcon.Open();

        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM Incidents";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        
        Debug.Log("Generating csv");

        CSVManager.DeleteReport();
        while (reader.Read())
        {
            string[] auxString = new string[11]
            {
                reader[0].ToString(),
                reader[1].ToString(),
                reader[2].ToString(),
                reader[3].ToString(),
                reader[4].ToString(),
                reader[5].ToString(),
                reader[6].ToString(),
                reader[7].ToString(),
                reader[8].ToString(),
                reader[9].ToString(),
                reader[10].ToString()
            };
            
            CSVManager.AppendToReport(auxString);
        }
        dbcon.Close();
        Debug.Log("Sending to Azure");

        string connectionString = "DefaultEndpointsProtocol=https;AccountName=ergosentblobs;AccountKey=Nx/tvCcBZ+EQE0IjuAMbmFjgPwJxNiCX/P7WdOUnyN+f7yL2fQE/MPuqL7Tm+AbdWHhUDDhHuQF1QudGOYIuLA==;EndpointSuffix=core.windows.net";
        string containerName = "ergoblob"; //Nombre del container en azure
        string blobName = BTNScriptsLogin.selectedUserId + " - " + BTNScriptsLogin.selectedUserName + ".csv"; //Nombre con el que queda el archivo en el directorio
        string filePath = CSVManager.GetFilePath(); //Archivo a subir

        // Get a reference to a container named "sample-container" and then create it
        Debug.Log("Creating connection");
        BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

        // Get a reference to a blob named "sample-file" in a container named "sample-container"
        Debug.Log("Getting pointer");
        BlobClient blob = container.GetBlobClient(blobName);
        
        Debug.Log("Uploading");
        blob.Upload(filePath,true);

        Debug.Log("Done");

        BTN_Sendtxt.GetComponent<UnityEngine.UI.Text>().text = "2. Enviar Datos";
        statusText.GetComponent<UnityEngine.UI.Text>().text = "Reporte Enviado!";
    }*/


}


public static class CSVManager
{

    private static string reportDirectoryName = "Report";
    private static string reportFileName = "report.csv";
    private static string reportSeparator = ",";
    private static string[] reportHeaders = new string[11] {
        "IncidentId",
        "Date",
        "StartTime",
        "EndTime",
        "RuleId",
        "DurationInSeconds",
        "Score",
        "Hand",
        "AvgDegree",
        "Username",
        "UserId"
    };

    #region Interactions

    public static void AppendToReport(string[] strings)
    {
        VerifyDirectory();
        VerifyFile();
        Debug.Log("Appending to: "+GetFilePath());
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += strings[i];
            }
            sw.WriteLine(finalString);
        }
    }

    public static void DeleteReport()
    {
        if (File.Exists(GetFilePath()))
        {
            File.Delete(GetFilePath());
        }
    }

    public static void CreateReport()
    {
        VerifyDirectory();
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += reportHeaders[i];
            }
            sw.WriteLine(finalString);
        }
    }

    #endregion


    #region Operations

    static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            CreateReport();
        }
    }

    #endregion


    #region Queries

    static string GetDirectoryPath()
    {
        return Application.dataPath + "/" + reportDirectoryName;
    }

    public static string GetFilePath()
    {
        return GetDirectoryPath() + "/" + reportFileName;
    }

    static string GetTimeStamp()
    {
        return System.DateTime.UtcNow.ToString();
    }

    #endregion

}
