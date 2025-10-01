using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCovers : MonoBehaviour
{
    [Header("Inscribe")]
    public Sprite[] cloudSprites;
    public int numClouds = 40;
    public Vector3 minPos = new Vector3(-20, -5, -5);
    public Vector3 maxPos = new Vector3(300, 40, 5);
    public Vector2 scaleRange = new Vector2(1, 4);
    // Start is called before the first frame update
    void Start()
    {
        Transform parentTrans = this.transform;
        GameObject cloudGO;
        Transform cloudTrans;
        SpriteRenderer sRend;
        float scaleMult;
        for (int i = 0; i < numClouds; i++) {
            cloudGO = new GameObject();
            cloudTrans = cloudGO.transform;
            sRend = cloudGO.AddComponent<SpriteRenderer>();
            int spriteNum = Random.Range(0, cloudSprites.Length);
            sRend.sprite = cloudSprites[spriteNum];
            cloudTrans.position = RandomPos();
            cloudTrans.SetParent(parentTrans);
            scaleMult = Random.Range(scaleRange.x, scaleRange.y);
            cloudTrans.localScale = Vector3.one * scaleMult;
        }
    }

    Vector3 RandomPos() {
        float x = Random.Range(minPos.x, maxPos.x);
        float y = Random.Range(minPos.y, maxPos.y);
        float z = Random.Range(minPos.z, maxPos.z);
        return (new Vector3(x,y,z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
