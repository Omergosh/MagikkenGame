using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAnimHandler : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    PlayerAnimationSet animationSet;
    public MageAnimHandler otherPlayerModel;
    public int playerIndex;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateVisuals(in GameState gameState)
    {

        transform.position = new Vector3(
        ((float)gameState.players[playerIndex].position.x) / 100f,
        (((float)gameState.players[playerIndex].position.y) / 100f) + BattleVisualsManager.stageFloorOffset,
        ((float)gameState.players[playerIndex].position.z) / 100f
        );

        transform.localScale = new Vector3(
            gameState.players[playerIndex].FacingMultiplier,
            transform.localScale.y,
            transform.localScale.z
            );

        UpdateRotation(in gameState);
        UpdateAnimation(in gameState);
    }

    public void UpdateRotation(in GameState gameState)
    {
        string playerStateName = gameState.players[playerIndex].stateMachine.state.GetType().Name;

        if(gameState.currentPhase == BattlePhase.DUEL_PHASE)
        {
            Vector3 lookAtTarget = new Vector3(
                otherPlayerModel.transform.position.x,
                transform.position.y,
                otherPlayerModel.transform.position.z);
            transform.LookAt(lookAtTarget);
        }
        else
        {
            if(playerStateName == "FieldMove")
            {
                Vector3 pVel = (Vector3)gameState.players[playerIndex].velocity.Normalized();
                if (pVel != Vector3.zero)
                {
                    //transform.forward = pVel;
                    transform.forward = Vector3.Slerp(transform.forward, pVel, 0.1f);
                }
            }
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
        if (!isAlreadyCurrentAnim && !isAlreadyNextAnim) { animator.CrossFadeInFixedTime(animName, 0.05f); }
        //if (!isAlreadyCurrentAnim && !isAlreadyNextAnim) { animator.Play(animName); }
    }
}
