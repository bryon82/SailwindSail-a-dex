# Sail-a-dex

This Sailwind mod is intended to keep track of various player activities along with badges that are acquired upon meeting certain criteria. Below are the currently tracked items and what is configurable.

### Requires

* [BepInEx 5.4.23](https://github.com/BepInEx/BepInEx/releases)
* [ModSaveBackups 1.1.1](https://thunderstore.io/c/sailwind/p/RadDude/ModSaveBackups/)

## Fish Caught Log

![Screenshot of the Fish Caught UI](https://github.com/bryon82/Sail-a-dex/blob/main/Screenshots/fishCaughtUI.jpg)  
A UI showing how many fish that you caught separated by the different types.  
A fish is registered as caught once you reel it in and collect it.  
Badges are acquired upon catching 25, 50, and 100 of each type, 50, 250, and 500 total, and also for catching at least one of each type.  
Access this log by selecting the "fish caught" bookmark in the player log.  

## Ports Visited Log

![Screenshot of the Ports Visited UI](https://github.com/bryon82/Sail-a-dex/blob/main/Screenshots/portsVisitedUI.jpg)  
A UI showing which ports you have visited separated by region.  
A port is registered as visited once you enter the area where mission goods are normally delivered.  
Badges are acquired upon visiting all ports within a region as well as visiting every port.  
Access this log by selecting the "ports visited" bookmark in the log.  

## Stats & Transit Log

![Screenshot of the Stats & Transit UI](https://github.com/bryon82/Sail-a-dex/blob/main/Screenshots/statsUI.jpg)  
A UI showing various stats and transit times between regions.  
Cargo mass is is the mass some of all crates, barrels, and packages.  
Cargo mass stat "Record" will be recorded once you unmoor from the dock.  
Total mass stat is the total mass of the ship and operates the same as cargo mass  
Underway "Record" is the longest time you are out at sea before pulling into port.  
Transit time "Record" is the fastest time you made a transit.  
The starting time for transits are tracked from the last island you were moored at from a region that you leave. This time will be used for when you moor at any island in other regions. Happy Bay is ignored for calculating fastest transits.  
(only the first time mooring at a city in another region will be recorded, i.e. You leave Gold Rock City, only the first time reaching Crab Beach will the AA to EA transit be recorded).  
If you use PassageDude mod and book travel or teleport via console that will reset your currently tracked transits.  
If you have RandomEncounters installed, Flotsam encounters and Dense Fog encounters if enabled will be tracked if.  
If you have the SeaLifeMod installed and controlled through RandomEncounters, the SeaLife encounters will be tracked as well.  

## Notifications

Notifications will pop up along with a ship bell sound on badges earned and fastest transit times recorded.  


## Configurable in BepInEx config

* By default the fish names are hidden before being caught for the first time, this can be disabled.
* By default port names are visible, can be configured to be hidden until visited for the first time.
* Notifications can be disabled.
* Notification sound can be adjusted and disabled.
* By default Miles sailied text is updated once moored at a port, this can be changed to be updated also when going to sleep or in real time.
* All Logs can be disabled individually.

## Installation

Place the Sail-a-dex folder (not the contents, the whole thing) into the Sailwind/BepInEx/Plugins folder.  
If upgrading from v1.0.0 delete the old Sail-a-dex.dll from the BepInEx/plugins folder first.  
