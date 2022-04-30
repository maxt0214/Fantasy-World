# MMORPG
<div id = "Top"> </div>
## FantasyWorld:
Fantasy World is a MMORPG that I made for sake of learning how to immplement complex game systems in a MMO game.

### Check It Out Here:

[![Watch the video](/Img/FantasyWorld1.jpg)](https://youtu.be/zCu0-mWWpvc)

[![Watch the video](/Img/FantasyWorld2.jpg)](https://youtu.be/EdqBH6APfkg)

Source Code can be found: [MMORPG](/PersonalProjects/MMORPG/).

## Technical Details
### Overall Framework
Deployed Manager-Model-View-Service architecture to manage the project, supporting 6 basic systems and 14 game systems
Followed a data driven approach to scale production pace, avoiding programming when adding some new game contents
Used event queue system to coordinate distinct systems, resulting a manageable software structure
### Network
Transferred messages using TCP/UDP and customized application protocol based on Google Protobuf
Used thread pool to handle network messages concurrently on multiple threads, boosting number of users served simultaneously
Optimized on Application layer by packet merging, packet delay, message queue to drop unnecessary network transitions
Relocated some data fields from application protocol to local machine. Compressed protocol overhead by 10-20%
Authenticated packets based on custom session management,  credential verification, and data validation to protect user account and prevent player cheating
### Database
Pumped SQL operation performance using asynchronous DB operations and DB caching with Entity Framework
Designed and implemented an efficient SQL data schema, and saved DB size by 20-30%
### Asset Management
Preloaded and cached resources to better game performance and memory usage. Avoided loading resources in 90% scenarios
Loaded resources with async operations for a smoother gameplay
### Development Tools
Automated character and level designs with Unity Editor and data-driven model
Pipelined local data preprocessing and importation from various locations and formats
Pipelined compilation of common libraries after editing to update at once on client and server
### Entities
Synchronized game entities among clients and server with interpolation algorithms for a better gameplay
Designed and realized each entity schema to communicate entities between clients and the server
Handled player inputs and AI to reflect on entity moves
Implemented Character animation using Unity animator and state machine
Validated entity transforms based on grid map generated from in-game navmesh
### UI
Integrated game UIs with existing artwork to provide a interactive gameplay
Provided a UI system to resolve common functionalities like window pop-up/close-down, message box, and etc
Cached UI resources to shorten UI pop up time
### NPC
Defined NPC behaviors based on local data
Designed NPC system based on event queue design pattern, assisting immediate NPC behavior change
Wrote NPC stories to diversify gameplay
### Social
Designed and deployed a multi-channel chat system using bit-manipulation. Used message pool to manage global chat room
Implemented a team system that responds in a timely manner to empower multiplayer gameplay
Arranged a guild system to engage players in a community setting
Synchronized user online status to avoid false positive social requests between users
### Battle
Deployed interpolation algorithm and message queue to make up for the network latency
Managed special effects and animation for player attacks with object pool to optimize memory
Applied spatial petitioning and AOI to effectively limit actions taken only on interested entities and lower the packets sent by 50 - 90%
Defined statistics of player skills, buffs, attributes, and etc. to diversify gameplay
Designed distinct match rules on different levels for a better gameplay
### AI
Employed GOAP to define interesting AI behaviors
Managed Navmesh and Navmesh Agent to support pathfinding
Deployed FSM to manage AI states
Used Component design pattern to diversify AI easily
### Quest
Designed and implemented a quest system that concretely defined quest ordering to support a sensible storyline
Deployed s target tracking system for a meaningful gameplay
Defined a navigation system to free up unnecessary operations from the player
Supplied quest stories for a meaningful gameplay
Used Unity timeline to support game storyline
### Props
Defined various item categories and large set of in-game items for a always replenishing gameplay
Balanced item properties to supply a fair gameplay
