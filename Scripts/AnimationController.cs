using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [field: SerializeField]
    public string[] whenChangeSide { get; private set; }
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
    public bool ChangeLayerOnTurn;

    float zPos;
    // Start is called before the first frame update
    void Start()
    {
        zPos = transform.localPosition.z;
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






        if (currentStateName == "") throw new System.Exception("No state found "+animator.activeStates);
        currentAnimation = AnimationSlicer.animations.Find(x => x.partName == partName && x.boolSide == side && x.name == currentStateName);
        if (currentState != null) spriteRenderer.sprite = currentAnimation.GetSprite(currentState.frameNumber);
        else throw new System.Exception("Smth's wrong");

        int zIndex = 1;
        if (ChangeLayerOnTurn)
        {
            if (!side) zIndex *= -1;
        }
        if (whenChangeSide.Contains(currentAnimation.name))
        {
            zIndex *= -1;
        }
        transform.localPosition=new Vector3(transform.localPosition.x,transform.localPosition.y,zPos*zIndex);

    }
}
