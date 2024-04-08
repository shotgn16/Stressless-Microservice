; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "StresslessService"
#define MyAppVersion "0.1"
#define MyAppPublisher "Butcher-Barnard, Inc."
#define MyAppExeName "MyProg.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7CC82925-04AA-4A93-9EBB-23438C9395EC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\Service
DisableDirPage=yes
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
InfoBeforeFile=C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\resources\licence.txt
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputBaseFilename=StresslessService
Password=Password.123
Encryption=yes
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "armenian"; MessagesFile: "compiler:Languages\Armenian.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "bulgarian"; MessagesFile: "compiler:Languages\Bulgarian.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "icelandic"; MessagesFile: "compiler:Languages\Icelandic.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovak"; MessagesFile: "compiler:Languages\Slovak.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Program Files (x86)\Inno Setup 6\Examples\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\cs\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\de\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\es\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\fr\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\it\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ja\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ko\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\pl\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\pt-BR\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\resources\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ru\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\runtimes\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\tr\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\zh-Hans\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\zh-Hant\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\appsettings.Development.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\appsettings.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\CommunityToolkit.WinUI.Notifications.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\EntityFramework.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Humanizer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\libman.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.AspNet.WebHooks.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.AspNet.WebHooks.Custom.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.AspNet.WebHooks.Custom.Mvc.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.AspNetCore.Authentication.JwtBearer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.AspNetCore.OpenApi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.Bcl.AsyncInterfaces.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.CodeAnalysis.CSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.CodeAnalysis.CSharp.Workspaces.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.CodeAnalysis.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.CodeAnalysis.Workspaces.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.Data.Sqlite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.EntityFrameworkCore.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.EntityFrameworkCore.Design.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.EntityFrameworkCore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.EntityFrameworkCore.Relational.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.EntityFrameworkCore.Sqlite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.Extensions.DependencyModel.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.IdentityModel.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.IdentityModel.JsonWebTokens.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.IdentityModel.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.IdentityModel.Protocols.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.IdentityModel.Protocols.OpenIdConnect.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.IdentityModel.Tokens.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.OpenApi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.Toolkit.Uwp.Notifications.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.Web.Infrastructure.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Microsoft.Win32.SystemEvents.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\MiniProfiler.AspNetCore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\MiniProfiler.Integrations.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\MiniProfiler.Shared.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Mono.TextTemplating.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Newtonsoft.Json.Bson.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\nlog.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\NLog.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\NLog.Extensions.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\NLog.Targets.Seq.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\NLog.Web.AspNetCore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\NLog.Web.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ServiceStack.Client.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ServiceStack.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ServiceStack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ServiceStack.Interfaces.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\ServiceStack.Text.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\SQLitePCLRaw.batteries_v2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\SQLitePCLRaw.core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\SQLitePCLRaw.provider.e_sqlite3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless.pfx"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless-Service.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless-Service.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless-Service.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless-Service.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless-Service.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Stressless-Service.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\swagger.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Swashbuckle.AspNetCore.Swagger.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Swashbuckle.AspNetCore.SwaggerGen.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\Swashbuckle.AspNetCore.SwaggerUI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.CodeDom.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Composition.AttributedModel.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Composition.Convention.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Composition.Hosting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Composition.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Composition.TypedParts.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Configuration.ConfigurationManager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Data.SqlClient.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Data.SQLite.EF6.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Drawing.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.IdentityModel.Tokens.Jwt.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Net.Http.Formatting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Security.Cryptography.ProtectedData.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Security.Permissions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.Helpers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.Http.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.Mvc.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.Razor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.WebPages.Deployment.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.WebPages.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Web.WebPages.Razor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Jack Butcher-Barnard\source\repos\Github\shotgn16\Stressless-Microservice\bin\Release\net8.0\System.Windows.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\Stressless-Service.exe"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
