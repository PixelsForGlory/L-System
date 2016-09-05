# Run unit test
Copy-Item $env:APPVEYOR_BUILD_FOLDER\LSystem\bin\Release\PixelsForGlory.ComputationalSystem.LSystem.* -Destination $env:APPVEYOR_BUILD_FOLDER\LSystemTest\bin\Release\ -Force
vstest.console $env:APPVEYOR_BUILD_FOLDER\LSystemTest\bin\Release\LSystemTest.dll /Settings:$env:APPVEYOR_BUILD_FOLDER\LSystemTest\test.runsettings /logger:Appveyor
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode)  }