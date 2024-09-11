using System.Collections;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField] public Renderer BrickMeshRenderer;
    [SerializeField] public Renderer PlayerMeshRenderer;
    [SerializeField] public Renderer EnemyMeshRenderer;
    [SerializeField] public BrickControl BrickControl;

    private void OnTriggerEnter(Collider other) 
    {
        
        if(other.CompareTag(Constant.TAG_PLAYER) || other.CompareTag(Constant.TAG_ENEMY))
        {
            BrickControl = other.gameObject.GetComponent<BrickControl>();
            EnemyMeshRenderer = other.gameObject.GetComponent<BrickControl>().meshRenderer;
            if(other.CompareTag(Constant.TAG_PLAYER))
            {
                if(BrickMeshRenderer.material.color != PlayerMeshRenderer.sharedMaterial.color && BrickControl.blockCount > 0)
                {
                    
                    {
                        BrickMeshRenderer.material.color = PlayerMeshRenderer.sharedMaterial.color;
                    }
                    RemoveBlock();
                }   
            }
            else
            {
                if(BrickMeshRenderer.material.color != EnemyMeshRenderer.sharedMaterial.color && BrickControl.blockCount > 0)
                {
                    
                    {
                        BrickMeshRenderer.material.color = EnemyMeshRenderer.sharedMaterial.color;
                    }
                    RemoveBlock();
                }   
            }
            

        }
        
    }
    public void RemoveBlock()
    {
        GameObject blockToRemove = BrickControl.blockList[BrickControl.blockCount - 1];
        BrickControl.blockList.RemoveAt(BrickControl.blockCount - 1);
        BrickControl.blockCount--;
        Destroy(blockToRemove);
    }
}
