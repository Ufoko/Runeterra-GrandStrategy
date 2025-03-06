using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using V1king;
using Mirror;

public class Recalling : NetworkBehaviour
{
    public Army army;
    [SerializeField] private Image recallImage = null;

    bool isRecalling;
    bool standingStill;
    public float recallTimer = 4f;
    public float recallStartTimer = 4f;
    Vector3 startPos;

    void Start() {

        startPos = transform.position;   
    }

    // Update is called once per frame
    void Update()
    {
        if(hasAuthority)
            LocalUpdate();
        else
            GlobalUpdate();
    }

    /// <summary>
    /// Update for the "local" player who owns the army.<br></br>
    /// Put logic to be performed in here.
    /// </summary>
    private void LocalUpdate()
    {
        if(army.movement.isStopped)
        {
            standingStill = true;
        }
        else
        {
            standingStill = false;
        }

        if(Input.GetKeyDown(KeyCode.B) && standingStill)
        {
            StartCoroutine(Recall(gameObject));
        }

        if(isRecalling)
        {
            recallTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Update for non-"local" players. These are all the other players that don't own the army.<br></br>
    /// Put code to sync changes here.
    /// </summary>
    private void GlobalUpdate()
    {
        const float recallEndTimer = 1f;
        if(isRecalling)
        {
            recallTimer -= Time.deltaTime;
            recallImage.fillAmount = MathConversions.ConvertNumberRange(recallTimer, recallEndTimer, recallStartTimer, 0f, 1f);

            // Reset when recall done
            if(recallTimer < 0f)
                recallTimer = recallEndTimer;
        }
    }

    public IEnumerator Recall (GameObject go) {
        CmdSetRecalling(true);

        while (recallTimer >= 0) {
            const float recallEndTimer = 1f;

            // Recall image
            recallImage.fillAmount = MathConversions.ConvertNumberRange(recallTimer, recallEndTimer, recallStartTimer, 0f, 1f);

            // Logic and animations
            if (!standingStill) {
                recallTimer = recallStartTimer;
                CmdSetRecalling(false);
                break;
            }

            if (recallTimer <= recallEndTimer && standingStill) {
                go.transform.position = startPos;
                CmdSetRecalling(false);
                recallTimer = recallStartTimer;
                break;
            }
            yield return null;
        }
    }

    [Command]
    private void CmdSetRecalling(bool value)
    {
        RpcSetRecalling(value);
    }
    [ClientRpc]
    private void RpcSetRecalling(bool value)
    {
        isRecalling = value;

        // Image
        recallImage.gameObject.SetActive(value);
        if(!value && !hasAuthority) // Non-local players have their own "close-enough" logic to handle recall timer
            recallTimer = recallStartTimer;

        // Animation & Audio
            army.animScript.CmdSetRecallingAnim(value);
    }
}
