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

        public ToolingIntegrationController(
            CreateToolingStructureUseCase createToolingUseCase,
            NXSessionManager sessionManager)
        {
            _createToolingUseCase = createToolingUseCase ?? throw new ArgumentNullException(nameof(createToolingUseCase));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
        }

        // <summary>
        /// Create tooling structure from Form1 data
        /// </summary>
        public async Task<ToolingCreationResult> CreateToolingStructureAsync(
            formToolStructure form,
            List<SketchGeometry> stationSketches,
            List<SketchGeometry> shoeSketches,
            List<SketchGeometry> commonPlateSketches = null)
        {
            try
            {
                // Convert form data to domain objects
                var machineSpec = CreateMachineSpecification(form);
                var drawingCode = CreateDrawingCode(form);
                var toolingParameters = ToolingParameters.FromForm(
                    form,
                    machineSpec,
                    drawingCode,
                    form.GetModel ?? "Default_Project",
                    form.GetDesginer ?? "unknown_Designer"
                    );

                // Create request
                var request = new CreateToolingStructureRequest
                {
                    ToolingParameters = toolingParameters,
                    StationSketches = stationSketches ?? new List<SketchGeometry>(),
                    ShoeSketches = shoeSketches ?? new List<SketchGeometry>(),
                    CommonPlateSketches = commonPlateSketches ?? new List<SketchGeometry>(),
                    OutputDirectory = form.GetPath ?? @"C:\CreateFolder"
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
