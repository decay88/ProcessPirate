# ProcessPirate
...is a tool for "zombifying" another trusted process.

It accomplishes this task by setting debug privileges and then injecting a native 'proxy dll' into the target process,
which the host application then controls via a websocket on the local host.
The "zombie-process" then is under full control of the host and can be abused to do various things like opening another process'
memory and reading/writing to it. Using this technique the host can "proxy" his malicious actions thru the trusted process and can 
avoid getting caught by 3rd parties (figure1).

## ![Figure1 / how it works](https://github.com/GreenPIsoftware/ProcessPirate/blob/master/proc_pirate.png)


# DONT USE THIS FOR ANYTHING MALICIOUS/CRIMINAL! I made this to cheat in videogames!!
