param ($runtime, $v)

if($runtime -eq $null) 
{
    $runtime = "win-x64"
}

if($v -eq $null)
{
 $v="1.0"
}


Write-Host "Start publishing with version $v ... "

$framework = "netcoreapp3.1"

dotnet publish UserInterface --framework $framework -c Release --runtime $runtime -p:Version=$v --self-contained

$location = ".\bin\Release\$framework\$runtime\publish"  

Push-Location "C:\Program Files (x86)\Inno Setup 6\"
./iscc.exe /dMyAppVersion=$v /dLocation=$location /dRuntime=$runtime c:\users\neo_c\desktop\sliderpuzzlesolver\installermaker.iss

Pop-Location