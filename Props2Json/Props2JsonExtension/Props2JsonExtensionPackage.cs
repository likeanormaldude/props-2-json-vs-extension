global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;
using Props2JsonExtension.ExtensionOptions;

namespace Props2JsonExtension;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.Props2JsonExtensionString)]
[ProvideOptionPage(typeof(Props2JsonOptions), "Props2Json Extension", "General", 0, 0, true)]
public sealed class Props2JsonExtensionPackage : ToolkitPackage
{
    protected override async Task InitializeAsync(
        CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress
    )
    {
        await this.RegisterCommandsAsync();
    }
}
