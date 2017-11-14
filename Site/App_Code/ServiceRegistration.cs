using System;
using System.Web.UI;
using Autofac;
using PX.Data;
using PX.Data.RelativeDates;
using PX.Data.Wiki.Parser;

public class ServiceRegistration: Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FinancialPeriodManagerStub>().As<IFinancialPeriodManager>();
        builder.RegisterType<TodayUTC>().As<ITodayUtc>();
		builder.RegisterInstance<Func<Page, ISettings>>(PXWikiSettingsRelative.GetSettings).Named<Func<Page, ISettings>>("Relative");
    }
}