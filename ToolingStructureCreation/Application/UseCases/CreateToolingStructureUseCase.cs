using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.Repositories;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Infrastructure.FileSystem;
using ToolingStructureCreation.Infrastructure.NX;

namespace ToolingStructureCreation.Application.UseCases
{
    /// <summary>
    /// Use case for creating complete tooling structure
    /// </summary>
    public class CreateToolingStructureUseCase
    {
        private readonly IToolingStructureRepository _repository;
        private readonly NXSessionManager _nxSessionManager;
        private readonly NXModelFactory _nxModelFactory;
        private readonly FileSystemService _fileSystemService;

        public CreateToolingStructureUseCase(
            IToolingStructureRepository repository,
            NXSessionManager nxSessionManager,
            NXModelFactory nxModelFactory,
            FileSystemService fileSystemService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _nxSessionManager = nxSessionManager ?? throw new ArgumentNullException(nameof(nxSessionManager));
            _nxModelFactory = nxModelFactory ?? throw new ArgumentNullException(nameof(nxModelFactory));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        }

        /// <summary>
        /// Execute tooling structure creation
        /// </summary>
        public async Task<CreateToolingStructureResult> ExecuteAsync(CreateToolingStructureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new CreateToolingStructureResult();

            try
            {
                // 1. Initialize NX session
                _nxSessionManager.Initialize();
                _nxSessionManager.ValidateSession();

                // 2. Create project directory
                var projectPath = _fileSystemService.CreateProjectDirectory(
                    request.OutputDirectory,
                    request.ToolingParameters.ProjectName);

                // 3. Create domain aggregate
                var toolingStructure = new ToolingStructureAggregate(request.ToolingParameters);
                toolingStructure.GenerateToolingStructure(
                    request.StationSketches,
                    request.ShoeSketches,
                    request.CommonPlateSketches);

                // 4. Create NX components (all in same directory)
                var createdParts = new List<string>();
                var projectCode = request.ToolingParameters.BaseDrawingCode.ToString();

                // Create stations and plates
                foreach (var station in toolingStructure.Stations)
                {
                    // Create individual plates
                    foreach (var plate in station.Plates)
                    {
                        var part = _nxModelFactory.CreatePlate(plate, projectPath, projectCode);
                        createdParts.Add(part.FullPath);
                        result.CreatedPlates.Add(plate.Name);
                    }

                    // Create station assembly
                    var stationAssembly = _nxModelFactory.CreateStationAssembly(station, projectPath, projectCode);
                    createdParts.Add(stationAssembly.FullPath);
                    result.CreatedAssemblies.Add($"Station_{station.StationNumber}");
                }

                // Create shoes
                foreach (var shoe in toolingStructure.Shoes)
                {
                    var part = _nxModelFactory.CreateShoe(shoe, projectPath, projectCode);
                    createdParts.Add(part.FullPath);
                    result.CreatedShoes.Add(shoe.Name);
                }

                // Create parallel bars
                foreach (var parallelBar in toolingStructure.ParallelBars)
                {
                    var part = _nxModelFactory.CreateParallelBar(parallelBar, projectPath, projectCode);
                    createdParts.Add(part.FullPath);
                    result.CreatedParallelBars.Add(parallelBar.Name);
                }

                // Create common plates
                foreach (var commonPlate in toolingStructure.CommonPlates)
                {
                    var part = _nxModelFactory.CreateCommonPlate(commonPlate, projectPath, projectCode);
                    createdParts.Add(part.FullPath);
                    result.CreatedCommonPlates.Add(commonPlate.Name);
                }

                // 5. Save domain aggregate
                await _repository.SaveAsync(toolingStructure);

                // 6. Cleanup temporary files
                _fileSystemService.CleanupTempFiles(projectPath);

                result.Success = true;
                result.ProjectPath = projectPath;
                result.Message = $"Successfully created {createdParts.Count} components";

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to create tooling structure: {ex.Message}";
                result.Exception = ex;
                return result;
            }
        }        
    }
}