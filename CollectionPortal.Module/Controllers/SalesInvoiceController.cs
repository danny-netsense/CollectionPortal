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
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace CollectionPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SalesInvoiceController : ViewController
    {
        public SalesInvoiceController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.Committing += OnViewObjectSpaceCommitting;
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            View.ObjectSpace.Committing -= OnViewObjectSpaceCommitting;
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void OnViewObjectSpaceCommitting(object sender, EventArgs e)
        {
            if (View.Id == "SalesInvoice_DetailView")
            {
                SalesInvoice sinvoice = (SalesInvoice)View.CurrentObject;
                if (sinvoice.DocumentStatus == DocumentStatus.CreditLimitExceeded)
                {
                    string sqlquery = "";
                    sqlquery = " select ";
                    sqlquery += " [CreditDaysExceededInvoiceNo]=case when CR_Control ='B' or (CR_Control='S' and isnull((select sy_cbcontrol from einstall),'')='B') then ";
                    sqlquery += " isnull((select top 1 b.bill_no from bill b where b.custcode=c.custcode and b.due_date<'" + sinvoice.DocumentDate.ToString("yyyyMMdd") + "'";
                    sqlquery += " and (b.net_amt-isnull((select sum(ca.amount) from customer_adj ca ";
                    sqlquery += " where ca.debit_no=b.bill_no and ca.doc_date<='" + sinvoice.DocumentDate.ToString("yyyyMMdd") + "' and ca.db_type='SB'),0))>0 order by bill_date desc),'') else '' end ";
                    sqlquery += " from Customer c where custcode='" + sinvoice.Customer.ERPCode + "'";

                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(sinvoice.Company.ServerName, sinvoice.Company.ERPMasterDB + sinvoice.Company.Initials), AutoCreateOption.None);
                    //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", sinvoice.Company.ERPMasterDB + sinvoice.Company.Initials), AutoCreateOption.None);
                    XpoDefault.Session = sinvoice.Session;

                    using (var uow = new UnitOfWork())
                    {
                        SelectedData sd = uow.ExecuteQuery(sqlquery);
                        foreach (var item in sd.ResultSet)
                        {
                            if (item.Rows[0].Values[0].ToString().Trim() != "")
                            {
                                string errorMessage = string.Empty;
                                errorMessage = "This invoice number " + item.Rows[0].Values[0].ToString().Trim() + " has been exceeded the Credit Days, Cannot Proceed!";
                                sinvoice.DocumentStatus = DocumentStatus.CreditLimitExceeded;

                                MessageOptions options = new MessageOptions();
                                options.Duration = 2000;
                                options.Message = string.Format(errorMessage);
                                options.Type = InformationType.Error;
                                options.Web.Position = InformationPosition.Right;
                                options.Win.Caption = "Error";
                                options.Win.Type = WinMessageType.Toast;
                                Application.ShowViewStrategy.ShowMessage(options);
                                throw new UserFriendlyException(errorMessage);

                                //return;
                            }
                        }
                    }


                }
            }
            
        }
    }
}
