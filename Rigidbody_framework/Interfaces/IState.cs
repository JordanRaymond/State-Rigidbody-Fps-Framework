public interface IState<T>
{
    void OnEnteredState(T tParams);

    void OnExitState(T tParams);

    void UpdateState(T tParams);

    void FixedUpdateState(T tParams);

    string GetName();
}