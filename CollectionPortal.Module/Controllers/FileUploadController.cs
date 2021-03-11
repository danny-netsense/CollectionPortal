using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;

namespace CollectionPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class FileUploadController : ViewController<DetailView>
    {
        FileDataPropertyEditor propertyEditor;
        public FileUploadController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            propertyEditor = View.FindItem("Attachment") as FileDataPropertyEditor;
            if (propertyEditor != null)
                propertyEditor.ControlCreated += propertyEditor_ControlCreated;
            // Perform various tasks depending on the target View.
        }

        private void propertyEditor_ControlCreated(object sender, EventArgs e)
        {
            FileDataEdit control = ((FileDataPropertyEditor)sender).Editor;
            if (control != null)
                control.UploadControlCreated += control_UploadControlCreated;
        }
        private void control_UploadControlCreated(object sender, EventArgs e)
        {
            ASPxUploadControl uploadControl = ((FileDataEdit)sender).UploadControl;
            uploadControl.ValidationSettings.AllowedFileExtensions = new String[] { ".zip" };
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            if (propertyEditor != null)
                propertyEditor.ControlCreated -= propertyEditor_ControlCreated;
        }
    }
}
