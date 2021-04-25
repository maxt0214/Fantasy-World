using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Models;

public class NPCController : MonoBehaviour
{
    public int NPCID;

    private SkinnedMeshRenderer currRenderer;
    private Animator anim;
    private NPCDefine npc;
    private Color originalColor;

    private bool isInteractive = false;

    private void Start()
    {
        currRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>(); 
        npc = NPCManager.Instance.GetNPCDefine(NPCID);
        originalColor = currRenderer.sharedMaterial.color;
        StartCoroutine(Actions());
    }

    IEnumerator Actions()
    {
        while(true)
        {
            if (isInteractive)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(Random.Range(5f,10f));
            Relax();
        }
    }

    private void Relax()
    {
        anim.SetTrigger("Relax");
    }

    private void Interact()
    {
        if(!isInteractive)
        {
            isInteractive = true;
            StartCoroutine(Interact(npc));
        }
    }

    private IEnumerator Interact(NPCDefine npc)
    {
        yield return FaceToPlayer();
        if(NPCManager.Instance.Interactive(npc.ID))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(1f);
        isInteractive = false;
    }

    private IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.currentCharacterObj.transform.position - transform.position).normalized;
        while(Mathf.Abs(Vector3.Angle(transform.forward,faceTo)) > 5)
        {
            transform.forward = Vector3.Lerp(transform.forward,faceTo,Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if((User.Instance.currentCharacterObj.transform.position - transform.position).magnitude < 10)
            Interact();
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    private void Highlight(bool ifHighlight)
    {
        if(ifHighlight)
        {
            if (currRenderer.sharedMaterial.color != Color.white)
                currRenderer.sharedMaterial.color = Color.white;
        } else
        {
            if (currRenderer.sharedMaterial.color != originalColor)
                currRenderer.sharedMaterial.color = originalColor;
        }
    }
}
