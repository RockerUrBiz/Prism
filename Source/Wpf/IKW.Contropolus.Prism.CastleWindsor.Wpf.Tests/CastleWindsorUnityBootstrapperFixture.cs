using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using CommonServiceLocator;
using IKW.Contropolus.Prism.CastleWindsor.WPF.ExceptionResolution;
using IKW.Contropolus.Prism.CastleWindsor.WPF.Legacy;
using Prism.Ioc;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Xunit;

namespace IKW.Contropolus.Prism.CastleWindsor.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class CastleWindsorUnityBootstrapperFixture: BootstrapperFixtureBase
    {
        [StaFact]
        public void ContainerDefaultsToNull()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);
        }

        [StaFact]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultUnityBootstrapper();
        }

        [StaFact]
        public void CreateContainerShouldInitializeContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            IWindsorContainer container = bootstrapper.CallCreateContainer();

            Assert.NotNull(container);
            Assert.IsAssignableFrom<IWindsorContainer>(container);
        }

        [StaFact]
        public void ConfigureContainerAddsModuleCatalogToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.Resolve<IModuleCatalog>();
            Assert.NotNull(returnedCatalog);
            Assert.True(returnedCatalog is ModuleCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsLoggerFacadeToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.Resolve<ILoggerFacade>();
            Assert.NotNull(returnedCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalEntryToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournalEntry>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournal>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournal>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationServiceToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationService>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationService>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsNavigationTargetHandlerToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationContentLoader>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.Same(actual1, actual2);
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ActivationException)));
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterResolutionFailedException()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ResolutionFailedException)));
        }
    }

    internal class DefaultUnityBootstrapper : CastleWindsorBootstrapper
    {
        public List<string> MethodCalls = new List<string>();
        public bool InitializeModulesCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public bool CreateLoggerCalled;
        public bool CreateModuleCatalogCalled;
        public bool ConfigureContainerCalled;
        public bool CreateShellCalled;
        public bool CreateContainerCalled;
        public bool ConfigureModuleCatalogCalled;
        public bool InitializeShellCalled;
        public bool ConfigureServiceLocatorCalled;
        public bool ConfigureViewModelLocatorCalled;
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public DependencyObject ShellObject = new UserControl();

        public DependencyObject BaseShell
        {
            get { return base.Shell; }
        }
        public IWindsorContainer BaseContainer
        {
            get { return base.Container; }
            set { base.Container = value; }
        }

        public MockLoggerAdapter BaseLogger
        { get { return base.Logger as MockLoggerAdapter; } }

        public IWindsorContainer CallCreateContainer()
        {
            return this.CreateContainer();
        }

        protected override IWindsorContainer CreateContainer()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateContainerCalled = true;
            return base.CreateContainer();
        }

        protected override void ConfigureContainer()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureContainerCalled = true;
            base.ConfigureContainer();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            throw new NotImplementedException();
        }

        protected override ILoggerFacade CreateLogger()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateLoggerCalled = true;
            return new MockLoggerAdapter();
        }

        protected override DependencyObject CreateShell()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateShellCalled = true;
            return ShellObject;
        }

        protected override void ConfigureServiceLocator()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureServiceLocatorCalled = true;
            base.ConfigureServiceLocator();
        }

        protected override void ConfigureViewModelLocator()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureViewModelLocatorCalled = true;
            base.ConfigureViewModelLocator();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateModuleCatalogCalled = true;
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureModuleCatalogCalled = true;
            base.ConfigureModuleCatalog();
        }

        protected override void InitializeShell()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.InitializeShellCalled = true;
            // no op
        }

        protected override void InitializeModules()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureDefaultRegionBehaviorsCalled = true;
            return base.ConfigureDefaultRegionBehaviors();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureRegionAdapterMappingsCalled = true;
            var regionAdapterMappings = base.ConfigureRegionAdapterMappings();

            DefaultRegionAdapterMappings = regionAdapterMappings;

            return regionAdapterMappings;
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            base.RegisterFrameworkExceptionTypes();
        }

        public void CallRegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
        }
    }
    
}
