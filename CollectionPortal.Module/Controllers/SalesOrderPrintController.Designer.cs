namespace CollectionPortal.Module.Controllers
{
    partial class SalesOrderPrintController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.PrintAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // PrintAction
            // 
            this.PrintAction.Caption = "Print";
            this.PrintAction.Category = "Edit";
            this.PrintAction.ConfirmationMessage = null;
            this.PrintAction.Id = "42dd66de-0b1a-4f14-b8b9-c0fa72b0f100";
            this.PrintAction.TargetObjectType = typeof(CollectionPortal.Module.BusinessObjects.SalesOrder);
            this.PrintAction.ToolTip = null;
            this.PrintAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintAction_Execute);
            // 
            // SalesOrderPrintController
            // 
            this.Actions.Add(this.PrintAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction PrintAction;
    }
}
