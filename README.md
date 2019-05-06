# Virtual Organ - PC service

This repository contains an ASP.NET Core API service that allows remote
control of a running [Hauptwerk][1] instance. Hauptwerk is a virtual
pipe organ software.

Supported commands:
* start Hauptwerk instance after audio card is active
* shutdown Hauptwerk and PC
* reset MIDI/audio stack
* get status of Hauptwerk (instance and MIDI/audio status)

Drafted commands, to be implemented:
* playback (start/stop) of a MIDI file in Hauptwerk from an archive of MIDI files

### MIDI interface
Communication between the service and Hauptwerk is achieved via MIDI messages
sent and received over a virtual MIDI device.
This component exploits [__virtualMIDI__ driver][2] by Tobias Erichsen.
It needs to be installed separately by installing [_loopMIDI_][3] or
_rtpMIDI_, from the author’s website. Mr. Erichsen has been so kind to allow
this not commercial software to link to the Windows driver _for free_.

[1]: https://www.hauptwerk.com
[2]: http://www.tobias-erichsen.de/software/virtualmidi.html
[3]: http://www.tobias-erichsen.de/software/loopmidi.html
