using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    SpriteRenderer spriteRenderer;
    [field: SerializeField]
    public string partName { get; private set; }
    [field: SerializeField]
    public string[] statesPriority { get; private set; }
    public string currentStateName { get; private set; }
    public Animation currentAnimation { get; private set; }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimationController))]
    public class AnimationControllerEditor : Editor
    {
        AnimationController animationController;
        private void OnEnable()
        {
            animationController = (AnimationController)target;
            if (animationController.statesPriority == null || animationController.statesPriority.Length == 0)
            {
                animationController.statesPriority = AnimationSlicer.animNames;
            }
        }
    }

#endif
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.animatorStateTimer += ChangeSprite;
    }

    void ChangeSprite()
    {
        State currentState = null;
        currentStateName = "";
        foreach (var st in statesPriority)
        {
            currentState = animator.states.Find(x => x.name == st);
            if (currentState == null) continue;
            if (currentState.active)
            {
                currentStateName = st;
                break;
            }
        }
        bool side = !animator.states.Find(x => x.name == "L").active;

        //DELETE
        if (partName == "Sword")
        {
            if (side) transform.localPosition = Vector3.back;
            else transform.localPosition = Vector3.forward;
        }
        if (partName == "Shield")
        {
            if (!side) transform.localPosition = Vector3.back;
            else transform.localPosition = Vector3.forward;
        }



        if (currentStateName == "") throw new System.Exception("No state found "+animator.activeStates);
        currentAnimation = AnimationSlicer.animations.Find(x => x.partName == partName && x.boolSide == side && x.name == currentStateName);
        if (currentState != null) spriteRenderer.sprite = currentAnimation.GetSprite(currentState.frameNumber);
        else throw new System.Exception("Smth's wrong");
    }
}
