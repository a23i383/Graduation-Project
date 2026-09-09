using UnityEngine;

public class BackGroundManager:MonoBehaviour
{
    [Header("camera")]
    [SerializeField] private Transform cameraTransform;
    [Header("sky")]
    [SerializeField] private Transform BGSky;
    [SerializeField] private float parallaxEffectSky = 0.6f;
    [Header("mountain")]
    [SerializeField] private Transform BGMountain;
    [SerializeField] private float parallaxEffectMountain = 0.3f;
    [Header("Tree")]
    [SerializeField] private Transform BGTree;
    [SerializeField] private float parallaxEffectTree = 0.1f;

    private Vector3 lastCameraPosition;
    private Vector3 delta;
    private float spriteWidthSky, spriteWidthMt, spriteWidthTree;
    private Transform[] BGSkys, BGTrees, BGMts;

    private void Start()
    {
        lastCameraPosition = cameraTransform.position;
        BGSkys = new Transform[3];
        InitBackGrounds(BGSkys, BGSky);
        BGTrees = new Transform[3];
        InitBackGrounds(BGTrees, BGTree);
        BGMts = new Transform[3];
        InitBackGrounds(BGMts, BGMountain);
        SpriteRenderer 
            srS = BGSkys[0].GetComponent<SpriteRenderer>(),
            srT = BGTrees[0].GetComponent<SpriteRenderer>(),
            srM = BGMts[0].GetComponent<SpriteRenderer>();

        spriteWidthSky = srS.bounds.size.x;
        spriteWidthTree = srT.bounds.size.x;
        spriteWidthMt = srM.bounds.size.x;
        spriteWidthTree += 10;
        Debug.Log(spriteWidthSky+","+spriteWidthTree+","+spriteWidthMt);
    }
    private void LateUpdate()
    {
        delta=cameraTransform.position - lastCameraPosition;
        ParallaxEffect(BGSky, parallaxEffectSky);
        ParallaxEffect(BGMountain, parallaxEffectMountain);
        ParallaxEffect(BGTree, parallaxEffectTree);
        lastCameraPosition=cameraTransform.position;

        BGLooper(BGSkys, spriteWidthSky);
        BGLooper(BGTrees, spriteWidthTree);
        BGLooper(BGMts, spriteWidthMt);

    }
    private void BGLooper(Transform[] BGs, float width)
    {
        if (cameraTransform.position.x < BGs[0].position.x + (width / 2))
        {
            BGs[2].position -= new Vector3(width * 3, 0, 0);
            Transform temp = BGs[0];
            BGs[0] = BGs[2];
            BGs[2] = BGs[1];
            BGs[1] = temp;
        }
        else if (BGs[2].position.x - (width / 2) < cameraTransform.position.x) 
        {
            BGs[0].position += new Vector3(width * 3, 0, 0);
            Transform temp = BGs[0];
            BGs[0] = BGs[1];
            BGs[1] = BGs[2];
            BGs[2] = temp;
        }


    }
    private void ParallaxEffect(Transform BG,float PE)
    {
        BG.position += new Vector3(delta.x * PE, 0, 0);
    }
    
    private void InitBackGrounds(Transform[] BGs,Transform BG)
    {
        for(int i = 0; i < 3; i++)
        {
            BGs[i]=BG.GetChild(i);
        }
    }
}
