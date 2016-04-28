using UnityEngine;

public class SlabScript : MonoBehaviour {
    private int pista;
    private Vector3 posicion;

    public GameObject slabPrefab;

    public Vector3 getPosicion() {
        return posicion;
    }

    public SlabScript setPosicion(Vector3 posicion) {
        this.posicion = posicion;
        return this;
    }

    public SlabScript setPosicion(int largo, int ancho) {
        this.posicion = new Vector3(largo, 0, ancho);
        return this;
    }

    public int getPista() {
        return pista;
    }

    public SlabScript setPista(int pista) {
        this.pista = pista;
        return this;
    }

    /*void Update() {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed left click.");
    }*/

    void OnMouseDown() {
        Debug.Log("Pressed left click.");
        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.red);
    }
}
