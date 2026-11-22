using UnityEngine;

public class Menu : MonoBehaviour
{
   public void jugar()
    {
        Debug.Log("JUGAR!!!");

        // aqui ja ficarem la resta de coses necessaries
    }

    public void nivells()
    {
        Debug.Log("NIVELLS!");

        // ficarem elq calgui
    }

    public void exit()
    {
        Debug.Log("SORTIM DEL JOC!"); // ho poso per ara pq l'editor no tanca el joc
        Application.Quit();
    }
}
