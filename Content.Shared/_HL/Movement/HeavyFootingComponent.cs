using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._HL.Movement;

/// <summary>
/// Every so many tiles walked there is a chance the entity loses its footing and falls over.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HeavyFootingComponent : Component
{
    /// <summary>
    /// Chance to fall over per step taken.
    /// </summary>
    [DataField]
    public float StumbleChance = 0.02f;

    /// <summary>
    /// How far the entity has to move for a step to count.
    /// </summary>
    [DataField]
    public float StepDistance = 1.5f;

    /// <summary>
    /// How long the entity stays down for after a stumble.
    /// </summary>
    [DataField]
    public TimeSpan KnockdownTime = TimeSpan.FromSeconds(2);

    [DataField]
    public SoundSpecifier StumbleSound = new SoundPathSpecifier("/Audio/Effects/slip.ogg");

    /// <summary>
    /// Popup shown to the entity when it stumbles.
    /// </summary>
    [DataField]
    public LocId Popup = "heavy-footing-stumble";

    /// <summary>
    /// Distance moved since the last step.
    /// </summary>
    [ViewVariables]
    public float Accumulator;
}
