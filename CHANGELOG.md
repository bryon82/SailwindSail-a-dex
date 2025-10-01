# Changelog

All notable changes to this project will be documented in this file.

## [v1.5.9] - 2025-10-01

### Fixed
- Bug in transit times where transits were not being tracked.

## [v1.5.8] - 2025-09-15

### Added
- New ports from 0.33 update to Ports Visited log.

### Updated
- Ports Visited log layout to accommodate new ports.

## [v1.5.7] - 2025-08-12

### Updated
- Assets and asset loader to decrease game startup time.

## [v1.5.6] - 2025-08-08

### Fixed
- Not being able to find the assets when using a mod manager.

## [v1.5.5] - 2025-08-06

### Fixed
- Bug when loading a game, turning back on an encounter type from random encounters, and then triggering that encounter.

## [v1.5.4] - 2025-05-13

### Added
- Islands/ports to Emerald Archipelago region for 0.32 update.

## [v1.5.3] - 2025-05-13

### Fixed
- Shimmertail count not increasing after 0.31 update. Name is now blue shimmertail.

## [v1.5.2] - 2025-05-08

### Fixed
- Bug introduced by RandomEncounters method name change.

### Updated
- Asset loader improving startup time.

## [v1.5.1] - 2025-05-07

### Fixed
- All ports visited notification bug.

### Added
- Handling of RandomEncounters settings fields changing to properties in the near future.

## [v1.5.0] - 2025-05-03

### Updated
- Major refactor of StatsUI and PortsVisitedUI logic.
- Improve encapsulation overall.

### Added
- Intense storm and fishing bonanza encounters to StatsUI.
- Dynamic add-remove of RandomEncounter stats.

### Changed
- Plugin name case to match mod name.

## [v1.4.5] - 2025-04-01

### Changed
- The way enabling/disabling logs works. You can now disable a log and save the game without it erasing your progress in the log.

## [v1.4.4] - 2025-03-23

### Added
- In Ports Visited log added ports for new islands added in update 0.30.

## [v1.4.3] - 2025-03-05

### Fixed
- Disabling dense fog or flotsam encounters in RandomEncounters mod causing text on StatsUI not to update properly.

## [v1.4.2] - 2025-03-03

### Added
- Total mass stat.

### Changed
- Stats log spacing.
- Cleaned up ReadMe text.

### Fixed
- Dense fog encounter stat not incrementing.

## [v1.4.1] - 2025-02-27

### Fixed
- Cargo mass calculation not including logs and lumber.

## [v1.4.0] - 2025-02-27

### Added
- Track the number of Dense Fog encounters from RandomEncounters.

### Changed
- Now track fastest transit between regions no matter what island you leave from or what island you moor at (Happy Bay excluded).

### Removed
- Dependency on SailwindModdingHelper.

## [v1.3.1] - 2024-09-18

### Added
- Weathered Storms to Lifetime stats.
- Fire Fish Lagoon fish to the fish caught log along with associated badges.

### Fixed
- Fire Fish Lagoon fish were missing from the list of fish which was also causing a bug when collecting one of those fish.

## [v1.3.0] - 2024-09-16

### Added
- ModSave Backups dependency to generate backups of the save data from this mod.
- Tie in with RandomEncounters if installed to track the number of Flotsam encounters and, if installed and controlled from RandomEncounters, SeaLife encounters.

### Changed
- The SaveContainer type and location with converter added.

## [v1.2.3] - 2024-09-04

### Changed
- Miles sailed calculation.

### Fixed
- Negative transit hours bug.
- Record underway time calculation bug.

## [v1.2.2] - 2024-08-31

### Added
- Config option to update miles sailed text on sleep.

## [v1.2.1] - 2024-08-29

### Added
- Miles sailed to lifetime stats.
- Ports visited to lifetime stats.

### Fixed
- NullRef when loading game off of ship and trying to sell an item.

## [v1.2.0] - 2024-08-27

### Added
- Stats & Transit UI.
- Notifications for badges and transit records.
- Notification sound.

### Changed
- Reorganized code.
- Refactored badge checks for ports visited.

## [v1.1.0] - 2024-08-13

### Added

- Badges for fish caught.
- Badges for ports visited.
- Assets for badges.

### Changed

- Ports visited bookmark will now shift left if fish caught UI is disabled.
- Refactored code to have external objects only accessed once instead of every time needed.
- UI have had badge locations added, fish caught UI text was shifted to accommodate space for badges.

## [v1.0.0] - 2024-08-09

### Added

- Fish Caught UI.
- Hide fish names in Fish Caught UI until first caught made configurable.
- Ports Visited UI.
- Hide port names in the Ports Visible UI until first visited made configurable.
- Both UIs can be disabled in config.
