# Features

* Displays the serveur population infos
* Tweak the information displayed in the chat

![](https://imgur.com/DmjZyN8.png)

# Permissions

* `popcommand.use` - Allows player to use the /pop command **if requiered in config**
* `popcommand.admin` - Allows player to use the /apop command

# Commands

* `pop` - Displays the population infos
* `apop` - Displays the full population infos (for admins)

# Configuration

Default configuration:

```json
{
  "Use permission for lambda players": false,
  "Broadcast to every player on /pop": false,
  "Chat default avatar": 0,
  "Display Options": {
    "Show player count": true,
    "Show server slots": true,
    "Show sleepers": false,
    "Show joining players": false,
    "Show players in queue": false
  }
}
```

* `Use permission for lambda players` (**true** or **false**) - If true, players will need the `popcommand.use` permission to use the `/pop` command
* `Broadcast to every player on /pop` (**true** or **false**) - If true, the `/pop` command will be broadcasted to every player on the server
* `Chat default avatar` (**ulong***) - The avatar displayed in the chat when using the `/pop` command *(to replace the rust logo)*

**WARNING:** 
You can tweak theses options but make sure to edit the **lang file** to match your changes

* `Display Options` - Options to tweak the information displayed in the chat
  * `Show player count` (**true** or **false**) - If true, the player count will be displayed
  * `Show server slots` (**true** or **false**) - If true, the server slots will be displayed
  * `Show sleepers` (**true** or **false**) - If true, the sleepers count will be displayed
  * `Show joining players` (**true** or **false**) - If true, the joining players count will be displayed
  * `Show players in queue` (**true** or **false**) - If true, the players in queue count will be displayed

# Localization

Default english translation:

```json
{
  "PopCommand.ChatMessage": "Players onlines: {0}/{1}",
  "PopCommand.AdminMessage": "Players onlines: {0}/{1} | Sleeping: {2} | Joining: {3} | Queued: {4}",
  "PopCommand.PermissionDeny": "You are not allowed to run this command!",
  "PopCommand.AdminPermissionDeny": "Only administrators can run this command!",
  "PopCommand.Error": "The config/lang file contains some errors!"
}
```

According to the `Display Options` in the config, you can tweak theses messages to match your changes.

**PLEASE MAKE SURE TO KEEP THE {x} PARAMETERS STARTING FROM 0 AND WITH THE ASCENDING ORDER!!**

If there is more **{x}** parameters than configure in the `Display Options`, the message will not be displayed and the error message will appear.

# Credits

* **[HandyS11](https://github.com/HandyS11)** - Author