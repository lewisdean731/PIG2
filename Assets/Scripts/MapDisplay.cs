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
        }

        // Update scale in case terrain tile counts changed
        TerrainData td = gameObject.GetComponent<MapGenerator>().terrainData;
        float xScale = (float)MapMetrics.tileSize / 10 * td.tileCountX;
        float zScale = (float)MapMetrics.tileSize / 10 * td.tileCountY;
        g.transform.localScale = new Vector3(xScale, 1, zScale);

        return g;
    }
}
