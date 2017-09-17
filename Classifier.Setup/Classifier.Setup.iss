#define APP_NAME "Myoddweb Classifier"
#define APP_PUBLISHER "myoddweb.com"
#define APP_URL "http://www.myoddweb.com"
#define APP_DIR "{pf}\myoddweb\classifier"
#define APP_SOURCE "..\bin\"
#define APP_REG_NAME "myoddweb.classifier"
#define EVENT_LOG_NAME "myoddweb.classifier"

; we add the engine version number, just in case.
#define APP_DESC "Classify mail items into various categories as they arrive (engine:" + GetFileVersion( AddBackslash( APP_SOURCE) + 'Classifier.Engine.dll' ) + ')'

; set the version number based on the classifier version number.
#define APP_VERSION GetFileVersion( AddBackslash( APP_SOURCE) + 'myoddweb.classifier.dll' )

;
; No need to edit past here.
;
[Setup]
AppName={#APP_NAME}
AppVersion={#APP_VERSION}
VersionInfoVersion={#APP_VERSION}
AppCopyright=myoddweb.classifier
AppId={{A061504F-FDFA-40E6-A283-AA044CB0DFC8}
DefaultDirName={#APP_DIR}
AppPublisher={#APP_PUBLISHER}
AppPublisherURL={#APP_URL}
AppSupportURL={#APP_URL}
AppUpdatesURL={#APP_URL}
; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x86 x64
OutputBaseFilename="setup.{#APP_VERSION}"


[Files]
; x64
Source: "{#APP_SOURCE}\x64\Classifier.Engine.dll"; DestDir: "{app}\x64"; Flags: ignoreversion
Source: "{#APP_SOURCE}\x64\Classifier.Interop.dll"; DestDir: "{app}\x64"; Flags: ignoreversion

; x86
Source: "{#APP_SOURCE}\x86\Classifier.Engine.dll"; DestDir: "{app}\x86"; Flags: ignoreversion
Source: "{#APP_SOURCE}\x86\Classifier.Interop.dll"; DestDir: "{app}\x86"; Flags: ignoreversion

Source: "{#APP_SOURCE}Classifier.Interfaces.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}myoddweb.classifier.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}myoddweb.classifier.dll.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}myoddweb.classifier.vsto"; DestDir: "{app}"; Flags: ignoreversion

; Newtonsoft
Source: "{#APP_SOURCE}Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion

; https://msdn.microsoft.com/en-us/library/ee712616.aspx
Source: "{#APP_SOURCE}Microsoft.Office.Tools.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Microsoft.Office.Tools.Outlook.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Microsoft.Office.Tools.v4.0.Framework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Microsoft.Office.Tools.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Microsoft.VisualStudio.Tools.Applications.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Microsoft.Office.Tools.Common.v4.0.Utilities.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#APP_SOURCE}Microsoft.Office.Tools.Outlook.v4.0.Utilities.dll"; DestDir: "{app}"; Flags: ignoreversion

[Registry]

; create the event log registry...
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Services\EventLog\Application\{#EVENT_LOG_NAME}"; ValueType: string; ValueName: "EventMessageFile"; ValueData: ""

; @see https://msdn.microsoft.com/en-us/library/bb772100.aspx
; @see LoadBehavior=3 for vsto https://msdn.microsoft.com/en-us/library/bb386106.aspx

; x64
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled64Bit; Flags: uninsdeletekeyifempty
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled64Bit; ValueType: dword; ValueName: "LoadBehavior"; ValueData: "3"; Flags: uninsdeletevalue uninsdeletekeyifempty 
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled64Bit; ValueType: string; ValueName: "Manifest"; ValueData: "file:///{app}\myoddweb.classifier.vsto|vstolocal"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled64Bit; ValueType: string; ValueName: "FriendlyName"; ValueData: "{#APP_NAME}"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKCU64; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled64Bit; ValueType: string; ValueName: "Description"; ValueData: "{#APP_DESC}"; Flags: uninsdeletevalue uninsdeletekeyifempty

; x86
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled32Bit; Flags: uninsdeletekeyifempty
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled32Bit; ValueType: dword; ValueName: "LoadBehavior"; ValueData: "3"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled32Bit; ValueType: string; ValueName: "Manifest"; ValueData: "file:///{app}\myoddweb.classifier.vsto|vstolocal"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled32Bit; ValueType: string; ValueName: "FriendlyName"; ValueData: "{#APP_NAME}"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKCU32; Subkey: "Software\Microsoft\Office\Outlook\Addins\{#APP_REG_NAME}"; Check: IsOutlookVSTOInstalled32Bit; ValueType: string; ValueName: "Description"; ValueData: "{#APP_DESC}"; Flags: uninsdeletevalue uninsdeletekeyifempty

[Code]
// check if we have the vsto 64 bit installed
function IsOutlookVSTOInstalled64Bit() : boolean;
begin
  // if we are not x64 then we cannot check any x64 values.
  if not IsWin64 then
  begin
   Result := false;
   exit;
  end

  if RegKeyExists( HKLM64, 'SOFTWARE\Microsoft\VSTO Runtime Setup\v4' ) then 
  begin
    Result := true;
  end else
  if RegKeyExists( HKLM64, 'SOFTWARE\Microsoft\VSTO Runtime Setup\v4R' ) then
  begin
    Result := true;
  end else
    Result := false;
end;

// check if we have the vsto 32 bit installed
function IsOutlookVSTOInstalled32Bit() : boolean;
begin
  if RegKeyExists( HKLM32, 'SOFTWARE\Microsoft\VSTO Runtime Setup\v4' ) then 
  begin
    Result := true;
  end else
  if RegKeyExists( HKLM32, 'SOFTWARE\Microsoft\VSTO Runtime Setup\v4R' ) then
  begin
    Result := true;
  end else
    Result := false;
end;

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // or 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // or 393297 on Windows 8.1 and older
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;
    
function InitializeUninstall() : Boolean;
var
   winHwnd: longint;
begin
   winHwnd := FindWindowByClassName( 'rctrl_renwnd32' );

   if(winHwnd <> 0 ) then
   begin
       MsgBox('Outlook is running. Close it before installing.', mbInformation, MB_OK);
       Result := false;
   end else
       Result := true

end;

function InitializeSetup(): Boolean;
var
   winHwnd: longint;
begin
   // 
   if( not IsOutlookVSTOInstalled64Bit and not IsOutlookVSTOInstalled32Bit ) then 
   begin
     MsgBox('Outlook VSTO 32/64 does appear to be installed, install cannot continue.', mbInformation, MB_OK);
     Result := false;
     exit;
   end
    
   // check if outlook is running
   winHwnd := FindWindowByClassName( 'rctrl_renwnd32' );
   if(winHwnd <> 0 ) then
   begin
       MsgBox('Outlook is running. Please close it before installing.', mbInformation, MB_OK);
       Result := false;
   end
   else
   begin 
     if not IsDotNetDetected('v4.5', 0) then begin
         MsgBox('MyOdd Classifier requires Microsoft .NET Framework 4.5.x'#13#13
                'Please use Windows Update to install this version,'#13
                'and then re-run the setup program.', mbInformation, MB_OK);
                result := false;
     end else
       result := true;
   end
end;