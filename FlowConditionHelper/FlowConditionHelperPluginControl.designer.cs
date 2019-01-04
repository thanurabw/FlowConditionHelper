namespace FlowConditionHelper
{
    partial class FlowConditionHelperPluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblFetch = new System.Windows.Forms.Label();
            this.lblCondition = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.richtxtFetchXml = new System.Windows.Forms.RichTextBox();
            this.richtxtCondition = new System.Windows.Forms.RichTextBox();
            this.radiobtnDefault = new System.Windows.Forms.RadioButton();
            this.lblScope = new System.Windows.Forms.Label();
            this.radiobtnCustom = new System.Windows.Forms.RadioButton();
            this.txtStep = new System.Windows.Forms.TextBox();
            this.toolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1019, 25);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 22);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // lblFetch
            // 
            this.lblFetch.AutoSize = true;
            this.lblFetch.Location = new System.Drawing.Point(12, 63);
            this.lblFetch.Name = "lblFetch";
            this.lblFetch.Size = new System.Drawing.Size(56, 13);
            this.lblFetch.TabIndex = 8;
            this.lblFetch.Text = "FetchXML";
            // 
            // lblCondition
            // 
            this.lblCondition.AutoSize = true;
            this.lblCondition.Location = new System.Drawing.Point(509, 63);
            this.lblCondition.Name = "lblCondition";
            this.lblCondition.Size = new System.Drawing.Size(51, 13);
            this.lblCondition.TabIndex = 9;
            this.lblCondition.Text = "Condition";
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(15, 28);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 10;
            this.btnConvert.Text = "Convert ->";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // richtxtFetchXml
            // 
            this.richtxtFetchXml.Location = new System.Drawing.Point(15, 82);
            this.richtxtFetchXml.Name = "richtxtFetchXml";
            this.richtxtFetchXml.Size = new System.Drawing.Size(491, 455);
            this.richtxtFetchXml.TabIndex = 11;
            this.richtxtFetchXml.Text = "";
            // 
            // richtxtCondition
            // 
            this.richtxtCondition.Location = new System.Drawing.Point(512, 82);
            this.richtxtCondition.Name = "richtxtCondition";
            this.richtxtCondition.Size = new System.Drawing.Size(491, 455);
            this.richtxtCondition.TabIndex = 12;
            this.richtxtCondition.Text = "";
            // 
            // radiobtnDefault
            // 
            this.radiobtnDefault.AutoSize = true;
            this.radiobtnDefault.Checked = true;
            this.radiobtnDefault.Location = new System.Drawing.Point(512, 34);
            this.radiobtnDefault.Name = "radiobtnDefault";
            this.radiobtnDefault.Size = new System.Drawing.Size(125, 17);
            this.radiobtnDefault.TabIndex = 13;
            this.radiobtnDefault.TabStop = true;
            this.radiobtnDefault.Text = "Default (TriggerBody)";
            this.radiobtnDefault.UseVisualStyleBackColor = true;
            this.radiobtnDefault.CheckedChanged += new System.EventHandler(this.radiobtnDefault_CheckedChanged);
            // 
            // lblScope
            // 
            this.lblScope.AutoSize = true;
            this.lblScope.Location = new System.Drawing.Point(509, 12);
            this.lblScope.Name = "lblScope";
            this.lblScope.Size = new System.Drawing.Size(237, 13);
            this.lblScope.TabIndex = 14;
            this.lblScope.Text = "Record scope (Flow step to apply conditions on):";
            // 
            // radiobtnCustom
            // 
            this.radiobtnCustom.AutoSize = true;
            this.radiobtnCustom.Location = new System.Drawing.Point(655, 35);
            this.radiobtnCustom.Name = "radiobtnCustom";
            this.radiobtnCustom.Size = new System.Drawing.Size(85, 17);
            this.radiobtnCustom.TabIndex = 15;
            this.radiobtnCustom.Text = "Custom Step";
            this.radiobtnCustom.UseVisualStyleBackColor = true;
            this.radiobtnCustom.CheckedChanged += new System.EventHandler(this.radiobtnCustom_CheckedChanged);
            // 
            // txtStep
            // 
            this.txtStep.Location = new System.Drawing.Point(746, 34);
            this.txtStep.Name = "txtStep";
            this.txtStep.ReadOnly = true;
            this.txtStep.Size = new System.Drawing.Size(257, 20);
            this.txtStep.TabIndex = 16;
            this.txtStep.Text = "body(\'step_name_goes_here\')";
            // 
            // FlowConditionHelperPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtStep);
            this.Controls.Add(this.radiobtnCustom);
            this.Controls.Add(this.lblScope);
            this.Controls.Add(this.radiobtnDefault);
            this.Controls.Add(this.richtxtCondition);
            this.Controls.Add(this.richtxtFetchXml);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.lblCondition);
            this.Controls.Add(this.lblFetch);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "FlowConditionHelperPluginControl";
            this.Size = new System.Drawing.Size(1019, 603);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.Label lblFetch;
        private System.Windows.Forms.Label lblCondition;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.RichTextBox richtxtFetchXml;
        private System.Windows.Forms.RichTextBox richtxtCondition;
        private System.Windows.Forms.RadioButton radiobtnDefault;
        private System.Windows.Forms.Label lblScope;
        private System.Windows.Forms.RadioButton radiobtnCustom;
        private System.Windows.Forms.TextBox txtStep;
    }
}
