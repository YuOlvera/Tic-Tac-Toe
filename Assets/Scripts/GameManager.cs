using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq; //Para BFS
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;

//En este código también se implementa el comportamiento del contrincante
public class GameManager : MonoBehaviour
{
    private int cantidadVertices;
    private LinkedList<string[,]> listaAdyacencia;
    private int nivel = 0;
    private bool turnoIA = true;
    //----------------------------------------------------------------
    public string[,] tablero = new string[3,3]; //Va a estar cambiando en cada turno
    public Text[] listaBotones;
    private static string playerSide; //Será aleatorio
    private bool eleccionIA = false; 
    private bool interrupcion = false;
    private int botonEscogido; //Es el botón que escoge la IA tras aplicar BFS

    //Para mostrar el turno:
    public Image imagenTurno;
    public Text textoTurno; 
    public Text GameOverText, YouWinText, DrawText;

    void Awake(){
        //---------------Crear el grafo-----------------
        //Asignar los botones al tablero
        ActualizarTablero();
        SetReferenciaBotonesDeGameManager();
        //Elegir aleatoriamente quién empieza
        int random = Random.Range(1,3);
        if(random == 1) //IA
            playerSide = "x";
        else            //Player
            playerSide = "o";
        CambiarColoresTurno();
        YouWinText.gameObject.SetActive(false);
        GameOverText.gameObject.SetActive(false);
        DrawText.gameObject.SetActive(false);
    }

    void Update() {
        if(playerSide == "x"){ //Si es el turno de la IA
            if(turnoIA){
                Debug.Log("turnoIA");
                turnoIA = false; //Para evitar que se repita el comportamiento
                //Toma el tablero y genera las posibilidades con BFS de acuerdo al nivel en el que vaya
                switch(nivel){
                    case 0: BFS(0,9); break; //Tiene 9 posibilidades
                    case 1: BFS(1,8); break; //8
                    case 2: BFS(2,7); break; //7
                    case 3: BFS(3,6); break; //6
                    case 4: BFS(4,5); break; //5
                    case 5: BFS(5,4); break; //4
                    case 6: BFS(6,3); break; //3
                    case 7: BFS(7,2); break; //2
                    case 8: BFS(8,1); break; //1
                }
            }
        }
    }
    void BFS(int s, int cantPosibilidades){
        Debug.Log("S: " + s);
        if(s==0){
            StartCoroutine(DelayNivelUnoIA());
        }else if(s==1){//Si el segundo turno le toca a la IA
            StartCoroutine(NivelDosIA());
        }if(s>=2){
            //Crear una cola para la búsqueda por anchura, se meten todas las posibilidades
            LinkedList<string[,]> cola = new LinkedList<string[,]>(); //Sin tamaño específico
            LinkedList<string[,]> visitados = new LinkedList<string[,]>(); //Sin tamaño específico
            //Marcar el nodo inicial como visitado y ponerlo en la cola
            cola.AddLast(tablero);

            //Sacar vértice de la cola 
            string[,] posibilidad = new string[3,3];
            string[,] auxiliar = new string[3,3];
            posibilidad = (string[,])cola.First().Clone();
            auxiliar = (string[,])posibilidad.Clone(); //COPIAR UN ARRAY ES DIFERENTE EN C#!!!!!
            cola.RemoveFirst(); //quitamos el primer nodo porque ahora almacenamos las posibilidades
            cola.Clear();
            
            //Calcular posibilidades----------------------------------------------------------
            for(int i=1; i<=cantPosibilidades; i++){
                bool añadido = false;
                for(int m=0; m<3; m++){ //Recorrido de la matriz
                    for(int n=0; n<3; n++){
                        if(string.IsNullOrEmpty(auxiliar[m,n]) && !añadido){
                            posibilidad[m,n] = "x"; //Se pone la x
                            añadido = true;
                            string[,] enviado = new string[3,3];
                            enviado = (string[,])posibilidad.Clone();
                            visitados.AddLast(enviado); //Se manda a la cola
                            posibilidad[m,n] = string.Empty; //Se quita la x
                            auxiliar[m,n] = "1";
                        }
                    }
                }
            }//--------------------------------------------------------------------------------
            //Como las posibilidades ahora están en la lista ligada que almacena cada celda una lista ligada con las posibilidades
            //se escoge la mejor opción dado un conjunto de reglas y la conexión de nodos

            Debug.Log("Tamaño visitados: " + visitados.Count());
            //Escoge la posibilidad dado el conjunto de reglas
            foreach(var matriz in visitados){
                if(!eleccionIA){
                    StringBuilder sb = new StringBuilder();
                    for(int x=0; x<3; x++){
                        for(int y=0; y<3; y++){
                            sb.Append(matriz[x,y] + "\t");
                        }
                        sb.AppendLine();
                    }
                    Debug.Log("Posibilidad \n" + sb.ToString());

                    //Contar donde habrían 2 x's ya sea en fila, columna o diagonal, donde hayan más se escoge ese camino
                    //CONDICIONES DE VICTORIA------------------------------------------------------------------------------------------------------------------------------------------
                    int[] coincidencias = new int[8];
                    //Si esa posibilidad se acerca a que gane la IA
                    for(int j=0; j<3; j++){
                        //FILAS
                        if(matriz[0,j] == "x") //Fila 1
                            coincidencias[0]++;
                        if(matriz[1,j] == "x") //Fila 2
                            coincidencias[1]++;
                        if(matriz[2,j] == "x") //Fila 3
                            coincidencias[2]++;
                        //Columnas
                        if(matriz[j,0] == "x") //Columna 1
                            coincidencias[3]++;
                        if(matriz[j,1] == "x") //Columna 2
                            coincidencias[4]++;
                        if(matriz[j,2] == "x") //Columna 3
                            coincidencias[5]++;
                    }
                    for(int i=0; i<3; i++){
                        if(matriz[i,i] == "x") //Diagonal hacia abajo
                            coincidencias[6]++;
                    }
                    if(matriz[2,0] == "x") //Diagonal hacia arriba
                        coincidencias[7]++;
                    if(matriz[1,1] == "x")
                        coincidencias[7]++;
                    if(matriz[0,2] == "x")
                        coincidencias[7]++;

                    Debug.Log(coincidencias[0] + " " + coincidencias[1] + " " + coincidencias[2] + " " + coincidencias[3] + " " + coincidencias[4] + " " + coincidencias[5] + " " + coincidencias[6] + " " + coincidencias[7]);
                    //Y esa posibilidad evita que el jugador gane
                    //Tómala primero
                    for(int i=0; i<8; i++){
                        if(coincidencias[i] >= 2){
                            eleccionIA = true;
                        }
                    }
                    eleccionIA = true;
                    //----------------------------------------------------------------------------------.--------------------------------------------------------------------------------
                    if(eleccionIA){
                        //Búsqueda de los cambios y actualización del tablero y botones.
                        int cont = 0;
                        for(int i=0; i<3;i++){
                            for(int j=0; j<3;j++){
                                if(matriz[i,j] != tablero[i,j]){ //Identificas el lugar del cambio
                                    tablero[i,j] = matriz[i,j];
                                    goto final;
                                }else{
                                    cont++;
                                }
                            }
                        }
                        //Modificar la lista de botones
                        final:
                        listaBotones[cont].GetComponentInParent<Boton>().AccionBoton();
                        eleccionIA = true;
                    }
                }
            }
            eleccionIA = false;
            cola.Clear();
            visitados.Clear();
            turnoIA = true;
        }
    }

    private IEnumerator DelayNivelUnoIA(){
        yield return new WaitForSeconds(0.5f);
        tablero[1,1] = "x"; //La IA en su primer turno siempre prioriza colocar la X en el centro
        listaBotones[4].GetComponentInParent<Boton>().AccionBoton();
        turnoIA = true;
    }

    private IEnumerator NivelDosIA(){
        yield return new WaitForSeconds(0.1f);
        if(listaBotones[4].GetComponentInParent<Button>().interactable == true){ //Si está disponible el centro
            listaBotones[4].GetComponentInParent<Boton>().AccionBoton();
            turnoIA = true;
        }else{
            listaBotones[0].GetComponentInParent<Boton>().AccionBoton();
            turnoIA = true;
        }
    }

    void ActualizarTablero(){
        StringBuilder sb = new StringBuilder();
        int count = 0;
        for(int i = 0; i<3; i++){
            for(int j = 0; j<3; j++){
                tablero[i,j] = listaBotones[count].text;
                sb.Append(tablero[i,j] + "\t");
                count++;
            }sb.AppendLine();
        }
        Debug.Log(sb.ToString()); //Mostrar matriz
    }

    void SetReferenciaBotonesDeGameManager(){
        for(int i=0; i< listaBotones.Length; i++){
            listaBotones[i].GetComponentInParent<Boton>().SetReferenciaGameManager(this); //Referencia del código Boton; Asigna el GameManager
        }
    }

    public string GetTurno(){
        return playerSide; //Regresa una X u O al botón
    }

    public void TerminarTurno(){
        ActualizarTablero();
        //Revisar si ya hay ganador--------------------------------------------------------------------------------------------------------
        bool flag = false;
        //FILAS:
        if(listaBotones[0].text == playerSide && listaBotones[1].text == playerSide && listaBotones[2].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[0].GetComponentInParent<Image>().color = new Color32(255,255,248,255);
            listaBotones[1].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[2].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        if(listaBotones[3].text == playerSide && listaBotones[4].text == playerSide && listaBotones[5].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[3].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[4].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[5].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        if(listaBotones[6].text == playerSide && listaBotones[7].text == playerSide && listaBotones[8].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[6].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[7].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[8].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        //COLUMNAS:
        if(listaBotones[0].text == playerSide && listaBotones[3].text == playerSide && listaBotones[6].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[0].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[3].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[6].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        if(listaBotones[1].text == playerSide && listaBotones[4].text == playerSide && listaBotones[7].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[1].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[4].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[7].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        if(listaBotones[2].text == playerSide && listaBotones[5].text == playerSide && listaBotones[8].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[2].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[5].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[8].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        //DIAGONALES:
        if(listaBotones[0].text == playerSide && listaBotones[4].text == playerSide && listaBotones[8].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[0].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[4].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[8].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        if(listaBotones[2].text == playerSide && listaBotones[4].text == playerSide && listaBotones[6].text == playerSide){ //Primera fila
            flag = true;
            listaBotones[2].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[4].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            listaBotones[6].GetComponentInParent<Image>().color  = new Color32(255,255,248,255);
            GameOver();
        }
        //--------------------------------------------------------------------------------------------------------------------------------

        //Revisar si hay un empate
        int contador = 0;
        for(int i=0; i<listaBotones.Length;i++){
            if(listaBotones[i].text == "x" || listaBotones[i].text == "o")
                contador++;
        }
        if(contador == 9 && !flag){
            DrawText.gameObject.SetActive(true);
            StartCoroutine("Reiniciar");
        }

        //Incrementar el nivel
        nivel++;
        //Cambiar turno
        CambiarTurno();
    }

    void GameOver(){
        for(int i=0; i<listaBotones.Length; i++){
            listaBotones[i].GetComponentInParent<Button>().interactable = false;
        }
        if(playerSide == "o"){
            GameOverText.gameObject.SetActive(true);
        }else{
            YouWinText.gameObject.SetActive(true);
        }

        StartCoroutine(Reiniciar());
    }

    public IEnumerator Reiniciar(){
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Game");
    }

    void CambiarTurno(){
        playerSide = (playerSide == "x") ? "o" : "x";
        //Obtener el nombre de quien tiene el turno y cambiar el color
        CambiarColoresTurno();
    }

    void CambiarColoresTurno(){
        if(playerSide == "x"){
            textoTurno.text = "AI";
            imagenTurno.color = new Color32(162,17,0,83);
            textoTurno.color = new Color32(255,0,14,255);
        }else{
            textoTurno.text = "PLAYER";
            imagenTurno.color = new Color32(0,103,45,83);
            textoTurno.color = new Color32(82,255,42,255);
        }
    }
}
