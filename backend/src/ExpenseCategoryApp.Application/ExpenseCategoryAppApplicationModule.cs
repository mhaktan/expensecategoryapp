using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ExpenseCategoryApp
{
    [DependsOn(typeof(ExpenseCategoryAppCoreModule), typeof(AbpAutoMapperModule))]
    public class ExpenseCategoryAppApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(ExpenseCategoryAppApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ExpenseCategoryAppApplicationModule).GetAssembly());
        }
    }
}
