## [Unreleased]

## [3.2.0] - 2026-5-25

### Added

- Hash field in Polity.Faction for easier Jobs integration

## [3.1.0] - 2026-5-24

### Added

- Factions in Polity.Manager can now be added and removed

### Fixed

- Faction field being overriden in multi select mode
- Faction being set in prefab mode

## [3.0.0] - 2026-4-14

### Added

- Unity modernization by switching to Unity Input System and URP graphics
- IMember interface which can be used instead of Member
- Polity.Faction class as the basis for getting faction names and relationships
- Polity.Leader class to control groups of Polity Members
- PolityFormation class to organize formations for the Polity Leader

### Removed

- Removed Family system

### Changed

- Polities, Classes and Factions have been simplied to Faction.
- Changed namespace from KL to Polity and renamed classes.

## [2.1.0] - 2025-5-11

### Changed

- CheckPolityRelation to CheckRelation
- ChangePolityRelation to ChangeRelation
- PolityReader Struct encapsulated and can only be modified by SetPolity()

### Fixed

- PolityReader dropdown fields not updating text when its polityStruct is changed

## [2.0.0] - 2025-03-14

### Added

- `PolityReader.cs` serializable class, which separates the polity dropdown from PolityMember, allowing the dropdown to be used in other classes

### Changed

- PolityRelation[,] PolityRelationMatrix renamed to RelationMatrix
- SerializePolityRelationMatrix() and DeserializePolityRelationmatrix() renamed to SerializeRelationMatrix() and DeserializeRelationMatrix()

### Fixed

- Null reference on a member when a PolityMember belonging to their family is destroyed

### Removed

- isPolityLeader, isClassLeader, and isFaction leader booleans

## [1.0.1] - 2024-08-21
  
### Changed
  
- GetPolityRelation() to CheckPolityRelation()
- ModifyPolityRelation() to ChangePolityRelation()

### Fixed

- PolityRelationMatrix not being initialized with a size at runtime

## [1.0.0] - 2024-08-06

- Initial upload
