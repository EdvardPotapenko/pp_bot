package patches.buildTypes

import jetbrains.buildServer.configs.kotlin.v2018_1.*
import jetbrains.buildServer.configs.kotlin.v2018_1.buildSteps.exec
import jetbrains.buildServer.configs.kotlin.v2018_1.ui.*

/*
This patch script was generated by TeamCity on settings change in UI.
To apply the patch, change the buildType with id = 'Up'
accordingly, and delete the patch script.
*/
changeBuildType(RelativeId("Up")) {
    params {
        add {
            password("env.POSTGRES_PASSWORD", "credentialsJSON:5855d4d5-c1de-4d3d-849f-8ba2b77beffd", display = ParameterDisplay.HIDDEN, readOnly = true)
        }
    }

    expectSteps {
        exec {
            path = "build.sh"
            arguments = "Up --skip"
        }
    }
    steps {
        insert(1) {
            step {
                name = "Production docker-compose configuration"
                type = "MRPP_CreateTextFile2"
                param("system.dest.file", "%teamcity.build.checkoutDir%/src/docker-compose.override.yml")
                param("content", """
                    version: '3.7'
                    
                    services:
                      ppbot:
                        environment:
                          - ASPNETCORE_ENVIRONMENT=%env.ASPNETCORE_ENVIRONMENT%
                      postgres:
                        environment:
                          - POSTGRES_PASSWORD=%env.POSTGRES_PASSWORD%
                """.trimIndent())
            }
        }
        insert(2) {
            step {
                name = "Production database settings"
                type = "MRPP_CreateTextFile2"
                param("system.dest.file", "%teamcity.build.checkoutDir%/src/pp_bot.Server/dbsettings.Production.json")
                param("content", """
                    {
                        "ConnectionStrings": {
                            "DB_CONN_STR": "Host=pp_bot_postgres;Port=5432;UserId=pp_bot;Password=%env.POSTGRES_PASSWORD%;Database=pp_bot;CommandTimeout=300;"
                        }
                    }
                """.trimIndent())
            }
        }
    }
}
