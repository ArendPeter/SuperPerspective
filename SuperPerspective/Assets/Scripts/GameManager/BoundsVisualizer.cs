using UnityEngine;
using System.Collections;

public class BoundsVisualizer : MonoBehaviour
{

    public GameObject zLeft, zRight, xFront, xBack;
    public GameObject[] demBoiz;
    float xx, yy, zz, distanceModifier, padding = 1;
    public BoundObject bObj;
    Rect bRect;
    SpriteRenderer temp;

    bool started = false;
	
    public void updateBounds()
    {
        bRect = bObj.GetBounds();
    }

    void StartOnce()
    {
        updateBounds();//This ensures that GetBounds returns values after they've been set
        demBoiz = new GameObject[]{ zLeft, zRight, xFront, xBack };
        started = true;
    }
	// Update is called once per frame
	void Update () {
        if (!started)
        {
            StartOnce();
        }
        setVisualizerLocations();
        setVisualizerAlpha();
    }

    void setVisualizerAlpha()
    {

        for (int i = 0; i < demBoiz.Length; i++)
        {
            temp = demBoiz[i].GetComponent<SpriteRenderer>();
            if (GameStateManager.is3D())
            {
                distanceModifier = (2f - Mathf.Min(2f, Vector3.Distance(demBoiz[i].transform.position, gameObject.transform.position))) / 2f;
                temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, distanceModifier);
            }
            else
            {
                temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0);
            }
        }
    }

    void setVisualizerLocations()
    {
        UpdateZLeft();
        UpdateZRight();
        UpdateXFront();
        UpdateXBack();
    }

    void UpdateZLeft()
    {
        xx = gameObject.transform.position.x;
        yy = gameObject.transform.position.y;
        zz = bRect.yMax + padding;
        zLeft.transform.position = new Vector3(xx, yy, zz);
    }

    void UpdateZRight()
    {
        xx = gameObject.transform.position.x;
        yy = gameObject.transform.position.y;
        zz = bRect.yMin - padding;
        zRight.transform.position = new Vector3(xx, yy, zz);
    }

    void UpdateXFront()
    {
        xx = bRect.xMax + padding;
        yy = gameObject.transform.position.y;
        zz = gameObject.transform.position.z;
        xFront.transform.position = new Vector3(xx, yy, zz);
    }

    void UpdateXBack()
    {
        xx = bRect.xMin - padding;
        yy = gameObject.transform.position.y;
        zz = gameObject.transform.position.z;
        xBack.transform.position = new Vector3(xx, yy, zz);
    }
}
