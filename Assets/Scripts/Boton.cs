using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boton : MonoBehaviour
{
    public Button boton;
    public Text textoBoton;
    public GameManager gameManager;

    public void AccionBoton(){ //Se llama cada vez que das clic en el botón
        textoBoton.text = gameManager.GetTurno();
        boton.interactable = false;
        CambiarColorBoton();
        gameManager.TerminarTurno(); //Tras escoger termina su turno, en caso de haber ganado llama a la función gameover
    }

    public void SetReferenciaGameManager(GameManager manager){
        gameManager = manager;
    }
    
    public void CambiarColorBoton(){
        if(textoBoton.text == "x"){ //IA
            textoBoton.color = new Color32(255,0,14,255);
        }else{ //Player
            textoBoton.color = new Color32(82,255,42,255);
        }
    }
}
