using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public void Jugar(){
        SceneManager.LoadScene("Game");
    }
    public void Salir(){
        Application.Quit();
    }
}
