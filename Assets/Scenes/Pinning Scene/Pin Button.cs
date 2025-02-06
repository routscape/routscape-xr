using UnityEngine;
using UnityEngine.UI;

public class PinButton : Button
{
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (state == SelectionState.Pressed)
        {
            Debug.Log("Pressed");
        }
    }

}
