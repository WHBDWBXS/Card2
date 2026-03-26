using Godot;

namespace Card2.Systems;

public partial class GameBootstrap : Node
{
    public override void _EnterTree()
    {
        EnsureAction("move_left", Key.A);
        EnsureAction("move_right", Key.D);
        EnsureAction("move_up", Key.W);
        EnsureAction("move_down", Key.S);
        EnsureAction("run", Key.Shift);
        EnsureAction("dash", Key.Space);
        EnsureAction("attack", MouseButton.Left);
        EnsureAction("switch_mode", Key.Tab);
        EnsureAction("next_gun", Key.Q);
    }

    private static void EnsureAction(string action, Key key)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        var ev = new InputEventKey { Keycode = key };
        if (!InputMap.ActionHasEvent(action, ev))
        {
            InputMap.ActionAddEvent(action, ev);
        }
    }

    private static void EnsureAction(string action, MouseButton button)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        var ev = new InputEventMouseButton { ButtonIndex = button };
        if (!InputMap.ActionHasEvent(action, ev))
        {
            InputMap.ActionAddEvent(action, ev);
        }
    }
}
