using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    public void DrawTexture(Texture2D texture)
    {
        GameObject displayGO = FindOrCreateDisplay("Display");

        Renderer textureRender = displayGO.GetComponent<Renderer>();

        textureRender.sharedMaterial.mainTexture = texture;
        // textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public GameObject FindOrCreateDisplay(string name)
    {
        GameObject g = GameObject.Find(name);
        if (g == null)
        {
            GameObject parent = gameObject;

            g = GameObject.CreatePrimitive(PrimitiveType.Plane);
            g.name = string.Format(name);
            g.transform.parent = parent.transform;
            g.transform.localPosition = new Vector3(0, 0, 0);
            // g.transform.Rotate(new Vector3(0, 180, 0));
            g.transform.localScale = new Vector3(MapMetrics.tileSize / 10, 1, MapMetrics.tileSize / 10);
        }

        return g;
    }
}
