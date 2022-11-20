# Voczel v1

Easy and Customizable voice assistant.

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://choosealicense.com/licenses/mit/)
[![Downloads](https://img.shields.io/github/downloads/bitmap7487/VOCzel-v1/total.svg)](https://github.com/bitmap7487/VOCzel-v1/releases)
## Download

🟢 **[Stable release](https://github.com/bitmap7487/YoutubeDownloader/releases/latest)**


## Features

- Has prebuild commands.
- Uses custom commands which can be programmed in C#.
- Custom commands get imported directly.


## Prebuild Commands
- Voczel (Starts the Listener and waits for its command)
- Stop Listening (Stop the listener)
- Shutdown (Shuts down Voczel)
- Mute (Mutes Voczel)
- Unmute (Unmutes Voczel)
- Reload Commands (Reloads all custom commands)
- Hide (Hides Voczel into your system tray)
- Unhide shows Voczel on your screen)

## Custom Commands

### How it works
Make a .cs file in the Commands directory.\
After making that, Restart Voczel or ask Voczel to "Reload Commands"

### Example
Close all Chrome processes:
```cs
using System;
using System.Diagnostics;
using System.Speech.Synthesis;
class MyScript
{

    static SpeechSynthesizer speech = new SpeechSynthesizer();

    public void ExecuteAFunction()
    {

        Process[] chromeInstances = Process.GetProcessesByName("chrome");

        foreach (Process p in chromeInstances) {
            p.Kill();
        }
        speech.SpeakAsync("Closed all chrome windows for you.");
    }
}
```
