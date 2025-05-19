using System;
using ToolingStructureCreation.Controller;
using ToolingStructureCreation.Interfaces;
using ToolingStructureCreation.Services;
using Unity;
using Unity.Lifetime;

namespace ToolingStructureCreation
{
    public partial class MyProgram
    {
        private static IUnityContainer ConfigureContainer()
        {
            var container = new UnityContainer();

            // Register services with singleton lifetime
            container.RegisterType<INXSessionProvider, NXService>(new ContainerControlledLifetimeManager());

            // Register IUIService with the same instance as INXSessionProvider
            container.RegisterFactory<IUIService>(c => c.Resolve<INXSessionProvider>() as IUIService);

            // Register other services
            container.RegisterType<INXSelectionService, SelectionService>(new ContainerControlledLifetimeManager());

            // Register controller
            container.RegisterType<IController, Control>();

            return container;
        }
        public static void Main(string[] args)
        {
            try
            {
                // Configure container
                var container = ConfigureContainer();

                // Resolve controller and initialize
                var controller = container.Resolve<IController>();
                controller.Initialize();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Application error: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
