#define MyAppName "Обновление"
#define MyAppVersion "1.0"
#define MyAppExeName "logOnOffService.exe"

#define use_dotnetfx40

[Setup]
AppId={{01D3E992-0FB7-42E5-8480-00907EACB6B0}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultGroupName={#MyAppName}
PrivilegesRequired=admin
DisableProgramGroupPage=yes
OutputBaseFilename=setup
AllowNoIcons=yes
Compression=lzma
SolidCompression=yes
AppendDefaultDirName=no
DefaultDirName={commonappdata}\{#MyAppName}
DisableDirPage=yes
DisableFinishedPage=yes

;Downloading and installing dependencies will only work if the memo/ready page is enabled (default behaviour)
DisableReadyPage=no
DisableReadyMemo=no

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"        
Name: "de"; MessagesFile: "compiler:Languages\German.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Files]
Source: "C:\logOnOffService\App\logOnOffService.exe"; DestDir: "{app}"; Flags: ignoreversion ; Permissions: everyone-full
Source: "C:\logOnOffService\App\MySql.Data.dll"; DestDir: "{app}"; Flags: ignoreversion ; Permissions: everyone-full
Source: "C:\logOnOffService\App\NLog.dll"; DestDir: "{app}"; Flags: ignoreversion ; Permissions: everyone-full
Source: "C:\logOnOffService\App\NLog.config"; DestDir: "{app}"; Flags: ignoreversion ; Permissions: everyone-full
Source: "C:\logOnOffService\App\settings.xml"; DestDir: "{app}"; Flags: ignoreversion ; Permissions: everyone-full
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

;[Icons]
;Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Parameters: "--install"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[UninstallRun]
Filename: "{app}\{#MyAppExeName}"; Parameters: "--uninstall"

;[Registry]
;Root: HKCU; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "logOnOff"; ValueData: """{app}\logOnOff.exe"""; Flags: uninsdeletevalue

[Code]
// shared code for installing the products
#include "scripts\products.iss"
// helper functions
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"


// actual products
#ifdef use_dotnetfx40
#include "scripts\products\dotnetfx40client.iss"
#endif

function InitializeSetup(): boolean;
begin
	// initialize windows version
	initwinversion();
  
#ifdef use_dotnetfx40
	if (not netfxinstalled(NetFx40Client, '') and not netfxinstalled(NetFx40Full, '')) then
		dotnetfx40client();
#endif

	Result := true;
end;