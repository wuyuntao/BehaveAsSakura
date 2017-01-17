$SolutionDir = "$PSScriptRoot\.."

$Runner = "$SolutionDir\packages\NUnit.ConsoleRunner.3.6.0\tools\nunit3-console.exe"

$TestExe = "$SolutionDir\BehaveAsSakuraTests\bin\Debug\BehaveAsSakuraTests.dll"

& $Runner $TestExe
