using UnityEngine;

public class TableroManager : MonoBehaviour {
    public int largo;
    public int ancho;

    public GameObject slabPrefab;

    private GameObject[][] slabs;

    void Start () {
        slabs = new GameObject[largo][];
        for (int l = 0; l < largo; l++) {
            slabs[l] = new GameObject[ancho];
            for (int a = 0; a < ancho; a++) {
                slabs[l][a] = MakeSlab(l, a);
            }
        }
	}

    public GameObject MakeSlab(int largo, int ancho) {
        Vector3 posicion = new Vector3(largo, 0, ancho);

        GameObject slab = Instantiate(slabPrefab, posicion, Quaternion.identity) as GameObject;

        slab.name = "slab" + largo + ancho;
        slab.AddComponent<SlabScript>();
        slab.GetComponent<SlabScript>().setPosicion(posicion);
        slab.GetComponent<SlabScript>().setPista(ancho);

        return slab;
    }
}
