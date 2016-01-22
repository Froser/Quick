@echo off
echo 开始生成项目...
SET PATH=%PATH%;C:\Program Files\WinRAR
cd /d %~dp0
rd /s /q Release4KSO
md Release4KSO
xcopy /i /y ..\Debug4KSO\config .\Release4KSO\config
copy /y ..\Release4KSO\Quick.exe .\Release4KSO\Quick.exe
copy /y ..\Release4KSO\QuickUpdate.exe .\Release4KSO\QuickUpdate.exe
copy /y ..\Release4KSO\QuickUI.dll .\Release4KSO\QuickUI.dll
md Release4KSO\plugins\3rdparty\everything
copy /y ..\Release4KSO\plugins\quickplugin.dll .\Release4KSO\plugins\quickplugin.dll
copy /y ..\Release4KSO\plugins\3rdparty\everything\everything.exe .\Release4KSO\plugins\3rdparty\everything\everything.exe
copy /y ..\Release4KSO\plugins\3rdparty\everything\everything32.dll .\Release4KSO\plugins\3rdparty\everything\everything32.dll
copy /y ..\Release4KSO\plugins\3rdparty\everything\everything64.dll .\Release4KSO\plugins\3rdparty\everything\everything64.dll

cd Release4KSO
rar a -r Quick.rar

echo 完成
pause