@echo off
echo 开始生成项目...
SET PATH=%PATH%;C:\Program Files\WinRAR
cd /d %~dp0
rd /s /q Release
md Release
xcopy /i /y ..\Debug4KSO\config .\Release\config
copy /y ..\Release\Quick.exe .\Release\Quick.exe
copy /y ..\Release\QuickUpdate.exe .\Release\QuickUpdate.exe
copy /y ..\Release\QuickUI.dll .\Release\QuickUI.dll
md Release\plugins\3rdparty\everything
copy /y ..\Release\plugins\quickplugin.dll .\Release\plugins\quickplugin.dll
copy /y ..\Release\plugins\3rdparty\everything\everything.exe .\Release\plugins\3rdparty\everything\everything.exe
copy /y ..\Release\plugins\3rdparty\everything\everything32.dll .\Release\plugins\3rdparty\everything\everything32.dll
copy /y ..\Release\plugins\3rdparty\everything\everything64.dll .\Release\plugins\3rdparty\everything\everything64.dll

cd Release
rar a -r Quick.rar

echo 完成
pause