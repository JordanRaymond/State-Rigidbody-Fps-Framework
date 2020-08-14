using UnityEngine;
using Core;

public abstract class Controller<Params, State> : MonoBehaviour where Params : Params<Params> where State : IState<Params>
{
    public Params defaultParams;
    private InputHandler inputHandler;

    #region State vars
    public State defaultState;
    private State currentState;
    #endregion 

    void Start()
    {
        if (GameManager.Instance != null)
        {
            inputHandler = GameManager.InputHandler;
        }
        else
        {
            Debug.LogError("The GameManager is not yet created, pls change script order.");
        }
        InitParams();
        SetState(defaultState);
    }

    /** 
    .summary
        Called in the Start function, it will init
        the needed var in the params.
    **/
    protected abstract void InitParams();

    void Update()
    {
        currentState.UpdateState(defaultParams);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(defaultParams);
    }

    public void SetState(State state)
    {
        if (state != null)
        {
            // Debug.Log("State set:" + state.GetName());
            if (currentState != null)
            {
                currentState.OnExitState(defaultParams);
            }
            currentState = state;
            currentState.OnEnteredState(defaultParams);
        }
        else
        {
            Debug.LogError("TODO: Name; Can't enter null state");
        }
    }
}
