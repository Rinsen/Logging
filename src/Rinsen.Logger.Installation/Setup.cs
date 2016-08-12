using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rinsen.DatabaseInstaller;
using System.Collections.Generic;

namespace Rinsen.Logger.Installation
{
    public static class Setup
    {
        public static void RunLoggerInstaller(this IApplicationBuilder app)
        {
            var installation = new List<DatabaseVersion>();
            installation.Add(new CreateLogTable());

            app.ApplicationServices.GetRequiredService<Installer>().Run(installation);
        }
    }
}
