# Rubber Drone Trait

A physical character trait for the [HardLight](https://github.com/HardLightSector/HardLight) Space Station 14 fork:
your body is sealed in drone-grade latex.

Files are laid out at the paths they belong at in the HardLight repo, so they can be copied over the
checkout root. `patches/drone-trait.patch` additionally contains the edits to the two existing files.

## The trait

| | |
| --- | --- |
| ID | `Drone` |
| Category | `Physical` |
| Cost | 6 |
| Excludes | `Insulated`, `Heatresistant` |
| Blocked for | `IPC`, `Synth`, `Vox`, `Slime`, anything with `BorgChassis` |

Upsides:

* `Insulated` : the latex is a natural insulator, so electrical shocks do nothing.
* Blunt and Slash reduced by 15%.
* Built-in internals: an `integrated rebreather` and an `integrated air reservoir` (3 L, ~18 minutes)
  spawn inside you and are wired into your `Internals`, so internals work with no mask and no carried
  tank. The reservoir's toggle action is granted directly since it never sits in an inventory slot.
* Hunger and thirst never decay : the frame sustains its occupant.

Downsides:

* 10% slower at a walk, 15% slower at a sprint; the rubber is heavy.
* 2% chance per step to lose your footing and fall over (with a 45 second grace period after each
  fall), and standing back up takes 4 seconds instead of 1.
* Heat damage +40%, Caustic damage +30%; latex melts and dissolves.
* Your mouth is sealed: you can never eat or drink anything, including chemicals and medicine.

## New files

* `Content.Shared/_HL/Body/InnateInternalsComponent.cs`, `Content.Server/_HL/Body/InnateInternalsSystem.cs` :
  spawns the breath tool and gas tank into an `innate_internals` container on the entity, connects the
  breath tool to `InternalsComponent`, and hands out the tank's internals toggle action.
* `Content.Shared/_HL/Nutrition/SealedMouthComponent.cs`, `Content.Server/_HL/Nutrition/SealedMouthSystem.cs` :
  cancels `IngestionAttemptEvent` and popups the reason.
* `Content.Shared/_HL/Movement/StaticSpeedModifierComponent.cs`, `Content.Shared/_HL/Movement/StaticSpeedModifierSystem.cs` :
  a flat walk/sprint multiplier applied through `RefreshMovementSpeedModifiersEvent`.
* `Content.Shared/_HL/Movement/HeavyFootingComponent.cs`, `Content.Server/_HL/Movement/HeavyFootingSystem.cs` :
  accumulates distance from `MoveEvent` and rolls a knockdown once per step travelled, ignoring
  teleport-sized jumps and respecting a post-fall grace period.
* `Resources/Locale/en-US/_HL/movement/heavy-footing.ftl` : the stumble popup.
* `Resources/Prototypes/_HL/Entities/Objects/Misc/innate_internals.yml` : the two internal entities.
* `Resources/Locale/en-US/_HL/nutrition/sealed-mouth.ftl` : the ingestion popup.

## Edits to existing files

`Resources/Prototypes/_HL/Traits/lewd.yml`:

```yaml
- type: trait
  id: Drone
  name: drone-name
  description: drone-text
  category: Lewd
  speciesBlacklist:
  - IPC
  - Synth
  - Vox
  - Slime
  blacklist:
    components:
    - BorgChassis
  mutuallyExclusiveTraits:
  - Insulated
  - Heatresistant
  - Vampirism
  replaceComponents: true
  components:
  - type: Insulated
  - type: InnateInternals
  - type: SealedMouth
  - type: StaticSpeedModifier
    walkModifier: 0.8
    sprintModifier: 0.75
  - type: HeavyFooting
    stumbleChance: 0.02
  # Hauling yourself upright in a rubber suit takes a while.
  - type: LayingDown
    standingUpTime: 5
  # The frame feeds and waters its occupant, so a sealed mouth isn't a slow death sentence.
  - type: Hunger
    baseDecayRate: 0
  - type: Thirst
    baseDecayRate: 0
  - type: DamageProtectionBuff
    modifiers:
      dermal:
        id: DroneTrait
        coefficients:
          Blunt: 0.85
          Slash: 0.85
          Heat: 1.4
          Caustic: 1.3
```

`replaceComponents: true` is what lets the trait override the existing `Hunger` and `Thirst` components;
without it the trait system skips components the entity already has.

`Resources/Locale/en-US/_HL/traits/traits.ftl`:

```ftl
drone-name = Drone
drone-text = Your body is sealed in a layer of heavy, but seamless drone-grade latex. The rubber cushions impacts, dulls blades and insulates you against electrical shocks. An integrated rebreather and air reservoir let you run internals without a mask or tank, and the frame feeds you, so you never hunger or thirst.
```

`Content.Shared/Body/Systems/SharedInternalsSystem.cs` : `FindBestGasTank` only searches back,
suit storage, hands and pockets, so clicking the internals *alert* failed with "You are not wearing a
gas tank" even though the granted action worked. It now checks `InnateInternalsComponent` first:

```csharp
if (TryComp<InnateInternalsComponent>(user, out var innate) &&
    TryComp<GasTankComponent>(innate.GasTankEntity, out var innateGasTank) &&
    _gasTank.CanConnectToInternals((innate.GasTankEntity.Value, innateGasTank)))
{
    return (innate.GasTankEntity.Value, innateGasTank);
}
```
