using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;

namespace FlowConditionHelper
{
    public partial class FlowConditionHelperPluginControl : PluginControlBase
    {
        private Settings mySettings;

        public FlowConditionHelperPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This plugin does not require connecting to an organisation. For more info on Flow conditions please visit Microsoft Docs page.", new Uri("https://docs.microsoft.com/en-us/flow/add-condition"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        /// <summary>
        /// This event occurs when the text is changed in the fetchxml rich text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richtxtFetchXml_TextChanged(object sender, EventArgs e)
        {
            HighlightRTF(richtxtFetchXml);
        }

        /// <summary>
        /// This method applies xml colour scheme to a rich text box
        /// </summary>
        /// <param name="rtb"></param>
        public static void HighlightRTF(RichTextBox rtb)
        {
            int k = 0;

            string str = rtb.Text;

            int st, en;
            int lasten = -1;
            while (k < str.Length)
            {
                st = str.IndexOf('<', k);

                if (st < 0)
                    break;

                if (lasten > 0)
                {
                    rtb.Select(lasten + 1, st - lasten - 1);
                    rtb.SelectionColor = HighlightColors.HC_INNERTEXT;
                }

                en = str.IndexOf('>', st + 1);
                if (en < 0)
                    break;

                k = en + 1;
                lasten = en;

                if (str[st + 1] == '!')
                {
                    rtb.Select(st + 1, en - st - 1);
                    rtb.SelectionColor = HighlightColors.HC_COMMENT;
                    continue;

                }
                String nodeText = str.Substring(st + 1, en - st - 1);


                bool inString = false;

                int lastSt = -1;
                int state = 0;
                /* 0 = before node name
                 * 1 = in node name
                   2 = after node name
                   3 = in attribute
                   4 = in string
                   */
                int startNodeName = 0, startAtt = 0;
                for (int i = 0; i < nodeText.Length; ++i)
                {
                    if (nodeText[i] == '"')
                        inString = !inString;

                    if (inString && nodeText[i] == '"')
                        lastSt = i;
                    else
                        if (nodeText[i] == '"')
                    {
                        rtb.Select(lastSt + st + 2, i - lastSt - 1);
                        rtb.SelectionColor = HighlightColors.HC_STRING;
                    }

                    switch (state)
                    {
                        case 0:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startNodeName = i;
                                state = 1;
                            }
                            break;
                        case 1:
                            if (Char.IsWhiteSpace(nodeText, i))
                            {
                                rtb.Select(startNodeName + st, i - startNodeName + 1);
                                rtb.SelectionColor = HighlightColors.HC_NODE;
                                state = 2;
                            }
                            break;
                        case 2:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startAtt = i;
                                state = 3;
                            }
                            break;

                        case 3:
                            if (Char.IsWhiteSpace(nodeText, i) || nodeText[i] == '=')
                            {
                                rtb.Select(startAtt + st, i - startAtt + 1);
                                rtb.SelectionColor = HighlightColors.HC_ATTRIBUTE;
                                state = 4;
                            }
                            break;
                        case 4:
                            if (nodeText[i] == '"' && !inString)
                                state = 2;
                            break;


                    }

                }
                if (state == 1)
                {
                    rtb.Select(st + 1, nodeText.Length);
                    rtb.SelectionColor = HighlightColors.HC_NODE;
                }


            }
        }

        /// <summary>
        /// This event occurs when the convert button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                //Add styling to fetchXML text area
                HighlightRTF(richtxtFetchXml);

                //Process XML
                XDocument doc = XDocument.Parse(richtxtFetchXml.Text);

                //pre-validations of the XML
                string errors = validateFetchCompatibilityToFlow(doc);
                if (!string.IsNullOrEmpty(errors))
                {
                    richtxtCondition.Text = @"Failed to generate Flow conditions. The fetchXML is not compatible." + Environment.NewLine + errors;
                    return;
                }

                richtxtCondition.Text = "@";

                var filter = doc.Descendants("filter").FirstOrDefault();
                if (filter != null)
                {
                    richtxtCondition.Text += ConvertFilterConditions(filter.ToString());
                }

            }
            catch (Exception ex)
            {
                richtxtCondition.Text = @"Failed to generate Flow conditions. The fetchXML is not compatible." + Environment.NewLine + ex.Message;
            }
        }

        /// <summary>
        /// This method converts '<filter>' nodes on fetchxml to Flow conditions
        /// </summary>
        /// <param name="xmlFilter"></param>
        private string ConvertFilterConditions(string xmlFilter)
        {
            string flowFilter = "";

            XDocument doc = XDocument.Parse(xmlFilter);

            #region Read Sub Filters

            var filters = doc.Elements("filter");

            foreach (XElement current in filters)
            {
                // new condition to be appended, so add a , at the start
                if (string.IsNullOrEmpty(flowFilter) == false) flowFilter += ",";

                foreach (var attribute in current.Attributes())
                {
                    if (attribute.Name.ToString().ToLower().Equals("type"))
                    {
                        if (attribute.Value.ToLower().Contains("or"))
                        {
                            flowFilter += "or(";
                        }
                        else
                        {
                            flowFilter += "and(";
                        }
                    }
                }

                string subFilterConditions = "";

                foreach (var node in current.Nodes())
                {
                    // new condition to be appended, so add a , at the start
                    if (string.IsNullOrEmpty(subFilterConditions) == false) subFilterConditions += ",";

                    subFilterConditions += ConvertFilterConditions(node.ToString());
                }

                flowFilter += string.Format("{0})", subFilterConditions);
            }

            #endregion

            #region Read Condition Nodes

            var conditions = doc.Elements("condition");

            foreach (XElement current in conditions)
            {
                // new condition to be appended, so add a , at the start
                if (string.IsNullOrEmpty(flowFilter) == false && flowFilter.EndsWith("(") == false) flowFilter += ",";

                //convert condition to Flow condition
                flowFilter += convertFetchConditiontoFlowCondition(current.ToString());
            }

            #endregion

            return flowFilter;
        }

        /// <summary>
        /// This method checks compatibility of the fetchxml against Flow conditions
        /// </summary>
        /// <param name="doc"></param>
        private string validateFetchCompatibilityToFlow(XDocument doc)
        {
            string errors = "";
            var linkedEntities = doc.Descendants("link-entity");
            if (linkedEntities.Any())
            {
                errors += "Linked entities found in the FetchXML.";
            }
            return errors;
        }

        /// <summary>
        /// This method converts '<condition>' nodes on fetchxml to Flow conditions
        /// </summary>
        /// <param name="conditionsXml"></param>
        private string convertFetchConditiontoFlowCondition(string conditionsXml)
        {
            string appendedConditions = "";
            XDocument doc = XDocument.Parse(conditionsXml);
            var ConditionNodes = doc.Elements("condition");
            string stepToApplyConditions = "triggerBody()";

            if (radiobtnCustom.Checked) stepToApplyConditions = txtStep.Text;

            foreach (var conditionNode in ConditionNodes)
            {
                string attributeName = "";
                string operatorType = "";
                string value = "";

                foreach (var attribute in conditionNode.Attributes())
                {
                    string checkName = attribute.Name.ToString().ToLower();
                    switch (checkName)
                    {
                        case "attribute":
                            attributeName = string.Format("{0}?['{1}']", stepToApplyConditions, attribute.Value);
                            break;
                        case "operator":
                            operatorType = attribute.Value;
                            break;
                        case "value":
                            value = attribute.Value;
                            break;
                        default:
                            continue;
                    }
                }
                
                appendedConditions += GenerateCondition(attributeName,operatorType,value, conditionNode.ToString());
            }

            return appendedConditions;
        }

        /// <summary>
        /// This method generates Flow conditions
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="operatorType"></param>
        /// <param name="value"></param>
        /// /// <param name="conditionXml"></param>
        private string GenerateCondition(string attributeName, string operatorType, string value, string conditionXml)
        {
            string conditiontoAppend = "";
            bool isGuidOrInt = false;
            int intVal;
            switch (operatorType)
            {
                case "in":
                    //add equals condition as multiple lines inside an or() group
                    XDocument doc = XDocument.Parse(conditionXml);
                    var values = doc.Descendants("value");
                    value = "";
                    conditiontoAppend += "or(";
                    int count = 0;
                    foreach (XElement element in values)
                    {
                        if (count > 0) conditiontoAppend += ",";

                        if (element.Value.StartsWith("{") && element.Value.EndsWith("}"))
                        {
                            value = element.Value.Replace("{", "'").Replace("}", "'").ToLower();
                            isGuidOrInt = true;
                        }
                        else
                        {
                            isGuidOrInt = true;
                            value = element.Value;
                        }

                        conditiontoAppend += FlowOperators.equals.Replace("<attributename>", attributeName)
                            .Replace("<value>", value);

                        count++;
                    }
                    conditiontoAppend += ")";
                    break;
                case "eq":
                    conditiontoAppend = FlowOperators.equals;
                    if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        isGuidOrInt = true;
                        value = value.Replace("{", "'").Replace("}", "'").ToLower();
                    }

                    if(int.TryParse(value,out intVal)) // room to improve to check against field name meta data
                    {
                        isGuidOrInt = true;
                    }
                    break;
                case "ne":
                    conditiontoAppend = FlowOperators.doesnotequal;
                    if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        isGuidOrInt = true;
                        value = value.Replace("{", "'").Replace("}", "'").ToLower();
                    }

                    if (int.TryParse(value, out intVal)) // room to improve to check against field name meta data
                    {
                        isGuidOrInt = true;
                    }
                    break;
                case "null":
                    conditiontoAppend = FlowOperators.equals;
                    value = "null";
                    break;
                case "not-null":
                    conditiontoAppend = FlowOperators.doesnotequal;
                    value = "null";
                    break;
                case "gt":
                    conditiontoAppend = FlowOperators.greaterthan;
                    if (int.TryParse(value, out intVal))
                    {
                        isGuidOrInt = true;
                    }
                    break;
                case "ge":
                    conditiontoAppend = FlowOperators.greaterthanOrEqual;
                    if (int.TryParse(value, out intVal))
                    {
                        isGuidOrInt = true;
                    }
                    break;
                case "lt":
                    conditiontoAppend = FlowOperators.lessthan;
                    if (int.TryParse(value, out intVal))
                    {
                        isGuidOrInt = true;
                    }
                    break;
                case "le":
                    conditiontoAppend = FlowOperators.lessthanOrEqual;
                    if (int.TryParse(value, out intVal))
                    {
                        isGuidOrInt = true;
                    }
                    break;
                case "begins-with":
                    conditiontoAppend = FlowOperators.startsWith;
                    break;
                case "not-begin-with":
                    conditiontoAppend = FlowOperators.doesnotstartWith;
                    break;
                case "ends-with":
                    conditiontoAppend = FlowOperators.endswith;
                    break;
                case "not-end-with":
                    conditiontoAppend = FlowOperators.doesnotendwith;
                    break;
                case "like":
                    if(!value.StartsWith("%") && !value.EndsWith("%"))
                    {
                        conditiontoAppend = FlowOperators.contains;
                    }
                    else if (!value.StartsWith("%") && value.EndsWith("%"))
                    {
                        conditiontoAppend = FlowOperators.startsWith;
                    }
                    else if (value.StartsWith("%") && !value.EndsWith("%"))
                    {
                        conditiontoAppend = FlowOperators.endswith;
                    }
                    value = value.Replace("%", "");
                    break;
                case "not-like":
                    if (!value.StartsWith("%") && !value.EndsWith("%"))
                    {
                        conditiontoAppend = FlowOperators.doesnotcontain;
                    }
                    else if (!value.StartsWith("%") && value.EndsWith("%"))
                    {
                        conditiontoAppend = FlowOperators.doesnotstartWith;
                    }
                    else if (value.StartsWith("%") && !value.EndsWith("%"))
                    {
                        conditiontoAppend = FlowOperators.doesnotendwith;
                    }
                    value = value.Replace("%", "");
                    break;
                case "on":
                    conditiontoAppend = FlowOperators.equals;
                    //remove time part from value
                    value = getDatePartFromString(value);
                    break;
                case "not-on":
                    conditiontoAppend = FlowOperators.doesnotequal;
                    //remove time part from value
                    value = getDatePartFromString(value);
                    break;
                case "on-or-after":
                    conditiontoAppend = FlowOperators.greaterthanOrEqual;
                    //remove time part from value
                    value = getDatePartFromString(value);
                    break;
                case "on-or-before":
                    conditiontoAppend = FlowOperators.lessthanOrEqual;
                    //remove time part from value
                    value = getDatePartFromString(value);
                    break;
                default:
                    string error = string.Format("Incompatible condition found {0}", operatorType);
                    throw new Exception(error);
                    break;
            }

            if (!isGuidOrInt) value = string.Format("'{0}'", value);
            return conditiontoAppend.Replace("<attributename>", attributeName).Replace("<value>", value);
        }

        /// <summary>
        /// This event occurs when the default scope radio button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radiobtnDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnDefault.Checked)
            {
                radiobtnCustom.Checked = false;
                txtStep.ReadOnly = true;
            }
            else if (radiobtnCustom.Checked == false) // cannot untick if other option is also not ticked
            {
                radiobtnDefault.Checked = true;
                txtStep.ReadOnly = true;
            }
            
        }

        /// <summary>
        /// This event occurs when the custom scope radio button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radiobtnCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnCustom.Checked)
            {
                radiobtnDefault.Checked = false;
                txtStep.ReadOnly = false;
            }
            else if (radiobtnDefault.Checked == false) // cannot untick if other option is also not ticked
            {
                radiobtnCustom.Checked = true;
                txtStep.ReadOnly = false;
            }
        }

        /// <summary>
        /// This method removes the time part from a date time string
        /// </summary>
        /// <param name="value"></param>
        private string getDatePartFromString(string value)
        {
            try
            {
                //remove time part from value
                DateTime date = Convert.ToDateTime(value).Date;
                value = date.ToShortDateString();
                return value;
            }
            catch (Exception e)
            {
                return value;
            }
        }
    }

    public class HighlightColors
    {
        public static Color HC_NODE = Color.Firebrick;
        public static Color HC_STRING = Color.Blue;
        public static Color HC_ATTRIBUTE = Color.Red;
        public static Color HC_COMMENT = Color.GreenYellow;
        public static Color HC_INNERTEXT = Color.Black;
    }

    public class FlowOperators
    {
        public static string contains = "contains(<attributename>,<value>)";
        public static string doesnotcontain = "not(contains(<attributename>,<value>))";
        public static string equals = "equals(<attributename>,<value>)";
        public static string doesnotequal = "not(equals(<attributename>,<value>))";
        public static string startsWith = "startsWith(<attributename>,<value>)";
        public static string doesnotstartWith = "not(startsWith(<attributename>,<value>))";
        public static string endswith = "endswith(<attributename>,<value>)";
        public static string doesnotendwith = "not(endswith(<attributename>,<value>))";
        public static string greaterthan = "greater(<attributename>,<value>)";
        public static string greaterthanOrEqual = "greaterOrEquals(<attributename>,<value>)";
        public static string lessthan = "less(<attributename>,<value>)";
        public static string lessthanOrEqual = "lessOrEquals(<attributename>,<value>)";
    }

}