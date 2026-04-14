**Polity Manager - Manage Factions & Formations**

- [Description](#description)
- [Quickstart](#quickstart)
  - [Video Tutorial](#video-tutorial)
- [Public APIs](#public-apis)
  - [PolityManager.cs (Polity.Manager)](#politymanagercs-politymanager)
    - [ChangeRelation()](#changerelation)
    - [CheckRelation()](#checkrelation)
    - [SerializeRelationMatrix()](#serializerelationmatrix)
    - [DeserializeRelationMatrix()](#deserializerelationmatrix)
  - [PolityFaction.cs (Polity.Faction)](#polityfactioncs-polityfaction)
  - [Events](#events)
    - [OnRelationChange](#onrelationchange)
    - [OnLeaderChange](#onleaderchange)
    - [OnFactionChange](#onfactionchange)
- [PolityManager.cs ContextMenus](#politymanagercs-contextmenus)
  - [Reset Polity Relation Matrix](#reset-polity-relation-matrix)
  - [Load Polity Relation Matrix](#load-polity-relation-matrix)
- [Credits](#credits)

## Description

Polity Manager is an editor based tool designed to manage relations between polities.

The PolityManager singleton (PM) contains a _Faction Relation Matrix_, a matrix table that displays the relation of one polity to another based on their position, similar to the Unity physics collision matrix.

![Polity Relation Matrix](./Assets/Documentation/Faction%20Relation%20Matrix.png)

>The Red Team is neutral to the Blue Team and allied to the Empire, but the Blue Team are enemies to the Empire and allied to the Shogunate.

To retrieve these factions from an object, you can attach the `Polity.Member` monobehaviour or have a class implement the `IMember` interface (the latter is recommended). This will provide you with a dropdown field listing all the factions that you have created in the Manager.

These utilizes the `Faction` class which is the main way GameObjects can communicate with Polity Manager. You can utilize this class to build your own classes with this as a field.

Polity Manager is suited for games that needs to manage various groups of NPCs, especially when these relationships are a bit more complex, such as when one NPC needs to react to an enemy of one or more allied NPCs. However, it can also be applicable to simple teams.

It is also designed to be very bare bones so that you can modify it more easily to suit your game's specific needs. It's less of a all encompassing solution and more of a framework.

## Quickstart

### [Video Tutorial](https://www.youtube.com/watch?v=f2wm8g-1v8A)

This should demonstrate a very basic implementation of how the PolityManager can control NavMeshAgents with a PolityMember that can react to relationship changes based on their current polity.

You can open the `PolityNPC.cs` class inside of Example/Scripts to get a better idea of how the class subscribes to events and how it calls public PolityManager methods.

## Public APIs

All classes in this package is under the `Polity` namespace.

### PolityManager.cs (Polity.Manager)

All public methods can be called from this PolityManager Singleton, referenced as `PM`, for example PM.ModifyPolityRelation();

#### ChangeRelation()

Sets a new relationship status between two polities based on their names, adjusting their relation to either Neutral, Allies, or Enemies.
If the polities matched, the `OnRelationChange` event will be invoked to notify all subscribers of the relation change.

| Parameter          | Type             | Description |
|--------------------|------------------|-------------|
| `polityMember`     | `PolityMember`   | The member of the polity initiating the relationship change. |
| `theirPolityName`  | `string`         | The name of the polity that is targeted for the relationship change, retrieved from `polityName` in `PolityMember`. |
| `factionRelation`  | `Polity.Relation` | The new relation to set; can be `Neutral`, `Allies`, or `Enemies`.|

#### CheckRelation()

Compares the PolityRelation of the polityName to the second polityName and returns the enum that indicates their PolityRelation if found.
The overload method replaces the PolityMember parameters as strings representing the polityName.

| Parameter          | Type             | Description |
|--------------------|------------------|-------------|
| `polityMember`     | `PolityMember`   | The member of the polity initiating the relationship comparison. |
| `theirPolityMember`  | `PolityMember`         | The name of the polity that is targeted for comparison, retrieved from `polityName` in `PolityMember`. |

**Returns**
The `Polity.Relation` enum value, which can be `Neutral`, `Allies` or `Enemies`.

#### SerializeRelationMatrix()

Serializes the `PolityRelation[,]` matrix and sets the serialized `polityRelationMatrixString` to its return value.

| Parameter          | Type             | Description |
|--------------------|------------------|-------------|
| `RelationMatrix`     | `Polity.Relation[,]`   | The 2D PolityRelation matrix. This can be omitted which will use the Singleton's `RelationMatrix`. |

**Returns**
The `string` of that serialized matrix.

#### DeserializeRelationMatrix()

Deserializes a string representing the `PolityRelation[,]` matrix.

| Parameter          | Type             | Description |
|--------------------|------------------|-------------|
| `json`     | `string`   | This can be omitted which will then use the Singleton's `relationMatrixString`. |

**Returns**
The `Polity.Relation[,]` matrix which was deserialized from the string.


### PolityFaction.cs (Polity.Faction)


### Events

#### OnRelationChange

Invoked whenever `ModifyPolityRelation()` is called or when the _Polity Relation Matrix_ cell is clicked on.

#### OnLeaderChange

Invoked whenever a `PolityMember` is set to a new Polity as their leader with `SetAsPolityLeader()` or `SetPolityLeader()`.

#### OnFactionChange

Invoked whenever a `Faction` is created or removed with `AddFactionToPolity()` and `RemoveFactionFromPolity()`.


## PolityManager.cs ContextMenus

### Reset Polity Relation Matrix

Reset every relation to `Neutral`.

### Load Polity Relation Matrix

If in some case the serialized polity relation matrix did not load, this can manually deserialize & load it.

## Credits

PBR Sand054 texture
[ambientcg.com](https://ambientcg.com) - CC0 License

**Polity Manager** was developed by Khiem Luong ([github.com/khiemgluong](https://github.com/khiemgluong))
