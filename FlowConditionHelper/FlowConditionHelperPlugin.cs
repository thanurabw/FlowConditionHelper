﻿using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace FlowConditionHelper
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "FetchXML to Flow Condition Converter"),
        ExportMetadata("Description", "This is a plugin built to convert FetchXML conditions to Microsoft Flow conditions"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", null),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", null),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class FlowConditionHelperPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new FlowConditionHelperPluginControl();
        }
    }
}