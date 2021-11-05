# Nick All-Star Brawl: Custom Stages Mod

Hey y'all, been chipping away at this mod and thought it wouldn't be a bad idea to open-source it for anyone else to help.
All dependency .dll's aren't included since they store the code for the game and distributing those is morally ambiguous and probably illegal lol


### How are stages loaded?
Stages are loaded via asset bundles, and accessed through the game's root path. Specifically, the path they access is "%gameroot%/Stages/%stagename%" and both "%stagename%.nick" and "%stagename%.nickscene."
.nick files store the stage's prefabs, thumbnails, and other information. The .nickscene stores the scene that plugs into NASB's Agent system.

### How do I make stages?
There's no public way to generate these asset bundles, but I'm working on getting the tools to a point where they can be shared with people easily. For now, you can [download the example stages here](https://drive.google.com/file/d/1M6dka22-oVe-NUlYbFFEFPRmryaxtlGu/view?usp=sharing).

### How do I play stages?
Once you put the .nick and .nickscene files in the right directories, it's just like loading any other in-game stage. Go to the Stage Select and select one of the custom stages. Be aware that any custom stages you have will also load in Classic mode too.
