using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ExpenseCategoryApp.EntityFrameworkCore;

namespace ExpenseCategoryApp.Web.Host
{
    [DependsOn(typeof(ExpenseCategoryAppApplicationModule), typeof(ExpenseCategoryAppEntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class ExpenseCategoryAppWebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(ExpenseCategoryAppApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ExpenseCategoryAppWebHostModule).GetAssembly());
        }
    }
}
