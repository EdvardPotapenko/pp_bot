// ------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated.
//
//     - To turn off auto-generation set:
//
//         [TeamCity (AutoGenerate = false)]
//
//     - To trigger manual generation invoke:
//
//         nuke --generate-configuration TeamCity --host TeamCity
//
// </auto-generated>
// ------------------------------------------------------------------------------

import jetbrains.buildServer.configs.kotlin.v2018_1.*
import jetbrains.buildServer.configs.kotlin.v2018_1.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2018_1.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2018_1.triggers.*
import jetbrains.buildServer.configs.kotlin.v2018_1.vcs.*

version = "2020.2"

project {
    buildType(Compile)
    buildType(Down)
    buildType(Up)

    buildTypesOrder = arrayListOf(Compile, Down, Up)

    params {
        select (
            "env.Verbosity",
            label = "Verbosity",
            description = "Logging verbosity during build execution. Default is 'Normal'.",
            value = "Normal",
            options = listOf("Minimal" to "Minimal", "Normal" to "Normal", "Quiet" to "Quiet", "Verbose" to "Verbose"),
            display = ParameterDisplay.NORMAL)
        select (
            "env.Configuration",
            label = "Configuration",
            description = "Configuration to build - Default is 'Debug' (local) or 'Release' (server)",
            value = "Release",
            options = listOf("Debug" to "Debug", "Release" to "Release"),
            display = ParameterDisplay.NORMAL)
        checkbox (
            "env.WipeDatabaseData",
            label = "WipeDatabaseData",
            description = "Check to wipe the database data",
            value = "False",
            checked = "True",
            unchecked = "False",
            display = ParameterDisplay.NORMAL)
        text (
            "env.ProjectName",
            label = "ProjectName",
            description = "The name of docker-compose project",
            value = "pp_bot",
            allowEmpty = true,
            display = ParameterDisplay.NORMAL)
    }
}
object Compile : BuildType({
    name = "Compile"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    steps {
        exec {
            path = "build.sh"
            arguments = "Restore Compile --skip"
        }
    }
    triggers {
        vcs {
            triggerRules = "+:**"
        }
    }
})
object Down : BuildType({
    name = "Down"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    steps {
        exec {
            path = "build.sh"
            arguments = "Down --skip"
        }
    }
})
object Up : BuildType({
    name = "Up"
    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }
    steps {
        exec {
            path = "build.sh"
            arguments = "Up --skip"
        }
    }
    triggers {
        vcs {
            triggerRules = "+:**"
        }
    }
    dependencies {
        snapshot(Down) {
            onDependencyFailure = FailureAction.FAIL_TO_START
            onDependencyCancel = FailureAction.CANCEL
        }
    }
})
