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
| Cost | 5 |
| Excludes | `Insulated`, `Heatresistant` |
| Blocked for | `IPC`, `Synth`, anything with `BorgChassis` |

Upsides:

* Shock damage halved, Blunt and Slash reduced by 15%.
* Built-in internals: an `integrated rebreather` and an `integrated air reservoir` (3 L, ~18 minutes)
  spawn inside you and are wired into your `Internals`, so internals work with no mask and no carried
  tank. The reservoir's toggle action is granted directly since it never sits in an inventory slot.
* Hunger and thirst never decay — the frame sustains its occupant.

Downsides:

* 10% slower at a walk, 15% slower at a sprint; the rubber is heavy.
* Heat damage +40%, Caustic damage +30%; latex melts and dissolves.
* Your mouth is sealed: you can never eat or drink anything, including chemicals and medicine.

## New files

* `Content.Shared/_HL/Body/InnateInternalsComponent.cs`, `Content.Server/_HL/Body/InnateInternalsSystem.cs` —
  spawns the breath tool and gas tank into an `innate_internals` container on the entity, connects the
  breath tool to `InternalsComponent`, and hands out the tank's internals toggle action.
* `Content.Shared/_HL/Nutrition/SealedMouthComponent.cs`, `Content.Server/_HL/Nutrition/SealedMouthSystem.cs` —
  cancels `IngestionAttemptEvent` and popups the reason.
* `Content.Shared/_HL/Movement/StaticSpeedModifierComponent.cs`, `Content.Shared/_HL/Movement/StaticSpeedModifierSystem.cs` —
  a flat walk/sprint multiplier applied through `RefreshMovementSpeedModifiersEvent`.
* `Resources/Prototypes/_HL/Entities/Objects/Misc/innate_internals.yml` — the two internal entities.
* `Resources/Locale/en-US/_HL/nutrition/sealed-mouth.ftl` — the ingestion popup.

## Edits to existing files

`Resources/Prototypes/_HL/Traits/Physical.yml`:

```yaml
- type: trait
  id: Drone
  name: drone-name
  description: drone-text
  category: Physical
  cost: 5
  speciesBlacklist:
  - IPC
  - Synth
  blacklist:
    components:
    - BorgChassis
  mutuallyExclusiveTraits:
  - Insulated
  - Heatresistant
  replaceComponents: true
  components:
  - type: InnateInternals
  - type: SealedMouth
  - type: StaticSpeedModifier
    walkModifier: 0.9
    sprintModifier: 0.85
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
          Shock: 0.5
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
drone-text = Your body is sealed in a seamless layer of drone-grade latex. The rubber cushions impacts, dulls blades and insulates you against half of all electrical shocks. An integrated rebreather and air reservoir let you run internals without a mask or tank, and the frame feeds you, so you never hunger or thirst. In exchange the heavy rubber weighs you down, slowing you by 10% at a walk and 15% at a sprint, your mouth is sealed shut, leaving you unable to eat or drink anything, and the latex melts and dissolves with ease, leaving you far more vulnerable to heat and caustic chemicals.
```

## Verification

Authored against the SandwichStation-HL mirror of HardLight (HardLightSector/HardLight itself was not
reachable). `dotnet build Content.Server` reports 0 errors and `Content.YAMLLinter` reports
"No errors found". Not yet tested in a running round.
