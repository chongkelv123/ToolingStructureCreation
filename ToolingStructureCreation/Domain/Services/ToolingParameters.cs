using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Domain.Services
{
    public class ToolingParameters
    {
        public Dictionary<PlateType, double> PlateThicknesses { get; }
        public double MaterialThickness { get; }
        public double UpperShoeThickness { get; }
        public double LowerShoeThickness { get; }
        public double ParallelBarThickness { get; }
        public double CommonPlateThickness { get; }
        public MachineSpecification MachineSpec { get; }
        public DrawingCode BaseDrawingCode { get; }
        public string ProjectName { get; }
        public string Designer { get; }

        public ToolingParameters(Dictionary<PlateType, double> plateThicknesses, double materialThickness,
            double upperShoeThickness, double lowerShoeThickness, double parallelBarThickness,
            double commonPlateThickness, MachineSpecification machineSpec, DrawingCode baseDrawingCode,
            string projectName, string designer)
        {
            PlateThicknesses = plateThicknesses ?? throw new ArgumentNullException(nameof(plateThicknesses));
            MaterialThickness = materialThickness;
            UpperShoeThickness = upperShoeThickness;
            LowerShoeThickness = lowerShoeThickness;
            ParallelBarThickness = parallelBarThickness;
            CommonPlateThickness = commonPlateThickness;
            MachineSpec = machineSpec;
            BaseDrawingCode = baseDrawingCode;
            ProjectName = projectName;
            Designer = designer;
        }

        // Factory method to create from Form
        public static ToolingParameters FromForm(formToolStructure form, MachineSpecification machineSpec,
            DrawingCode baseDrawingCode, string projectName, string designer)
        {
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, form.UpperPadThk },
                { PlateType.Punch_Holder, form.PunHolderThk },
                { PlateType.Bottoming_Plate, form.BottomPltThk },
                { PlateType.Stripper_Plate, form.StripperPltThk },
                { PlateType.Die_Plate, form.DiePltThk },
                { PlateType.Lower_Pad, form.LowerPadThk }
            };

            return new ToolingParameters(
                plateThicknesses,
                form.MatThk,
                form.UpperShoeThk,
                form.LowerShoeThk,
                form.ParallelBarThk,
                form.CommonPltThk,
                machineSpec,
                baseDrawingCode,
                projectName,
                designer
            );
        }
    }
}
