# pp_bot
## open source telegram bot for growing your pp

> Sometimes **we all just need** our ego to **GROW** - so what a better way you can achieve this - than growing your virtual pp with this *brand new open source c# .net core telegram bot.*

Not to mention, we are working to make this dude [GDPR](https://en.wikipedia.org/wiki/General_Data_Protection_Regulation) compliant!

This project is powered by ASP.NET Core 5 and Nuke build automation system.

### Terms of use

By using this project or its source code, for any purpose and in any shape or form, you grant your implicit agreement to all the following statements:

* You condemn Russia and its military aggression against Ukraine
* You recognize that Russia is an occupant that unlawfully invaded a sovereign state
* You support Ukraine's territorial integrity, including its claims over temporarily occupied territories of Crimea and Donbas
* You reject false narratives perpetuated by Russian state propaganda

Glory to Ukraine! 🇺🇦

### Build status
| Service | Status |
| ------- | ------ |
| TeamCity Debian | ![TeamCity Debian Build Status](https://img.shields.io/teamcity/build/s/PpBot_Compile?server=https%3A%2F%2Ftc.vova-lantsov.dev&style=for-the-badge) |

### Prerequisites
1. Install either the Visual Studio, the Visual Studio Code or the JetBrains Rider editor.
2. Install the [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) on your PC.
3. Clone the project repository locally.
4. Install the [Docker for Windows](https://hub.docker.com/editions/community/docker-ce-desktop-windows/) on your PC (by pressing the _Get Docker_ button).
5. Ensure that `SVM mode` / `hypervisor` is enabled in your motherboard's BIOS.

### How To Run
To run this project locally, open a terminal (for example, `cmd.exe` or `Git Bash`) and navigate to the project's root directory (where the `docker-compose.yml` file is located).  
To build the containers, run the following command: `docker-compose build`  
To run the already built containers, run: `docker-compose up`  
You can also build the containers and run them simultaneously: `docker-compose up --build`  
If you want the containers to run in the background (even if you close the terminal), append the `-d` argument to the end of a command:
`docker-compose up -d` or `docker-compose up --build -d`  
To shut the containers down while they are working in the background, run:
`docker-compose down`  
To shut the containers down and destroy all the data stored in a database, run:
`docker-compose down -v`

There is no HTTPS configured for the local web-host.

### Other
Big thanks to [Vova Lantsov](https://github.com/vova-lantsov-dev) for help with docker and installation guide.  
Kudos to @kraft28_bot for the original idea.
