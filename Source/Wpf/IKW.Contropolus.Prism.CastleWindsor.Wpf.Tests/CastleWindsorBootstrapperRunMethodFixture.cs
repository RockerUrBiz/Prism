using System;
using System.Windows;
using System.Windows.Controls;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator;
using IKW.Contropolus.Prism.CastleWindsor.WPF.Legacy;
using IKW.Contropolus.Prism.CastleWindsor.Wpf.ServiceLocator;
using Moq;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Xunit;

namespace IKW.Contropolus.Prism.CastleWindsor.Wpf.Tests
{
    using global::Prism.Ioc;
    using Regions;
    using WPF.Ioc;

    [Collection("ServiceLocator")]
    public class CastleWindsorBootstrapperRunMethodFixture
    {
        [StaFact]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
        }

        [StaFact]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [StaFact]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.True(CommonServiceLocator.ServiceLocator.Current is CastleWindsorServiceLocatorAdapter);
        }

        [StaFact]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.NotNull(container);
            Assert.IsType<WindsorContainer>(container);
        }

        [StaFact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IWindsorContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Equal(typeof(WindsorContainer), returnedContainer.GetType());
        }

        [StaFact]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [StaFact]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [StaFact]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateLoggerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateShellCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureContainerCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureServiceLocator()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureServiceLocatorCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureViewModelLocator()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureViewModelLocatorCalled);
        }

        [StaFact]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);


            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            //Container.Register(Component.For(typeof(ILoggerFacade))
            //    .Instance(Logger)
            //    .LifeStyle.Singleton);

            mockedContainer.Verify(container => container.Register(Component.For(typeof(ILoggerFacade))
                    .Instance(bootstrapper.BaseLogger)
                    .LifeStyle.Singleton),
                Times.Once());

            //mockedContainer.Verify(c => c.RegisterInstance(typeof(ILoggerFacade), null, bootstrapper.BaseLogger, It.IsAny<IInstanceLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IModuleCatalog))
                    .Instance(It.IsAny<object>())
                    .LifeStyle.Singleton),
                Times.Once());

            //mockedContainer.Verify(c => c.RegisterInstance(typeof(IModuleCatalog), null, It.IsAny<object>(), It.IsAny<IInstanceLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIServiceLocator()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IServiceLocator))
                .Instance(typeof(CastleWindsorServiceLocatorAdapter))
                .LifeStyle.Singleton), Times.Once());

            //mockedContainer.Verify(c => c.RegisterType(typeof(IServiceLocator), typeof(CastleWindsorServiceLocatorAdapter), null, It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IModuleInitializer))
                    .Instance(typeof(object))
                    .LifeStyle.Singleton),Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(
                container => container.Register(Component.For(typeof(IRegionManager))
                    .Instance(It.IsAny<Type>())
                    .LifeStyle.Transient),
                Times.Once());

            //mockedContainer.Verify(c => c.RegisterType(typeof(IRegionManager), It.IsAny<Type>(), null, It.IsAny<LifeStyle>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(
                container => container.Register(Component.For(typeof(RegionAdapterMappings))
                    .Instance(typeof(RegionAdapterMappings))
                    .LifeStyle.Singleton),
                Times.Once());

            //mockedContainer.Verify(container => container.Register(Component.For(typeof(RegionAdapterMappings)).Instance(typeof(RegionAdapterMappings)).LifeStyle.Singleton), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IRegionViewRegistry))
                    .Instance(It.IsAny<Type>())
                    .LifeStyle.Transient),
                Times.Once());

            //mockedContainer.Verify(c => c.RegisterType(typeof(IRegionViewRegistry), It.IsAny<Type>(), null, It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IRegionBehaviorFactory))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Once());

            //mockedContainer.Verify(c => c.RegisterType(typeof(IRegionBehaviorFactory), It.IsAny<Type>(), null, It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IEventAggregator))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Once());

            //mockedContainer.Verify(c => c.RegisterType(typeof(IEventAggregator), It.IsAny<Type>(), null, It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunFalseShouldNotRegisterDefaultServicesAndTypes()
        {
            var mockedContainer = new Mock<IWindsorContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);
            bootstrapper.Run(false);

            mockedContainer.Verify(container => container.Register(Component.For(typeof(IEventAggregator))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Never());
            mockedContainer.Verify(container => container.Register(Component.For(typeof(IRegionManager))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Never());
            mockedContainer.Verify(container => container.Register(Component.For(typeof(RegionAdapterMappings))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Never());
            mockedContainer.Verify(container => container.Register(Component.For(typeof(IServiceLocator))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Never());
            mockedContainer.Verify(container => container.Register(Component.For(typeof(IModuleInitializer))
                    .Instance(typeof(object))
                    .LifeStyle.Transient),
                Times.Never());
        }

        [StaFact]
        public void ModuleManagerRunCalled()
        {
            // Have to use a non-mocked container because of IsRegistered<> extension method, Registrations property,and ContainerRegistration
            var container = new WindsorContainer();

            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();
            var serviceLocatorAdapter = new CastleWindsorServiceLocatorAdapter(container);
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocatorAdapter);

            container.Register(Component.For(typeof(IServiceLocator)).Instance(serviceLocatorAdapter).LifeStyle.Transient);
            container.Register(Component.For(typeof(IModuleCatalog)).Instance(mockedModuleInitializer.Object).LifeStyle.Transient);
            container.Register(Component.For(typeof(IModuleInitializer)).Instance(new ModuleCatalog()).LifeStyle.Transient);
            container.Register(Component.For(typeof(IModuleManager)).Instance(mockedModuleManager.Object).LifeStyle.Transient);
            container.Register(Component.For(typeof(RegionAdapterMappings)).Instance(regionAdapterMappings).LifeStyle.Transient);


            //container.RegisterInstance<IServiceLocator>(serviceLocatorAdapter);
            //container.RegisterInstance<IModuleCatalog>(new ModuleCatalog());
            //container.RegisterInstance<IModuleInitializer>(mockedModuleInitializer.Object);
            //container.RegisterInstance<IModuleManager>(mockedModuleManager.Object);
            //container.RegisterInstance<RegionAdapterMappings>(regionAdapterMappings);

            container.RegisterType<RegionAdapterMappings, RegionAdapterMappings>(true);
            container.RegisterType<IRegionManager, RegionManager>(true);
            container.RegisterType<IRegionViewRegistry, EventAggregator>(true);
            container.RegisterType<IRegionViewRegistry, RegionViewRegistry>(true);
            container.RegisterType<IRegionBehaviorFactory, RegionBehaviorFactory>(true);
            container.RegisterType<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(true);
            container.RegisterType<IRegionNavigationJournal, RegionNavigationJournal>(true);
            container.RegisterType<IRegionNavigationService, RegionNavigationService>(true);
            container.RegisterType<IRegionNavigationContentLoader, CastleWindsorRegionNavigationContentLoader>(true);

            //container.RegisterSingleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings));
            //container.RegisterSingleton(typeof(IRegionManager), typeof(RegionManager));
            //container.RegisterSingleton(typeof(IEventAggregator), typeof(EventAggregator));
            //container.RegisterSingleton(typeof(IRegionViewRegistry), typeof(RegionViewRegistry));
            //container.RegisterSingleton(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory));
            //container.RegisterSingleton(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry));
            //container.RegisterSingleton(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal));
            //container.RegisterSingleton(typeof(IRegionNavigationService), typeof(RegionNavigationService));
            //container.RegisterSingleton(typeof(IRegionNavigationContentLoader), typeof(global::Prism.Regions.CastleWindsorRegionNavigationContentLoader));

            container.RegisterInstance<SelectorRegionAdapter, SelectorRegionAdapter>(new SelectorRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ItemsControlRegionAdapter, ItemsControlRegionAdapter>(new ItemsControlRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ContentControlRegionAdapter, ContentControlRegionAdapter>(new ContentControlRegionAdapter(regionBehaviorFactory));

            //container.RegisterInstance<SelectorRegionAdapter>(new SelectorRegionAdapter(regionBehaviorFactory));
            //container.RegisterInstance<ItemsControlRegionAdapter>(new ItemsControlRegionAdapter(regionBehaviorFactory));
            //container.RegisterInstance<ContentControlRegionAdapter>(new ContentControlRegionAdapter(regionBehaviorFactory));

            var bootstrapper = new MockedContainerBootstrapper(container);
            bootstrapper.Run(false);

            mockedModuleManager.Verify(mm => mm.Run(), Times.Once());
        }

        [StaFact]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.Equal("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.Equal("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.Equal("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.Equal("CreateContainer", bootstrapper.MethodCalls[3]);
            Assert.Equal("ConfigureContainer", bootstrapper.MethodCalls[4]);
            Assert.Equal("ConfigureServiceLocator", bootstrapper.MethodCalls[5]);
            Assert.Equal("ConfigureViewModelLocator", bootstrapper.MethodCalls[6]);
            Assert.Equal("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[7]);
            Assert.Equal("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[8]);
            Assert.Equal("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[9]);
            Assert.Equal("CreateShell", bootstrapper.MethodCalls[10]);
            Assert.Equal("InitializeShell", bootstrapper.MethodCalls[11]);
            Assert.Equal("InitializeModules", bootstrapper.MethodCalls[12]);
        }

        [StaFact]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.Contains("Logger was created successfully.", messages[0]);
            Assert.Contains("Creating module catalog.", messages[1]);
            Assert.Contains("Configuring module catalog.", messages[2]);
            Assert.Contains("Creating Unity container.", messages[3]);
            Assert.Contains("Configuring the Unity container.", messages[4]);
            Assert.Contains("Adding UnityBootstrapperExtension to container.", messages[5]);
            Assert.Contains("Configuring ServiceLocator singleton.", messages[6]);
            Assert.Contains("Configuring the ViewModelLocator to use Unity.", messages[7]);
            Assert.Contains("Configuring region adapters.", messages[8]);
            Assert.Contains("Configuring default region behaviors.", messages[9]);
            Assert.Contains("Registering Framework Exception Types.", messages[10]);
            Assert.Contains("Creating the shell.", messages[11]);
            Assert.Contains("Setting the RegionManager.", messages[12]);
            Assert.Contains("Updating Regions.", messages[13]);
            Assert.Contains("Initializing the shell.", messages[14]);
            Assert.Contains("Initializing modules.", messages[15]);
            Assert.Contains("Bootstrapper sequence completed.", messages[16]);
        }

        [StaFact]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
        [StaFact]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Unity container.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringContainer()
        {
            const string expectedMessageText = "Configuring the Unity container.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringViewModelLocator()
        {
            const string expectedMessageText = "Configuring the ViewModelLocator to use Unity.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultUnityBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.False(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        private static void SetupMockedContainerForVerificationTests(Mock<IWindsorContainer> mockedContainer)
        {
            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager     = new Mock<IModuleManager>();
            var regionAdapterMappings   = new RegionAdapterMappings();
            var serviceLocatorAdapter   = new CastleWindsorServiceLocatorAdapter(mockedContainer.Object);
            var regionBehaviorFactory   = new RegionBehaviorFactory(serviceLocatorAdapter);

            mockedContainer.Setup(c => c.Resolve(typeof(IServiceLocator), (string)null)).Returns(serviceLocatorAdapter);

            //mockedContainer.Setup(c => c.RegisterInstance(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IInstanceLifetimeManager>()));

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleCatalog), (string)null)).Returns(
                new ModuleCatalog());

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleInitializer), (string)null)).Returns(
                mockedModuleInitializer.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleManager), (string)null)).Returns(
                mockedModuleManager.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(RegionAdapterMappings), (string)null)).Returns(
                regionAdapterMappings);

            mockedContainer.Setup(c => c.Resolve(typeof(SelectorRegionAdapter), (string)null)).Returns(
                new SelectorRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ItemsControlRegionAdapter), (string)null)).Returns(
                new ItemsControlRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ContentControlRegionAdapter), (string)null)).Returns(
                new ContentControlRegionAdapter(regionBehaviorFactory));
        }

        private class MockedContainerBootstrapper : CastleWindsorBootstrapper
        {
            private readonly IWindsorContainer container;
            public ILoggerFacade BaseLogger
            { get { return base.Logger; } }

            public void CallConfigureContainer()
            {
                base.ConfigureContainer();
            }

            public MockedContainerBootstrapper(IWindsorContainer container)
            {
                this.container = container;
            }

            protected override IWindsorContainer CreateContainer()
            {
                return container;
            }

            protected override IContainerExtension CreateContainerExtension()
            {
                return new CastleWindsorContainerExtension();
            }

            protected override DependencyObject CreateShell()
            {
                return new UserControl();
            }

            protected override void InitializeShell()
            {
                // no op
            }
        }

    }
}
