using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Jagi.Utility;
using Microsoft.Practices.ServiceLocation;
using JagiWebSample.Areas.Admin.Models;
using Jagi.Mvc.Angular;
using JagiWebSample.Models;
using Jagi.Database.Mvc;
using Jagi.Interface;
using JagiWebSample.Utility;
using JagiWebSample.Controllers;

namespace JagiWebSample.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container.RegisterType<AdminDataContext>(new PerRequestLifetimeManager());
            string connection = @"Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\aspnet-JagiWebSample-20150929115454.mdf;Initial Catalog=aspnet-JagiWebSample-20150929115454;Integrated Security=True";
            container.RegisterType<DataContext>(new PerRequestLifetimeManager(), 
                new InjectionConstructor(connection, new CurrentRequest()));

            // Setup AccountController 使用無參數版本，讓 Owin 自行協助注入
            container.RegisterType<AccountController>(new InjectionConstructor());

            // Setup Email Provider
            EmailSetting setting = new EmailSetting { Email = "redmine.excelsior@gmail.com", Password = "490910490910" };
            container.RegisterInstance(typeof(EmailSetting), setting);

            // Setup Angular Html Tag Provider
            container.RegisterInstance(typeof(AngularHtmlTag), new EntityHtmlTag());

            ExecuteStartupTasks(container);

            // Setup Crypto Service Provider
            var cryptoSetting = new CryptoSetting("Ers@Hope", "jagi@Excelsi0r");
            container.RegisterInstance(cryptoSetting);

            // Setup TableValue
            container.RegisterType<TableValue>(new PerRequestLifetimeManager());

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static void ExecuteStartupTasks(IUnityContainer container)
        {
            // registering all type of IRunAtStartup:
            container.RegisterTypes(AllClasses.FromLoadedAssemblies()
                .Where(type => typeof(IRunAtStartup).IsAssignableFrom(type)),
                WithMappings.FromAllInterfaces, WithName.TypeName, WithLifetime.Transient);

            foreach (var task in container.ResolveAll<IRunAtStartup>())
                task.Execute();
        }
    }
}
