### General Usage

Download from the BISM Normalizer [Visual Studio Marketplace page](https://marketplace.visualstudio.com/items?itemName=ChristianWade.BISMNormalizer3). New releases will be uploaded to this page.

This video covers BISM Normalizer 3 use cases and demo.

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/LZdOwfJqFrM/0.jpg)](http://www.youtube.com/watch?v=LZdOwfJqFrM)

### Object Model & Build Process

This video shows the internals of BISM Normalizer 3. Everything still applies except obfuscation, which has been removed.

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/r3eGK-dSYuw/0.jpg)](http://www.youtube.com/watch?v=r3eGK-dSYuw)

### Command to Perform a Release Build

Output goes to bin\ReleaseObfusc

`msbuild BismNormalizer.csproj /verbosity:m /target:Rebuild /property:Configuration=Release`

### Set Up New Development Machine

VS 2019 Workloads installed must include .NET desktop development and VS extension development.

Requires VS Installer Projects [extension](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2017InstallerProjects).

May need to temporarily comment out following at bottom of BismNormalizer.csproj to load project into VS for 1st time. After 1st successful load, add it back.

`Import Project="..\packages\MSBuild.Extension.Pack.1.8.0\build\net40\MSBuild.Extension.Pack.targets"`

Update: 4/23/2018: AMO libraries are now accessed through NuGet references, so no longer necessary to install AMO client libraries from the [MSI](https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-data-providers).

Do a Release build from the command-line to set up cross project references for the 1st time (see command above) - and then close and reopen VS.

Set AlmToolkit or BismNormalizer as startup project

In BismNormalizer project properities > Debug tab, set
* Start External Program (assuming Enterprise edition): C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe

* Command Line Arguments: /rootsuffix Exp

The automated tests refer to a localhost SSAS tabular server.
