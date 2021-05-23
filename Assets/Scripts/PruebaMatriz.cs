using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaMatriz : MonoBehaviour
{
    int[,] matriz = new int[3, 3];
    StringBuilder sb = new StringBuilder();
    // Start is called before the first frame update
    void Start()
    {
        int count = 1;
        for(int i=0; i<3; i++){
            for(int j=0; j<3; j++){
                matriz[i,j] = count;
                sb.Append(matriz[i,j] + "\t");
                count++;
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }
}
