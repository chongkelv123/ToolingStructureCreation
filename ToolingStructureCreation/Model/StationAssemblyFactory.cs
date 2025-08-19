using NXOpen;
using NXOpen.CAE.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Model
{
    public class StationAssemblyFactory
    {
        public Dictionary<string, double> PlateThicknesses { get; set; }
        public List<Sketch> StationSketchLists;
        public List<Sketch> ShoeSketchLists;
        public List<Sketch> ComPltSketchLists;
        string folderPath = "";
        const string STN = "Stn";

        Controller.Control control;
        formToolStructure myForm;

        const double PARALLEL_BAR_WIDTH = 60.0; // Width of the parallel bar
        const double Y_POSITION = 0.0; // Y position is always 0.0 for the station assembly

        List<string> uprShoeComponentCollection;
        List<string> lowShoeComponentCollection;
        List<string> shoeComponentCollection;
        List<string> pBarComponentCollection;
        List<string> comPltComponentCollection;
        List<string> subToolAsmCollection;
        public StationAssemblyFactory(Dictionary<string, double> plateThicknesses, List<Sketch> stationSketchLists, List<Sketch> shoeSketchLists, List<Sketch> comPltSketchLists, string folderPath, Controller.Control control)
        {
            PlateThicknesses = plateThicknesses ?? new Dictionary<string, double>();
            StationSketchLists = stationSketchLists ?? new List<Sketch>();
            ShoeSketchLists = shoeSketchLists ?? new List<Sketch>();
            ComPltSketchLists = comPltSketchLists ?? new List<Sketch>();
            this.folderPath = folderPath;
            this.control = control;
            myForm = control.GetForm;
            shoeComponentCollection = new List<string>();

            uprShoeComponentCollection = new List<string>();
            lowShoeComponentCollection = new List<string>();
            pBarComponentCollection = new List<string>();
            comPltComponentCollection = new List<string>();
            subToolAsmCollection = new List<string>();
        }

        public void CreateStnAsmFactory(ToolingInfo toolingInfo)
        {
            //System.Diagnostics.Debugger.Launch();
            if (PlateThicknesses == null)
            {
                throw new InvalidOperationException("PlateThicknesses dictionary is not initialized. Please provide a valid dictionary of plate thicknesses.");
            }

            for (int i = 0; i < StationSketchLists.Count; i++)
            {
                Sketch stnSketch = StationSketchLists[i];
                int stnNo = i + 1;
                string stnNumber = STN + stnNo;
                List<string> pltComponentNames = new List<string>();
                Dictionary<string, double> pltLists = new Dictionary<string, double>();

                foreach (var plt in PlateThicknesses)
                {
                    if (plt.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                    {
                        // Skip material thickness, as it is not a plate
                        pltLists.Add(plt.Key, plt.Value);
                        continue;
                    }
                    var type = CodeGeneratorService.GetToolingType(plt.Key);

                    // ✅ UPDATED: Using UnifiedCodeGeneratorService instead of PlateCodeGeneratorService
                    var pltCodeGenerator = UnifiedCodeGeneratorService.CreateForPlate(
                        control,
                        myForm.GetProjectInfo(),
                        type,
                        stnNo);

                    string fileName = pltCodeGenerator.AskFileName();
                    pltLists.Add(fileName, plt.Value);
                    double plateWidth = toolingInfo.MaterialGuideType == MaterialGuideType.FullCoverage && type == ToolingStructureType.STRIPPER_PLATE
                        ? toolingInfo.CoilWidth - 11.0
                        : stnSketch.Width;
                    Plate plate = new Plate(fileName, stnSketch.Length, plateWidth, plt.Value);
                    string itemName2 = plt.Key.Replace("_", " ");
                    plate.CreateNewPlate(
                        folderPath,
                        myForm.GetProjectInfo(),
                        pltCodeGenerator.AskDrawingCode(),
                        itemName2);
                }

                string itemName = $"{stnNumber}-Assembly";

                // ✅ UPDATED: Using UnifiedCodeGeneratorService instead of AsmCodeGeneratorServicecs
                var asmCodeGenerator = UnifiedCodeGeneratorService.CreateForAssembly(
                    control,
                    myForm.GetProjectInfo(),
                    itemName);

                string subAsmFileName = asmCodeGenerator.AskFileName();
                subToolAsmCollection.Add(subAsmFileName);
                CreateStationAssembly(
                    pltLists,
                    subAsmFileName,
                    folderPath,
                    myForm.GetProjectInfo(),
                    asmCodeGenerator.AskDrawingCode(),
                    itemName);
            }

            Sketch shoeSketch = null;

            // Create Shoe
            ProjectInfo projectInfo = myForm.GetProjectInfo();
            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                shoeSketch = ShoeSketchLists[i];

                string uprShoeItemName = ShoeSketchLists.Count > 1
                    ? $"{Shoe.UPPER_SHOE}-{i + 1}"
                    : Shoe.UPPER_SHOE;
                var uprShoeGenerator = UnifiedCodeGeneratorService.CreateForShoe(
                    control,
                    projectInfo,
                    uprShoeItemName);

                //ShoeCodeGeneratorService uprShoeGenerator = new ShoeCodeGeneratorService(control, projectInfo, uprShoeItemName);

                string uprShoeFileNameWithoutExtension = uprShoeGenerator.AskFileName();
                uprShoeComponentCollection.Add(uprShoeFileNameWithoutExtension);
                Shoe upperShoe = new Shoe(uprShoeFileNameWithoutExtension, shoeSketch.Length, shoeSketch.Width, myForm.UpperShoeThk);
                string itemName1 = Shoe.UPPER_SHOE.Replace("_", " ");
                upperShoe.CreateNewShoe(
                    folderPath,
                    myForm.GetProjectInfo(),
                    uprShoeGenerator.AskDrawingCode(),
                    itemName1
                    );

                string lowShoeItemName = ShoeSketchLists.Count > 1
                    ? $"{Shoe.LOWER_SHOE}-{i + 1}"
                    : Shoe.LOWER_SHOE;
                var lowShoeGenerator = UnifiedCodeGeneratorService.CreateForShoe(control, projectInfo, lowShoeItemName);
                string lowShoeFileNameWithoutExtension = lowShoeGenerator.AskFileName();
                lowShoeComponentCollection.Add(lowShoeFileNameWithoutExtension);
                Shoe lowerShoe = new Shoe(lowShoeFileNameWithoutExtension, shoeSketch.Length, shoeSketch.Width, myForm.LowerShoeThk);
                string itemName2 = Shoe.LOWER_SHOE.Replace("_", " ");
                lowerShoe.CreateNewShoe(
                    folderPath,
                    myForm.GetProjectInfo(),
                    lowShoeGenerator.AskDrawingCode(),
                    itemName2
                    );
            }

            // Create Parallel Bar
            if (shoeSketch != null)
            {
                string pBarItemName = ParallelBar.PARALLEL_BAR;
                var pBarCodeGenerator = UnifiedCodeGeneratorService.CreateForShoe(control, myForm.GetProjectInfo(), pBarItemName);
                string fileName = pBarCodeGenerator.AskFileName();
                pBarComponentCollection.Add(fileName);
                ParallelBar parallelBar = new ParallelBar(fileName, PARALLEL_BAR_WIDTH, shoeSketch.Width - 85.0, myForm.ParallelBarThk);
                string itemName3 = pBarItemName.Replace("_", " ");
                parallelBar.CreateNewParallelBar(
                    folderPath,
                    myForm.GetProjectInfo(),
                    pBarCodeGenerator.AskDrawingCode(),
                    itemName3
                    );
            }

            // Create Common Plate
            string compPltItemName = CommonPlate.LOWER_COMMON_PLATE;
            string itemName4 = compPltItemName.Replace("_", " ");
            Machine machine = myForm.GetMachine;
            var commonPltInfo = machine.GetCommonPlate(myForm.GetMachineName);

            if (ComPltSketchLists.Count > 0)
            {
                // Create Common Plate (Double Joint Tool)
                for (int i = 0; i < ComPltSketchLists.Count; i++)
                {
                    compPltItemName = $"{CommonPlate.LOWER_COMMON_PLATE}-{i + 1}";
                    Sketch comPltSketch = ComPltSketchLists[i];
                    var compCodeGenerator = UnifiedCodeGeneratorService.CreateForShoe(control, myForm.GetProjectInfo(), compPltItemName);
                    string compPlatefileName = compCodeGenerator.AskFileName();
                    comPltComponentCollection.Add(compPlatefileName);

                    CommonPlateBase commonPlate;
                    if (i == 0)
                    {
                        commonPlate = new CommonPlateLeft(
                            comPltSketch.Length,
                            comPltSketch.Width,
                            myForm.CommonPltThk,
                            compPlatefileName);
                    }
                    else
                    {
                        commonPlate = new CommonPlateRight(
                            comPltSketch.Length,
                            comPltSketch.Width,
                            myForm.CommonPltThk,
                            compPlatefileName);
                    }

                    commonPlate.CreateNewCommonPlate(
                        folderPath,
                        myForm.GetProjectInfo(),
                        compCodeGenerator.AskDrawingCode(),
                        itemName4
                        );
                }
            }
            else
            {
                // Create Common Plate (Single)
                var compCodeGenerator = UnifiedCodeGeneratorService.CreateForShoe(control, myForm.GetProjectInfo(), compPltItemName);
                string compPlatefileName = compCodeGenerator.AskFileName();
                comPltComponentCollection.Add(compPlatefileName);
                CommonPlateBase commonPlate = new CommonPlate(commonPltInfo.GetLength(), commonPltInfo.GetWidth(), myForm.CommonPltThk, compPlatefileName);
                commonPlate.CreateNewCommonPlate(
                    folderPath,
                    myForm.GetProjectInfo(),
                    compCodeGenerator.AskDrawingCode(),
                    itemName4
                    );
            }
        }

        public void CreateToolAsmFactory(ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = ToolingAssembly.TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = ToolingAssembly.ASSEMBLY_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = ToolingAssembly.ASSEMBLY;
            fileNew.SetCanCreateAltrep(false);

            //string itemName = "MainToolAssembly";
            var asmCodeGenerator = UnifiedCodeGeneratorService.CreateForAssembly(
                control,
                myForm.GetProjectInfo(),
                itemName);

            string fileName = asmCodeGenerator.AskFileName();
            fileNew.NewFileName = $"{folderPath}{fileName}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;

            try
            {
                NXObject shoeObject = fileNew.Commit();

                Part workAssy = session.Parts.Work;
                Part displayPart = session.Parts.Display;

                fileNew.Destroy();

                session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

                // Insert Strip Layout
                StripLayout stripLayout = control.GetStripLayout;
                Shoe.Insert(workAssy, stripLayout.GetFileNameWithoutExtension, stripLayout.GetPosition, folderPath);

                // Orient the work view to Isometric
                workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

                // Insert Station Assembly
                for (int i = 0; i < StationSketchLists.Count; i++)
                {
                    Sketch stnSketch = StationSketchLists[i];
                    string compName = subToolAsmCollection[i];
                    Point3d startLocation = stnSketch.StartLocation;
                    double stnAsmZPosition = myForm.GetDiePlt_LowPadThk() * -1;

                    ToolingAssembly.InsertStationAssembly(
                        workAssy,
                        compName,
                        new Point3d(startLocation.X, Y_POSITION, stnAsmZPosition),
                        folderPath);
                }

                // Insert Shoe, Parallel Bar, and Common Plate
                for (int i = 0; i < ShoeSketchLists.Count; i++)
                {
                    Sketch shoeSketch = ShoeSketchLists[i];
                    double uprShoeZPosition = myForm.GetUpperShoeZPosition();
                    double lowShoeZPosition = myForm.GetDiePlt_LowPadThk() * -1;
                    string uprShoeCompName = uprShoeComponentCollection[i];
                    string lowShoeCompName = lowShoeComponentCollection[i];
                    Shoe.Insert(
                            workAssy,
                            uprShoeCompName,
                            new Point3d(shoeSketch.StartLocation.X, Y_POSITION, uprShoeZPosition),
                            folderPath);
                    Shoe.Insert(
                            workAssy,
                            lowShoeCompName,
                            new Point3d(shoeSketch.StartLocation.X, Y_POSITION, lowShoeZPosition),
                            folderPath);

                    foreach (var compName in pBarComponentCollection)
                    {
                        double firstParallelBarXPosition = shoeSketch.StartLocation.X + (PARALLEL_BAR_WIDTH / 2.0);
                        double lastParallelBarXPosition = shoeSketch.StartLocation.X + shoeSketch.Length - PARALLEL_BAR_WIDTH / 2.0;
                        double distBetweenFirstLastPBars = (lastParallelBarXPosition - firstParallelBarXPosition);
                        const double DIST_BETWEEN_PBAR = 330.0;
                        int numberOfParallelBars = (int)Math.Ceiling(distBetweenFirstLastPBars / DIST_BETWEEN_PBAR);
                        Shoe.Insert(workAssy, compName, new Point3d(firstParallelBarXPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);
                        Shoe.Insert(workAssy, compName, new Point3d(lastParallelBarXPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);

                        for (int j = 0; j < numberOfParallelBars - 2; j++)
                        {
                            double dist = distBetweenFirstLastPBars / (numberOfParallelBars - 1);
                            double xPosition = firstParallelBarXPosition + (j + 1) * dist;
                            Shoe.Insert(workAssy, compName, new Point3d(xPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);
                        }
                    }

                    if (ComPltSketchLists == null || ComPltSketchLists.Count == 0)
                    {
                        if (i >= 0 && i < comPltComponentCollection.Count)
                        {
                            string comPltCompName = comPltComponentCollection[i];

                            Shoe.Insert(
                                workAssy,
                                comPltCompName,
                                new Point3d(shoeSketch.MidPoint.X, Y_POSITION, myForm.GetCommonPlateZPosition()),
                                folderPath);
                        }
                    }
                }

                // Insert Common Plate (Double Joint Tool)            
                for (int i = 0; i < ComPltSketchLists.Count; i++)
                {
                    Sketch comPltSketch = ComPltSketchLists[i];

                    if (i >= 0 && i < comPltComponentCollection.Count)
                    {
                        string comPltCompName = comPltComponentCollection[i];
                        double comPltZPosition = myForm.GetCommonPlateZPosition();
                        Shoe.Insert(
                            workAssy,
                            comPltCompName,
                            new Point3d(comPltSketch.MidPoint.X, Y_POSITION, comPltZPosition),
                            folderPath);
                    }
                }

                NXDrawing.UpdatePartProperties(
                    projectInfo,
                    drawingCode,
                    itemName,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    PartProperties.ASM);

                BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
                BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
                workAssy.Save(saveComponentParts, save);

                workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
            }
            catch (NXOpen.NXException nxEx) when (nxEx.Message.Contains("File already exists"))
            {                
                // User-friendly error handling
                string message = $"File already exists: {fileName}{NXDrawing.EXTENSION}\n\n" +
                                $"Location: {folderPath}\n\n" +
                                "Please:\n" +
                                "• Delete the existing file, or\n" +
                                "• Choose a different output directory, or\n" +
                                "• Modify the project code prefix";

                string title = "File Conflict";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Warning, message);

                // Re-throw to stop the creation process
                throw new InvalidOperationException($"Cannot create plate '{fileName}' - file already exists", nxEx);
            }
            catch (NXOpen.NXException nxEx)
            {                
                // Handle other NX-specific errors
                string message = $"NX Error creating plate '{fileName}':\n{nxEx.Message}";
                string title = "NX Operation Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw new InvalidOperationException($"Failed to create plate '{fileName}'", nxEx);
            }
            catch (Exception ex)
            {                
                // Handle unexpected errors
                string message = $"Unexpected error creating plate '{fileName}':\n{ex.Message}";
                string title = "Unexpected Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw;
            }

        }

        public void CreateStationAssembly(Dictionary<string, double> plateList, string fileName, string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = ToolingAssembly.TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = ToolingAssembly.ASSEMBLY_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = ToolingAssembly.ASSEMBLY;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{fileName}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;

            try
            {
                NXObject shoeObject = fileNew.Commit();

                Part workPart = session.Parts.Work;
                Part displayPart = session.Parts.Display;

                fileNew.Destroy();

                session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

                workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

                double cumThk = 0.0;
                foreach (var component in plateList)
                {
                    cumThk += component.Value;
                    if (component.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                    {
                        continue; // Skip the material thickness entry
                    }
                    string fn = component.Key;
                    Plate.InsertPlate(workPart, fn, cumThk, folderPath);
                }

                NXDrawing.UpdatePartProperties(
                    projectInfo,
                    drawingCode,
                    itemName,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    NXDrawing.HYPHEN,
                    PartProperties.ASM);

                BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
                BasePart.CloseAfterSave save = BasePart.CloseAfterSave.True;
                workPart.Save(saveComponentParts, save);
            }
            catch (NXOpen.NXException nxEx) when (nxEx.Message.Contains("File already exists"))
            {                
                // User-friendly error handling
                string message = $"File already exists: {fileName}{NXDrawing.EXTENSION}\n\n" +
                                $"Location: {folderPath}\n\n" +
                                "Please:\n" +
                                "• Delete the existing file, or\n" +
                                "• Choose a different output directory, or\n" +
                                "• Modify the project code prefix";

                string title = "File Conflict";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Warning, message);

                // Re-throw to stop the creation process
                throw new InvalidOperationException($"Cannot create plate '{fileName}' - file already exists", nxEx);
            }
            catch (NXOpen.NXException nxEx)
            {                
                // Handle other NX-specific errors
                string message = $"NX Error creating plate '{fileName}':\n{nxEx.Message}";
                string title = "NX Operation Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw new InvalidOperationException($"Failed to create plate '{fileName}'", nxEx);
            }
            catch (Exception ex)
            {                
                // Handle unexpected errors
                string message = $"Unexpected error creating plate '{fileName}':\n{ex.Message}";
                string title = "Unexpected Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw;
            }

        }
    }
}