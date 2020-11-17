using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using CollectionPortal.Module.BusinessObjects;
using DevExpress.ExpressApp.Model;

namespace CollectionPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SalesOrderDocNosController : ViewController
    {
        public event CustomizePopupWindowParamsEventHandler CustomizePopupWindowParams;

        public SalesOrderDocNosController()
        {
            InitializeComponent();
            //SalesOrderDocNosAction.CustomizePopupWindowParams += SalesOrderDocNosAction_CustomizePopupWindowParams;

            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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
        }
        private void SalesOrderDocNosAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            //Type objectType = typeof(DocumentNumberingScheme);
            //e.View = Application.CreateListView(objectType, true);
            //if (View.CurrentObject == null)
            //    return;
            //if (View.CurrentObject.GetType().Name != "SalesOrder")
            //    return;
            //SalesOrder sorder = (SalesOrder)View.CurrentObject;
            //CriteriaOperator op = null;
            //op = GroupOperator.And(op, new BinaryOperator("Module", "SalesOrder"));
            //if (sorder == null)
            //    return;
            //if (sorder.Location != null)
            //op = GroupOperator.And(op, new BinaryOperator("Location.Name", sorder.Location.Name.ToString()));
            //else
            //    op = GroupOperator.And(op, new BinaryOperator("Location.Name", ""));

            //(e.View.Model as IModelListView).Filter = op.ToString();
            //e.View.ControlsCreated += new EventHandler(View_ControlsCreated);
        }

        void View_ControlsCreated(object sender, EventArgs e)
        {
            //SalesOrder sorder = (SalesOrder)View.CurrentObject;
            //CriteriaOperator op = null;
            //op = GroupOperator.And(op, new BinaryOperator("Module", "SalesOrder"));
            //if (sorder.Location != null)
            //    op = GroupOperator.And(op, new BinaryOperator("Location.Name", sorder.Location.Name.ToString()));
            //else
            //    op = GroupOperator.And(op, new BinaryOperator("Location.Name", ""));

            //((sender as ListView).Model as IModelListView).Filter = op.ToString();
        }

        private void SalesOrderDocNosAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs args)
        {
            
            //SalesOrder salesorder = (SalesOrder)View.CurrentObject;
            //foreach (DocumentNumberingScheme docnos in args.PopupWindowViewSelectedObjects)
            //{
            //    if (!string.IsNullOrEmpty(salesorder.DocumentNumber))
            //    {
            //        salesorder.DocumentNumber ="";
            //    }
            //    if (docnos.Automatic == true)
            //    {
            //        salesorder.DocumentNumber = docnos.Prefix + new String('X', docnos.Body) + docnos.Suffix;
            //        salesorder.DocSchemeOid = docnos.Oid;
            //        salesorder.IsNew = true;
            //    }
            //}
            //if (((DetailView)View).ViewEditMode == ViewEditMode.View)
            //{
            //    View.ObjectSpace.CommitChanges();
            //}

            //(args.PopupWindow.View.Model as IModelListView).Filter = "";
        }
    }
}
