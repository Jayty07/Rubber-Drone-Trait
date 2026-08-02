using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._HL.Body;

/// <summary>
/// Gives the entity a self contained breathing apparatus. A breath tool and a gas tank are spawned
/// inside the entity and wired into its internals, so it can run internals without a mask or a
/// carried tank.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class InnateInternalsComponent : Component
{
    /// <summary>
    /// Breath tool that gets spawned and connected to the entity's internals.
    /// </summary>
    [DataField]
    public EntProtoId BreathTool = "HLInnateRebreather";

    /// <summary>
    /// Gas tank that gets spawned and made available to the entity's internals.
    /// </summary>
    [DataField]
    public EntProtoId GasTank = "HLInnateAirReservoir";

    /// <summary>
    /// Container the spawned apparatus is kept in.
    /// </summary>
    [DataField]
    public string ContainerId = "innate_internals";

    [ViewVariables]
    public EntityUid? BreathToolEntity;

    [ViewVariables]
    public EntityUid? GasTankEntity;
}
