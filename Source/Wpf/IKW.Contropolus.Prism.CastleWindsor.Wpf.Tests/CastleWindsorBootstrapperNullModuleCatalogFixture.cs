using System;
using System.Windows;
using IKW.Contropolus.Prism.CastleWindsor.WPF.Legacy;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.Modularity;
using Xunit;

namespace IKW.Contropolus.Prism.CastleWindsor.Wpf.Tests
{
    
    public class CastleWindsorBootstrapperNullModuleCatalogFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void NullModuleCatalogThrowsOnDefaultModuleInitialization()
        {
            var bootstrapper = new NullModuleCatalogBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IModuleCatalog");
        }

        private class NullModuleCatalogBootstrapper : CastleWindsorBootstrapper
        {
            protected override IModuleCatalog CreateModuleCatalog()
            {
                return null;
            }

            protected override DependencyObject CreateShell()
            {
                throw new NotImplementedException();
            }

            protected override void InitializeShell()
            {
                throw new NotImplementedException();
            }
        }
        
    }
}