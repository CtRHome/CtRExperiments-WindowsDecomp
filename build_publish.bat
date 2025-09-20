@echo off
cd CutTheRope.WindowsDX

echo For some reason, the exe file that built by command prompt will be larger than Visual Studio one. If you want the smaller exe, please use Visual Studio to publish.

dotnet restore
dotnet publish -p:PublishProfile=Properties/PublishProfiles/FolderProfile
pause