﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateNewPlate.Model
{
    public class StationToolingStructure
    {
        Dictionary<string, double> plateThicknesses;
        double plateWidth;
        double plateLength;
        string stationNumber;
        NXDrawing drawing;
        string folderPath;

        public StationToolingStructure(double plateWidth, double plateLength, string stationNumber, NXDrawing drawing, string folderPath)
        {
            plateThicknesses = new Dictionary<string, double>
            {
                { "LOWER_PAD", 25.0 },
                { "DIE_PLATE", 35.0 },
                { "mat_thk", 1.55 }, // Material thickness, not a plate
                { "STRIPPER_PLATE", 30.0 },
                { "BOTTOMING_PLATE", 16.0 },
                { "PUNCH_HOLDER", 30.0 },
                { "UPPER_PAD", 27.0 }
            };
            this.plateWidth = plateWidth;
            this.plateLength = plateLength;
            this.stationNumber = stationNumber;
            this.drawing = drawing;
            this.folderPath = folderPath;
        }

        public Dictionary<string, double> GetPlateThicknesses()
        {
            return plateThicknesses;
        }

        public double GetTotalThickness()
        {
            double totalThickness = 0.0;
            foreach (var plate in plateThicknesses)
            {
                if (!plate.Key.Equals("mat_thk", StringComparison.OrdinalIgnoreCase))
                {
                    totalThickness += plate.Value;
                }
            }
            return totalThickness;
        }

        public double GetPlateWidth()
        {
            return plateWidth;
        }

        public double GetPlateLength()
        {
            return plateLength;
        }

        public string GetStationNumber()
        {
            return stationNumber;
        }

        public void CreateStationFactory()
        {
            var list = GetPlateThicknesses();
            foreach (var plate in list)
            {
                if (plate.Key.Equals("mat_thk", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip material thickness, as it is not a plate
                    continue;
                }
                Plate createPlate = new Plate(plate.Key, GetPlateLength(), GetPlateWidth(), plate.Value, drawing);
                createPlate.CreateNewPlate(folderPath, stationNumber);
            }

            drawing.CreateStationAssembly(list, GetStationNumber(), folderPath);
        }
    }
}
