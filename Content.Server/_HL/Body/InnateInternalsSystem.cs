using Content.Shared._HL.Body;
using Content.Shared.Actions;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Robust.Shared.Containers;

namespace Content.Server._HL.Body;

/// <summary>
/// Spawns the built in breathing apparatus of an <see cref="InnateInternalsComponent"/> and keeps it
/// wired into the entity's internals.
/// </summary>
public sealed class InnateInternalsSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedInternalsSystem _internals = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InnateInternalsComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<InnateInternalsComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnMapInit(Entity<InnateInternalsComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<InternalsComponent>(ent, out var internals))
            return;

        _container.EnsureContainer<Container>(ent, ent.Comp.ContainerId);

        var tool = SpawnInContainerOrDrop(ent.Comp.BreathTool, ent, ent.Comp.ContainerId);
        var tank = SpawnInContainerOrDrop(ent.Comp.GasTank, ent, ent.Comp.ContainerId);

        ent.Comp.BreathToolEntity = tool;
        ent.Comp.GasTankEntity = tank;
        Dirty(ent);

        _internals.ConnectBreathTool((ent, internals), tool);

        // The tank never ends up in hands or inventory, so hand out its internals toggle directly.
        if (TryComp<GasTankComponent>(tank, out var gasTank))
        {
            _actions.AddAction(ent, ref gasTank.ToggleActionEntity, gasTank.ToggleAction, tank);
            Dirty(tank, gasTank);
        }
    }

    private void OnShutdown(Entity<InnateInternalsComponent> ent, ref ComponentShutdown args)
    {
        if (TryComp<InternalsComponent>(ent, out var internals) && ent.Comp.BreathToolEntity is { } tool)
            _internals.DisconnectBreathTool((ent, internals), tool, forced: true);

        QueueDel(ent.Comp.BreathToolEntity);
        QueueDel(ent.Comp.GasTankEntity);

        ent.Comp.BreathToolEntity = null;
        ent.Comp.GasTankEntity = null;
        Dirty(ent);
    }
}
