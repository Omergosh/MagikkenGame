using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAnimHandler : MonoBehaviour
{
    Animator animator;
    [SerializeField] PlayerAnimationSet animationSet;
    public MageAnimHandler otherPlayerModel;
    public PlayerDebugVisuals debugVisuals;
    public int playerIndex;

    public bool displayDebug = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        debugVisuals.playerIndex = playerIndex;
    }

    public void UpdateVisuals(in GameState gameState)
    {

        transform.position = new Vector3(
        ((float)gameState.players[playerIndex].position3D.x) / 100f,
        (((float)gameState.players[playerIndex].position3D.y) / 100f) + BattleVisualsManager.stageFloorOffset,
        ((float)gameState.players[playerIndex].position3D.z) / 100f
        );

        //transform.localScale = new Vector3(
        //    gameState.players[playerIndex].FacingMultiplier,
        //    transform.localScale.y,
        //    transform.localScale.z
        //    );

        UpdateRotation(in gameState);
        UpdateAnimation(in gameState);
        if (displayDebug) {
            debugVisuals.gameObject.SetActive(true);
            debugVisuals.UpdateBoxes(in gameState);
        }
        else
        {
            debugVisuals.gameObject.SetActive(false);
        }
        
    }

    public void UpdateRotation(in GameState gameState)
    {

        //string playerStateName = gameState.players[playerIndex].stateMachine.state.GetType().Name;

        if (gameState.currentPhase == BattlePhase.DUEL_PHASE)
        {
            transform.forward = (Vector3)gameState.players[playerIndex].Forward;
        }
        else
        {
            //transform.forward = (Vector3)gameState.players[playerIndex].forward;
            transform.forward = Vector3.Slerp(transform.forward, (Vector3)gameState.players[playerIndex].Forward, 0.1f);

            //if (playerStateName == "FieldMove")
            //{
            //    Vector3 pVel = (Vector3)gameState.players[playerIndex].velocity.Normalized();
            //    if (pVel != Vector3.zero)
            //    {
            //        //transform.forward = pVel;
            //        transform.forward = Vector3.Slerp(transform.forward, pVel, 0.1f);
            //    }
            //}
        }
    }

    public void UpdateAnimation(in GameState gameState)
    {
        string playerStateName = gameState.players[playerIndex].stateMachine.state.GetType().Name;
        string animName = animationSet.playerStateAnimations[playerStateName];
        bool isAlreadyCurrentAnim = animator.GetCurrentAnimatorStateInfo(0).IsName(animName);
        bool isAlreadyNextAnim = animator.GetNextAnimatorStateInfo(0).IsName(animName);
        //Debug.Log($"current: {isAlreadyCurrentAnim} next: { isAlreadyNextAnim}");
        //Debug.Log($"current: {animator.GetCurrentAnimatorStateInfo(0).IsName(playerStateName)}");
        //Debug.Log($"current: {animator.GetNextAnimatorStateInfo(0).IsName(playerStateName)}");

        // TODO: Check if animation is a trivial 'looping' type,
        // or has actual associated frame data to keep in sync with.
        if (gameState.players[playerIndex].stateMachine.state.animData != null)
        {
            if (!isAlreadyCurrentAnim && !isAlreadyNextAnim) {
                int currentFrame = (int)gameState.players[playerIndex].stateMachine.state.currentFrame;
                int totalFrames = (int)gameState.players[playerIndex].stateMachine.state.animData.frames.Count;
                animator.Play(animName, -1, (float)currentFrame / (float)totalFrames);
            }
        }
        else
        {
            if (!isAlreadyCurrentAnim && !isAlreadyNextAnim) { animator.CrossFadeInFixedTime(animName, 0.05f); }
        }
    }
}
