
; Include the common baseline stuff. QT4 will also bring in the MSVC dlls, too.
; NOTE: because of the way Inno processes the macros, it's not possible to make
; a macro reference a macro. In certain places we instead have to use GetEnv instead
; of the define'd value.

#define BVTBINFOLDER		GetEnv('TEMP')+"\BVTBuild"
#define BERTECINSTALLCOMMON	GetEnv('BERTECINSTALLCOMMON')
#define BERTECINSTALLOUT	GetEnv('BERTECINSTALLOUT')

#pragma include __INCLUDE__ + ";" + GetEnv('BERTECINSTALLCOMMON')

#include <Install_RequireMinWin7Sp1.iss>
#include <InstallDrivers.iss>

#define MyAppName 		"Bertec Vision Trainer"
#define MyAppNameShort	"BVT"
#define MyInstallFolder "Bertec\Bertec Vision Trainer"
#define MyInstallGroup  "Bertec"
#define MyAppVer     	GetFileVersion(GetEnv('TEMP')+"\BVTBuild\BVT.exe")
#define MyAppPublisher 	"Bertec Corporation"
#define MyAppCopyright 	"Copyright (c) 2019-2020 Bertec Corporation"
#define MyAppURL 		"http://www.bertec.com/"
#define MyAppExeName 	"BVT.exe"


[ThirdPartySettings]
CompileLogMethod=append

[PostCompile]
Name: {#BERTECINSTALLCOMMON}\SignInstallSetup.bat; Parameters: BertecVisionTrainerSetup.exe; Flags: runminimized abortonerror

[ThirdParty]
CompileLogMethod=append

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{E6CD8AD8-30F9-4040-9C33-2F56D400CC58}
AppName={#MyAppName}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppMutex={#MyAppName}ApplicationRunningMutex
DefaultDirName={pf}\{#MyInstallFolder}
DefaultGroupName={#MyInstallGroup}
DisableDirPage=no
OutputDir={#BERTECINSTALLOUT}
OutputBaseFilename=BertecVisionTrainerSetup
Compression=lzma2/ultra64
SolidCompression=true
InternalCompressLevel=ultra64
PrivilegesRequired=poweruser
VersionInfoCompany={#MyAppPublisher}
VersionInfoVersion={#MyAppVer}
VersionInfoDescription={#MyAppName}
VersionInfoTextVersion={#MyAppVer}
VersionInfoCopyright={#MyAppCopyright}
OutputManifestFile=BertecVisionTrainerSetup-Has-These-Files.txt
AlwaysShowDirOnReadyPage=true
AlwaysShowGroupOnReadyPage=true
AppVersion={#MyAppVer}
UninstallDisplayName={#MyAppName}
SignedUninstaller=true
RestartIfNeededByRun=false
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
SetupMutex=InstallingBertecVisionTrainer.0X64
LZMAUseSeparateProcess=yes
ShowLanguageDialog=auto
LZMANumBlockThreads=4
; This only works if the command line caller passes "/Sstandardsigning=signtool sign /a /fd sha256 /tr http://sha256timestamp.ws.symantec.com/sha256/timestamp /td sha256 $f"
; This is only being done to avoid the continual problem with the uninstaller needed to be signed (seems to be tied into the version and copyright strings)
SignTool=standardsigning

[Registry]
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{#MyAppExeName}; ValueType: string; ValueData: {app}\{#MyAppExeName}; Flags: uninsdeletekey
Root: HKLM; Subkey: Software\Bertec\BVT; ValueType: none; Permissions: everyone-full

[Languages]
Name: en; MessagesFile: compiler:Default.isl
Name: fr; MessagesFile: compiler:Languages\French.isl
Name: it; MessagesFile: compiler:Languages\Italian.isl
Name: es; MessagesFile: compiler:Languages\Spanish.isl
Name: gr; MessagesFile: compiler:Languages\German.isl

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}
Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Files]
Source: {#BVTBINFOLDER}\*; DestDir: {app}; Excludes: output_log.txt; Flags: ignoreversion recursesubdirs createallsubdirs
Source: Manual\BER_BVT_UserManual_v10.pdf; DestDir: {app}; DestName: BVT User Manual.pdf; Flags: ignoreversion

[Dirs]
; This will create the C:\ProgramData\Bertec\BVT if it's missing, and set the permissions so that all users can change files in those folders.
; If the folder already exists it will modify the permissions for it and all files within so that an existing database created by User A can be written by Users B.
Name: {commonappdata}\Bertec; Flags: uninsneveruninstall; Permissions: everyone-full
Name: {commonappdata}\Bertec\BVT; Flags: uninsneveruninstall; Permissions: everyone-full

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Run]

[UninstallRun]


[InstallDelete]

[_ISTool]
UseAbsolutePaths=true

[Icons]
Name: {group}\{#MyAppNameShort}; Filename: {app}\{#MyAppExeName}
Name: {group}\BVT User Manual; Filename: {app}\BVT User Manual.pdf
Name: {group}\{cm:UninstallProgram,{#MyAppNameShort}}; Filename: {uninstallexe}
Name: {commondesktop}\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: desktopicon
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: quicklaunchicon

[Types]
Name: standardinstall; Description: Standard Installation
Name: custom; Description: Custom Installation; Flags: iscustom
;Name: balancecheck; Description: BalanceCheck
;Name: balancecheckscreener; Description: BalanceCheck - Screener
;Name: balancechecktrainer; Description: BalanceCheck - Trainer

[Components]
;Name: exampledb; Description: Example Database; Flags: dontinheritcheck
;Name: stabilitytests; Description: Stability Tests; Types: standardinstall balancechecktrainer balancecheckscreener balancecheck
;Name: limittests; Description: Limit Tests; Types: standardinstall balancecheck
;Name: stabilitytrainings; Description: Stability Trainings; Types: standardinstall balancechecktrainer

[_ISToolPostCompile]
Name: {#BERTECINSTALLCOMMON}\SignInstallSetup.bat; Parameters: BertecVisionTrainerSetup.exe

[UninstallDelete]
Type: files; Name: {app}\BVT_Data\output_log.txt
Type: filesandordirs; Name: {app}

[Code]

/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;

/////////////////////////////////////////////////////////////////////
// Allows for standard command line parsing assuming a key/value organization
function GetCommandlineParam (inParam: String):String;
var
  LoopVar : Integer;
  BreakLoop : Boolean;
begin
  // Init the variable to known values
  LoopVar :=0;
  Result := '';
  BreakLoop := False;

  // Loop through the passed in arry to find the parameter
  while ( (LoopVar < ParamCount) and
          (not BreakLoop) ) do
  begin
    // Determine if the looked for parameter is the next value
    if ( (ParamStr(LoopVar) = inParam) and
         ( (LoopVar+1) <= ParamCount )) then
    begin
      // Set the return result equal to the next command line parameter
      Result := ParamStr(LoopVar+1);

      // Break the loop
      BreakLoop := True;
    end;

    // Increment the loop variable
    LoopVar := LoopVar + 1;
  end;
end;
