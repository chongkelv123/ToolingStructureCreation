﻿using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.CAE;
using NXOpen.CAE.Connections;
using NXOpen.Display;
using NXOpen.Features;
using NXOpen.Layout2d;
using NXOpen.UF;
using NXOpenUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Controller;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Model
{
    public enum PlateColor
    {
        UPPERSHOE = 127,
        UPPERPAD = 91,
        PUNCHHOLDER = 59,
        BOTTOMINGPLATE = 124,
        STRIPPERPLATE = 55,
        DIEPLATE = 108,
        LOWERPAD = 92,
        LOWERSHOE = 60,
        COMMONPLATE = 80,
        PARALLELBAR = 87,
    }

    public class NXDrawing
    {
        Session session;
        Part workPart;
        UI ui;
        UFSession ufs;
        Controller.Control control;

        public const string LOWER_PAD = "LOWER_PAD";
        public const string DIE_PLATE = "DIE_PLATE";
        public const string MAT_THK = "mat_thk";
        public const string STRIPPER_PLATE = "STRIPPER_PLATE";
        public const string BOTTOMING_PLATE = "BOTTOMING_PLATE";
        public const string PUNCH_HOLDER = "PUNCH_HOLDER";
        public const string UPPER_PAD = "UPPER_PAD";

        public const string UG_APP_MODELING = "UG_APP_MODELING";
        public const string MODEL = "MODEL";
        public const string EXTENSION = ".prt";
        public const string MODEL_TEMPLATE = "ModelTemplate";

        public const string FIFTYTWO_FIFTYFOUR = "52~54";
        public const string GOA = "GOA";
        public const string S50C = "S50C";
        public const string HYPHEN = "-";

        bool showDebugMessages = false;

        public NXDrawing()
        {
        }

        public NXDrawing(Controller.Control control)
        {
            session = Session.GetSession();
            ufs = UFSession.GetUFSession();
            workPart = session.Parts.Work;
            ui = UI.GetUI();


            this.control = control;
        }

        public static void ShowMessageBox(string title, NXMessageBox.DialogType msgboxType, string message)
        {
            UI ui = UI.GetUI();
            ui.NXMessageBox.Show(title, msgboxType, message);
        }

        public Session GetSession()
        {
            return session;
        }
        public UI GetUI()
        {
            return ui;
        }
        public UFSession GetUFSession()
        {
            return ufs;
        }
        public Part GetWorkPart()
        {
            return workPart;
        }

        public double GetUpperShoe_ZValue()
        {
            formToolStructure myForm = control.GetForm;
            double totalPlateThickness =
                myForm.UpperPadThk +
                myForm.PunHolderThk +
                myForm.BottomPltThk +
                myForm.StripperPltThk +
                myForm.MatThk +
                myForm.DiePltThk +
                myForm.LowerPadThk +
                myForm.UpperShoeThk;

            return totalPlateThickness;
        }

        public bool IsDrawingOpen()
        {
            string title = "No active drawing";
            string message = "You accidentally launched the Tooling Structure Creation command by mistake and ";
            NXMessageBox.DialogType msgboxType = NXMessageBox.DialogType.Warning;

            Part displayPart = session.Parts.Display;
            bool isNoCompnent = displayPart == null;

            if (isNoCompnent)
            {
                message += " you are not open any drawings yet! ;-)";
                ShowMessageBox(title, msgboxType, message);
                return false;
            }

            return true;
        }
        public StripLayout GetStripLayout()
        {
            var fullPath = workPart.FullPath;
            var position = new Point3d(0.0, 0.0, 0.0); // Default position, can be modified later            
            if (showDebugMessages)
            {
                Guide.InfoWriteLine($"Full path: {fullPath}");
            }
            return new StripLayout(fullPath, position);
        }

        public static void UpdatePartProperties(ProjectInfo projectInfo, string drawingCode, string itemName, string length, string thickness, string width, string hrc, string material, string partType)
        {
            Part workPart = Session.GetSession().Parts.Work;
            TitleBlockProperties properties = new TitleBlockProperties(
                workPart,
                projectInfo.Designer,
                drawingCode,
                hrc,
                itemName,
                length,
                thickness,
                width,
                material,
                projectInfo.Model,
                projectInfo.Part
                );
            var attrList = properties.AttributeInfoToList(PartProperties.CATEGORY_TITLE, properties.KeyValueInfo);
            properties.SetAttributesByList(attrList);

            NXObject.AttributeInformation info = new NXObject.AttributeInformation();
            info.Category = PartProperties.CATEGORY_TOOL;
            info.Title = PartProperties.PART_TYPE;
            info.Type = NXObject.AttributeType.String;
            info.StringValue = partType;
            properties.SetAttribute(info);
        }
    }
}


