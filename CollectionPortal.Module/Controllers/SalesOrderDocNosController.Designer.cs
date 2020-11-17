namespace CollectionPortal.Module.Controllers
{
    partial class SalesOrderDocNosController
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
            this.SalesOrderDocNosAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // SalesOrderDocNosAction
            // 
            this.SalesOrderDocNosAction.AcceptButtonCaption = null;
            this.SalesOrderDocNosAction.CancelButtonCaption = null;
            this.SalesOrderDocNosAction.Caption = "Select Document Numbering Scheme";
            this.SalesOrderDocNosAction.Category = "Edit";
            this.SalesOrderDocNosAction.ConfirmationMessage = null;
            this.SalesOrderDocNosAction.Id = "SalesOrderDocNosAction";
            this.SalesOrderDocNosAction.TargetObjectsCriteria = "";
            this.SalesOrderDocNosAction.TargetObjectType = typeof(CollectionPortal.Module.BusinessObjects.SalesOrder);
            this.SalesOrderDocNosAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.SalesOrderDocNosAction.ToolTip = null;
            this.SalesOrderDocNosAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.SalesOrderDocNosAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.SalesOrderDocNosAction_Execute);
            // 
            // SalesOrderDocNosController
            // 
            this.Actions.Add(this.SalesOrderDocNosAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction SalesOrderDocNosAction;
    }
}
