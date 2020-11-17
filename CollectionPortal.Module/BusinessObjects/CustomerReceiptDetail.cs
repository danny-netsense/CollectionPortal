using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Xpo.DB;

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class CustomerReceiptDetail : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public CustomerReceiptDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _CustomerReceiptDetailID = (Session.Evaluate<CustomerReceiptDetail>(CriteriaOperator.Parse("Max(CustomerReceiptDetailID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<CustomerReceiptDetail>(CriteriaOperator.Parse("Max(CustomerReceiptDetailID)"), CriteriaOperator.Parse("")))) + 1;
            //GetInvoiceList();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Persistent("CustomerReceiptDetailID")] // this line for read-only columns mapping
        private int _CustomerReceiptDetailID;
        //[RuleRequiredField] // Validation for Required
        //[RuleUniqueValue] // Validation for unique value
        [Browsable(false)]

        [PersistentAlias("_CustomerReceiptDetailID")] // This line for read-only column mapping
        public int CustomerReceiptDetailID
        {
            get { return _CustomerReceiptDetailID; }
        }

        private ReceiptDocumentType fType;
        public ReceiptDocumentType Type
        {
            get
            {
                return fType;
            }
            set
            {
                SetPropertyValue(nameof(ReceiptDocumentType), ref fType, value);
            }
        }
        //private void GetInvoiceList()
        //{
        //    if (fAvailableSalesInvoice == null)
        //        return;
        //    // Process the situation when the Party is not specified (see the Scenario 3 above)
        //    if (this.CustomerReceipt.Location == null)
        //    {
        //        // Show only Global Collection when the Party is not specified
        //        //fAvailableBookingOrders.Criteria = CriteriaOperator.Parse("1=1");
        //    }
        //    else
        //    {

        //        string errorMessage = string.Empty; 
        //        //try
        //        //{
        //        string sqlquery = "";
        //        sqlquery = " select bill_no as 'DocumentNumber', bill_date as 'DocumentDate',  Net_amt as 'TotalAmount', bal_amt from bill b ";
        //        sqlquery += " where bill_date<='"+ this.CustomerReceipt.DocumentDate.ToString("yyyyMMdd")+"' and custcode='"+this.CustomerReceipt.Customer.ERPCode+"' and bal_amt>0";
        //        //Initialize your data layer.
        //        //By default if you don't do this, XPO will try and use an access databse (jet)
        //        Session session = new Session();
        //        XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("DESKTOP-RDO21MB", CustomerReceipt.Company.ERPMasterDB + CustomerReceipt.Company.Initials), AutoCreateOption.None);
        //        //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", this.CustomerReceipt.Company.ERPMasterDB + this.CustomerReceipt.Company.Initials), AutoCreateOption.None);
        //        XpoDefault.Session = session;
        //        BindingList<SalesInvoice> invoices = new BindingList<SalesInvoice>();
        //        //Equivalent of SELECT * FROM TableName in SQL
        //        // YourClassName would be your XPO object (your persistent object)
        //        using (var uow = new UnitOfWork())
        //        {
        //            SelectedData sd = uow.ExecuteQuery(sqlquery);


        //            foreach (var item in sd.ResultSet)
        //            {
        //                //invoices.Add(item);

        //            }
        //        }






        //    }
        //}

        //private List<SalesInvoice> fAvailableSalesInvoice;
        ////[DataSourceProperty(nameof(AvailableSalesInvoice))]
        //[Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        //public List<SalesInvoice> AvailableSalesInvoice
        //{
        //    get
        //    {
        //        if (fAvailableSalesInvoice == null)
        //        {
        //            // Retrieve all Sample objects
        //            fAvailableSalesInvoice = new List<SalesInvoice>();
        //            // Filter the retrieved collection according to current conditions
        //            GetInvoiceList();
        //        }
        //        // Return the filtered collection of Sample objects
        //        return fAvailableSalesInvoice;
        //    }
        //}

        //private SalesInvoice fSalesInvoice;
        //[ImmediatePostData(true)]
        ////[DataSourceProperty(nameof(AvailableSalesInvoice))]
        //[Appearance("ReceiptTypeSalesInvoiceCond", Enabled = false, Criteria = "IsNullOrEmpty(Type)", Context = "DetailView")]
        //[DataSourceCriteria("Customer.Oid == '@this.CustomerReceipt.Customer.Oid'")]
        //public SalesInvoice SalesInvoice
        //{
        //    get
        //    {
        //        return fSalesInvoice;
        //    }
        //    set
        //    {
        //        SetPropertyValue(nameof(Customer), ref fSalesInvoice, value);
        //        if (!IsLoading && !IsSaving && fSalesInvoice != null)
        //            GetInvoiceData();
        //    }
        //}


        RefERPInvoice invoice;
        [ImmediatePostData]
        [NonPersistent]
        [DataSourceProperty("RefERPInvoices")]
        public RefERPInvoice SalesInvoice
        {
            get
            {
                return invoice;
            }
            set
            {
                invoice = value;
                InvoiceDate = value.InvoiceDate;
                InvoiceAmount = value.InvoiceAmount;
                BalanceAmount = value.BalanceAmount;
            }
        }
        
        private BindingList<RefERPInvoice> _RefERPInvoice = new BindingList<RefERPInvoice>();
        [Browsable(false)]
        public BindingList<RefERPInvoice> RefERPInvoices
        {
            get
            {
                _RefERPInvoice.Clear();

                string errorMessage = string.Empty;
                //try
                //{
                string sqlquery = "";
                sqlquery = " select bill_no as 'DocumentNumber', bill_date as 'DocumentDate',  Net_amt as 'TotalAmount', bal_amt from bill b ";
                sqlquery += " where bill_date<='" + this.CustomerReceipt.DocumentDate.ToString("yyyyMMdd") + "' and custcode='" + this.CustomerReceipt.Customer.ERPCode + "' and bal_amt>0";
                //Initialize your data layer.
                //By default if you don't do this, XPO will try and use an access databse (jet)
                Session session = new Session();
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(CustomerReceipt.Company.ServerName, CustomerReceipt.Company.ERPMasterDB + CustomerReceipt.Company.Initials), AutoCreateOption.None);
                //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", this.CustomerReceipt.Company.ERPMasterDB + this.CustomerReceipt.Company.Initials), AutoCreateOption.None);
                XpoDefault.Session = session;
                BindingList<SalesInvoice> invoices = new BindingList<SalesInvoice>();
                //Equivalent of SELECT * FROM TableName in SQL
                // YourClassName would be your XPO object (your persistent object)
                using (var uow = new UnitOfWork())
                {
                    SelectedData sd = uow.ExecuteQuery(sqlquery);


                    foreach (var item in sd.ResultSet)
                    {
                        _RefERPInvoice.Add(new RefERPInvoice() { InvoiceNo = item.Rows[0].Values[0].ToString(), InvoiceDate = Convert.ToDateTime(item.Rows[0].Values[1]), InvoiceAmount = Convert.ToDecimal(item.Rows[0].Values[2]), BalanceAmount = Convert.ToDecimal(item.Rows[0].Values[3]) });

                    }
                }

                
                return _RefERPInvoice;
            }
        }

        DateTime fInvoiceDate;
        [Appearance("SalesInvoiceDateCond", Enabled = false, Criteria = "IsNullOrEmpty(SalesInvoice)", Context = "DetailView")]
        public DateTime InvoiceDate
        {
            get { return fInvoiceDate; }
            set { SetPropertyValue<DateTime>("InvoiceDate", ref fInvoiceDate, value); }
        }

        decimal fInvoiceAmount;
        [Appearance("SalesInvoiceInvoiceAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(SalesInvoice)", Context = "DetailView")]
        public decimal InvoiceAmount
        {
            get { return fInvoiceAmount; }
            set { SetPropertyValue<decimal>("InvoiceAmount", ref fInvoiceAmount, value); }
        }

        decimal fBalanceAmount;
        [Appearance("SalesInvoiceBalanceAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(SalesInvoice)", Context = "DetailView")]
        public decimal BalanceAmount
        {
            get { return fBalanceAmount; }
            set { SetPropertyValue<decimal>("BalanceAmount", ref fBalanceAmount, value); }
        }

        decimal fAdjustAmount;
        [RuleRequiredField(DefaultContexts.Save)] // Validation for Required
        [Appearance("ReceiptTypeAdjustAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(Type)", Context = "DetailView")]
        public decimal AdjustAmount
        {
            get { return fAdjustAmount; }
            set
            {
                bool modified = SetPropertyValue(nameof(AdjustAmount), ref fAdjustAmount, value);
                if (!IsLoading && !IsSaving && CustomerReceipt != null && modified)
                {
                    CustomerReceipt.UpdateTotalAmount(true);
                }
            }
        }

        private CustomerReceipt _CustomerReceipt;
        [Association]
        public CustomerReceipt CustomerReceipt
        {
            get { return _CustomerReceipt; }
            set { SetPropertyValue<CustomerReceipt>(nameof(CustomerReceipt), ref _CustomerReceipt, value); }
        }



        private void GetInvoiceData()
        {
            if (this.SalesInvoice != null)
            {
                //this.InvoiceDate = this.SalesInvoice.DocumentDate; 
                //this.BalanceAmount = this.SalesInvoice.TotalAmount;
                //this.InvoiceAmount = this.SalesInvoice.TotalAmount;
            }
        }
    }

    public enum ReceiptDocumentType
    {
        Advance,
        Invoice
    }

    [DomainComponent]
    [DefaultProperty("InvoiceNo")]
    public class RefERPInvoice
    {
        [Key]
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal BalanceAmount { get; set; }
    }
}