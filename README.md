# pp_bot
## open source telegram bot for growing your pp

> Sometimes **we all just need** our ego to **GROW** - so what a better way you can achieve this - than growing your virtual pp with this *brand new open source c# .net core telegram bot.*

This project is powered by ASP.NET Core 5.

**Build status** <br>
**TeamCity Debian** ![TeamCity Debian Build Status](https://img.shields.io/teamcity/build/s/PpBot_Compile?server=https%3A%2F%2Ftc.vova-lantsov.dev&style=for-the-badge)



Not to mention, we are working to make this dude [GDPR](https://en.wikipedia.org/wiki/General_Data_Protection_Regulation) compliant!

**Prerequisites**

1. [Docker](https://docs.docker.com/engine/install/ubuntu/)
2. [Dotnet SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

**Installation guide**

1. Clone this repo via `git clone https://github.com/EdvardPotapenko/pp_bot`
2. Go to src directory
3. Restore packages via `dotnet restore`
4. Navigate to the project's root directory (where the docker-compose.yml file is located).
5. Set BOT_KEY and DEV_KEY environment variables in docker-compose.yml
6. To build the containers, run the following command: `docker-compose build`
7. To run the already built containers, run: `docker-compose up`

<p>You can also build the containers and run them simultaneously: `docker-compose up --build` </p>

<p>If you want the containers to run in the background (even if you close the terminal), append the -d argument to the end of a command: `docker-compose up -d` or `docker-compose up --build -d`</p>

> To shut the containers down while they are working in the background, run: docker-compose down

Big thanks to [Vova Lantsov](https://github.com/vova-lantsov-dev) for help with docker and installation guide

Kudos to @kraft28_bot for the original idea

