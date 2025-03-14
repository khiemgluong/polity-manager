## [Unreleased]

## [2.0.0] - 2025-03-15

### Added

- `PolityReader.cs` serializable class, which separates the polity dropdown from PolityMember, allowing the dropdown to be used in other classes

### Changed

- PolityRelation[,] PolityRelationMatrix renamed to RelationMatrix

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
