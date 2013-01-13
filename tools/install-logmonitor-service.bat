@ECHO Off

pushd %~dp0

C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe LogMonitor.exe
net start LogMonitor

popd

pause