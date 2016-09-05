$destinationFolder = "$env:STAGING_DIR\ReleaseContents\Plugins\"
if (!(Test-Path -path $destinationFolder)) {New-Item $destinationFolder -Type Directory}
Copy-Item $env:APPVEYOR_BUILD_FOLDER\LSystem\bin\Release\PixelsForGlory.ComputationalSystem.LSystem.dll -Destination $destinationFolder -Force 
