# Changelog

All notable changes to this project will be documented in this file.

## [v1.4.0] - 2024-02-27

### Added
- Track the number of Dense Fog encounters from RandomEncounters

### Changed
- Now track fastest transit between regions no matter what island you leave from or what island you moor at (Happy Bay excluded).

### Removed
- Dependency on SailwindModdingHelper

## [v1.3.1] - 2024-09-18

### Added
- Weathered Storms to Lifetime stats
- Fire Fish Lagoon fish to the fish caught log along with associated badges

### Fixed
- Fire Fish Lagoon fish were missing from the list of fish which was also causing a bug when collecting one of those fish

## [v1.3.0] - 2024-09-16

### Added
- ModSave Backups dependency to generate backups of the save data from this mod
- Tie in with RandomEncounters if installed to track the number of Flotsam encounters and, if installed and controlled from RandomEncounters, SeaLife encounters

### Changed
- The SaveContainer type and location with converter added

## [v1.2.3] - 2024-09-04

### Changed
- Miles sailed calculation

### Fixed
- Negative transit hours bug
- Record underway time calculation bug

## [v1.2.2] - 2024-08-31

### Added
- Config option to update miles sailed text on sleep

## [v1.2.1] - 2024-08-29

### Added
- Miles sailed to lifetime stats
- Ports visited to lifetime stats

### Fixed
- NullRef when loading game off of ship and trying to sell an item

## [v1.2.0] - 2024-08-27

### Added
- Stats & Transit UI.
- Notifications for badges and transit records.
- Notification sound

### Changed
- Reorganized code
- Refactored badge checks for ports visited

## [v1.1.0] - 2024-08-13

### Added

- Badges for fish caught.
- Badges for ports visited.
- Assets for badges.

### Changed

- Ports visited bookmark will now shift left if fish caught UI is disabled.
- Refactored code to have external objects only accessed once instead of everytime needed.
- UI have had badge locations added, fish caught UI text was shifted to accomodate space for badges.

## [v1.0.0] - 2024-08-09

### Added

- Fish Caught UI.
- Hide fish names in Fish Caught UI until first caught made configurable.
- Ports Visited UI.
- Hide port names in the Ports Visible UI until first visited made configurable.
- Both UIs can be disabled in config.
