using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Application.UseCases;
using ToolingStructureCreation.Infrastructure.NX;
using ToolingStructureCreation.View;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;
using ToolingStructureCreation.Infrastructure.FileSystem;
using ToolingStructureCreation.Infrastructure.Repositories;

namespace ToolingStructureCreation.Integration
{
    public class ToolingIntegrationController
    {
        private readonly CreateToolingStructureUseCase _createToolingUseCase;
        private readonly NXSessionManager _sessionManager;
        private readonly Controller.Control _controller;

        public ToolingIntegrationController(
            CreateToolingStructureUseCase createToolingUseCase,
            NXSessionManager sessionManager,
            Controller.Control control)
        {
            _createToolingUseCase = createToolingUseCase ?? throw new ArgumentNullException(nameof(createToolingUseCase));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _controller = control ?? throw new ArgumentNullException(nameof(control));
        }

        // <summary>
        /// Create tooling structure using existing form properties and domain naming service
        /// </summary>
        public async Task<ToolingCreationResult> CreateToolingStructureAsync(
            formToolStructure form,
            List<SketchGeometry> stationSketches,
            List<SketchGeometry> shoeSketches,
            List<SketchGeometry> commonPlateSketches = null)
        {
            try
            {
                // Create base drawing code from form using domain layer
                var baseDrawingCode = CreateBaseDrawingCode(form);

                // Use domain naming service for proper code generation
                var namingService = new NamingConventionService(baseDrawingCode, form.GetPath);
                var mainAssemblyNaming = namingService.GenerateAssemblyNaming("Main Tool Assembly");

                // Use existing form methods - same as Control.StartWithDomainLayer()
                var machineSpec = MachineSpecification.GetByName(form.GetMachineName);
                var toolingParameters = ToolingParameters.FromForm(
                    form,
                    machineSpec,
                    mainAssemblyNaming.DrawingCode,  // Use generated drawing code
                    form.GetModel,
                    form.GetDesginer);

                // Create request using existing form structure
                var request = new CreateToolingStructureRequest
                {
                    ToolingParameters = toolingParameters,
                    StationSketches = stationSketches ?? new List<SketchGeometry>(),
                    ShoeSketches = shoeSketches ?? new List<SketchGeometry>(),
                    CommonPlateSketches = commonPlateSketches ?? new List<SketchGeometry>(),
                    OutputDirectory = form.GetPath  // Use existing GetPath property
                };

                // Execute use case
                var result = await _createToolingUseCase.ExecuteAsync(request);

                return new ToolingCreationResult
                {
                    Success = result.Success,
                    Message = result.Message,
                    ProjectPath = result.ProjectPath,
                    ComponentCount = result.TotalComponentsCreated,
                    Exception = result.Exception
                };

            }
            catch (Exception ex)
            {
                return new ToolingCreationResult
                {
                    Success = false,
                    Message = $"Integration error: {ex.Message}",
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// Initialize integration - call from Control.cs
        /// </summary>
        public void Initialize()
        {
            _sessionManager.Initialize();
            _sessionManager.ValidateSession();
        }

        private MachineSpecification CreateMachineSpecification(formToolStructure form)
        {
            var machineName = form.GetMachineName ?? "MC304";
            return MachineSpecification.GetByName(machineName);
        }

        private DrawingCode CreateDrawingCode(formToolStructure form)
        {
            var codePrefix = form.GetCodePrefix ?? "3DA";
            return new DrawingCode(codePrefix);
        }

        /// <summary>
        /// Factory method to create configured controller
        /// </summary>
        public static ToolingIntegrationController Create(string templateBasePath, string dataDirectory)
        {
            // Setup infrastructure
            var sessionManager = new NXSessionManager();
            var fileSystemService = new FileSystemService();
            var modelFactory = new NXModelFactory(sessionManager, templateBasePath);
            var repository = new ToolingStructureRepository(fileSystemService, dataDirectory);

            // Setup application
            var useCase = new CreateToolingStructureUseCase(
                repository,
                sessionManager,
                modelFactory,
                fileSystemService);

            return new ToolingIntegrationController(useCase, sessionManager);
        }
    }
}
