using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class SketchSelection
    {
        NXDrawing drawing;
        Session session;
        UFSession ufs;
        UI ui;

        public double DiagonalLength { get; set; }
        //List<Point3d> sortedPoints;
        bool showDebugMessages = false;

        public SketchSelection(NXDrawing drawing)
        {
            this.drawing = drawing;
            session = drawing.GetSession();
            ui = drawing.GetUI();
            ufs = UFSession.GetUFSession();
        }

        public TaggedObject[] SelectSketch()
        {
            var selctionManager = ui.SelectionManager;
            string message = "Select a sketch for the plate";
            string title = "Select Sketch";
            Selection.SelectionScope scope = Selection.SelectionScope.WorkPart;
            Selection.MaskTriple[] maskTriples = new Selection.MaskTriple[] {
                new Selection.MaskTriple(
                    NXOpen.UF.UFConstants.UF_sketch_type,
                    NXOpen.UF.UFConstants.UF_all_subtype,
                    0)
            };
            Selection.SelectionAction action = Selection.SelectionAction.ClearAndEnableSpecific;
            bool includeFeatures = false;
            bool keepHighlighted = false;
            TaggedObject[] objectArray;

            Selection.Response response = selctionManager.SelectTaggedObjects(
                message,
                title,
                scope,
                action,
                includeFeatures,
                keepHighlighted,
                maskTriples,
                out objectArray
                );

            return objectArray;
        }

        public Sketch AskSketchBoudingBox(TaggedObject sketchObject)
        {
            Tag skecth_tag = sketchObject.Tag;
            UFSket.Info sket_info = new UFSket.Info();
            ufs.Sket.AskSketchInfo(
                skecth_tag,
                out sket_info);

            NXOpen.Sketch sketch = sketchObject as NXOpen.Sketch;

            //double[] bBox = new double[6];
            double[] bBox = new double[6];
            double[] overallBbox = new double[6];
            NXObject[] nXObjects = sketch.GetAllGeometry();
            Model.Sketch modelSketch = new Model.Sketch();
            if (!modelSketch.IsRectangle(nXObjects))
            {
                return null;
            }

            //sortedPoints = SortPoints(modelSketch.PointCollections);
            NXObject[] curveObjs = sketch.GetAllGeometry();
            ufs.Modl.AskBoundingBox(curveObjs[0].Tag, bBox);
            overallBbox[0] = bBox[0];
            overallBbox[1] = bBox[1];
            overallBbox[2] = bBox[2];
            overallBbox[3] = bBox[3];
            overallBbox[4] = bBox[4];
            overallBbox[5] = bBox[5];

            foreach (var cObj in curveObjs)
            {
                ufs.Modl.AskBoundingBox(cObj.Tag, bBox);
                overallBbox[0] = Math.Min(overallBbox[0], bBox[0]); // xmin
                overallBbox[1] = Math.Min(overallBbox[1], bBox[1]); // ymin
                overallBbox[2] = Math.Min(overallBbox[2], bBox[2]); // zmin
                overallBbox[3] = Math.Max(overallBbox[3], bBox[3]); // xmax
                overallBbox[4] = Math.Max(overallBbox[4], bBox[4]); // ymax
                overallBbox[5] = Math.Max(overallBbox[5], bBox[5]); // zmax
            }

            string message = "";
            message += $"\nMinimum Point for Sketch: \"{sket_info.name}\": {overallBbox[0]}, {overallBbox[1]}, {overallBbox[2]}";
            message += $"\nMaximum Point for Sketch: \"{sket_info.name}\": {overallBbox[3]}, {overallBbox[4]}, {overallBbox[5]}";
            message += $"\nLength: {GetBoundingBoxLength(overallBbox)}";
            message += $"\nWidth: {GetBoundingBoxWidth(overallBbox)}";
            message += $"\nMid Point: {GetBoundingBoxMidPointX(overallBbox).X}, {GetBoundingBoxMidPointX(overallBbox).Y}, {GetBoundingBoxMidPointX(overallBbox).Z}";
            message += $"\nStart Location: {GetStartLocation(overallBbox).X}, {GetStartLocation(overallBbox).Y}, {GetStartLocation(overallBbox).Z}";
            message += "\n**********";

            if(showDebugMessages)
            {
                Guide.InfoWriteLine(message);
            }
                
            return new Sketch(
                GetBoundingBoxLength(overallBbox),
                GetBoundingBoxWidth(overallBbox),
                GetStartLocation(overallBbox),
                GetBoundingBoxMidPointX(overallBbox));
        }

        public static double GetBoundingBoxDiagonal(double[] bBox)
        {
            double dx = GetBoundingBoxLength(bBox);
            double dy = GetBoundingBoxWidth(bBox);
            double dz = GetBoundingBoxHeight(bBox);
            return Math.Sqrt(dx * dx + dy * dy + dz * dz); // sqrt(dx^2 + dy^2 + dz^2)
        }

        public static List<Point3d> SortPoints(List<Point3d> points)
        {
            // Sort points by X, then Y, then Z
            return points
                .OrderBy(p => p.X)
                .ThenBy(p => p.Y)
                .ThenBy(p => p.Z)
                .ToList();
        }

        public static double GetBoundingBoxHeight(double[] bBox)
        {
            return bBox[5] - bBox[2]; // zmax - zmin
        }
        public static double GetBoundingBoxHeight(List<Point3d> points)
        {
            return points[1].Z - points[0].Z; // zmax - zmin
        }

        public static double GetBoundingBoxLength(double[] bBox)
        {
            return bBox[3] - bBox[0]; // xmax - xmin
        }
        public static double GetBoundingBoxLength(List<Point3d> points)
        {
            return points[3].X - points[0].X; // xmax - xmin
        }

        public static double GetBoundingBoxWidth(double[] bBox)
        {
            return bBox[4] - bBox[1]; // ymax - ymin
        }
        public static double GetBoundingBoxWidth(List<Point3d> points)
        {
            return points[1].Y - points[0].Y; // ymax - ymin
        }

        public static Point3d GetBoundingBoxMidPointX(double[] bBox)
        {
            double midX = (bBox[0] + bBox[3]) / 2.0; // (xmin + xmax) / 2
            double midY = (bBox[1] + bBox[4]) / 2.0; // (ymin + ymax) / 2
            double midZ = (bBox[2] + bBox[5]) / 2.0; // (zmin + zmax) / 2
            return new Point3d(midX, midY, midZ);
        }
        public static Point3d GetBoundingBoxMidPointX(List<Point3d> points)
        {
            double midX = GetBoundingBoxLength(points) / 2.0; // (xmin + xmax) / 2
            double midY = GetBoundingBoxWidth(points) / 2.0; // (ymin + ymax) / 2
            double midZ = GetBoundingBoxHeight(points) / 2.0; // (zmin + zmax) / 2
            return new Point3d(midX, midY, midZ);
        }

        public static Point3d GetStartLocation(double[] bBox)
        {
            Point3d midPoint = GetBoundingBoxMidPointX(bBox);
            double length = GetBoundingBoxLength(bBox);
            return new Point3d(midPoint.X - length / 2, midPoint.Y, midPoint.Z);
        }
        public static Point3d GetStartLocation(List<Point3d> points)
        {
            Point3d midPoint = GetBoundingBoxMidPointX(points);
            double length = GetBoundingBoxLength(points);
            return new Point3d(midPoint.X - length / 2, midPoint.Y, midPoint.Z);
        }

        public List<Sketch> AskListFromTaggedObjects(TaggedObject[] taggedObjects)
        {
            //System.Diagnostics.Debugger.Launch();
            List<Sketch> sketchList = new List<Sketch>();
            foreach (var tagObj in taggedObjects)
            {
                Sketch sketch = AskSketchBoudingBox(tagObj);
                if (sketch != null)
                {
                    sketchList.Add(sketch);
                }
            }
            return sketchList;
        }
    }
}
