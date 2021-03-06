﻿using System;
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
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Xpo.DB;

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    [DefaultProperty("DocumentNumber")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class SalesInvoice : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SalesInvoice(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _SalesInvoiceID = (Session.Evaluate<SalesInvoice>(CriteriaOperator.Parse("Max(SalesInvoiceID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<SalesInvoice>(CriteriaOperator.Parse("Max(SalesInvoiceID)"), CriteriaOperator.Parse("")))) + 1;
            if (SecuritySystem.CurrentUserId != null)
            {
                fOwner = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
                XPCollection<Employee> emp = new XPCollection<Employee>(Session);
                if (fOwner.UserName != null)
                {

                    emp.Criteria = CriteriaOperator.Parse("UserName='" + fOwner.UserName.ToString() + "'");
                    foreach (var item in emp)
                    {
                        Employee = item;
                        foreach (Location loc in Employee.Locations)
                        {
                            Location = loc;
                            break;
                        }
                        Company = item.Company;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Please provide atleast users view rights to create a document!");
                }
                DocumentDate = DateTime.Today;
                CreatedDateTime = DateTime.Now;
                Agent = Employee.Agent;
            }
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        private XPCollection<Customer> customerDataSource;
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (customerDataSource != null)
            {
                customerDataSource.Dispose();
                customerDataSource = null;
            }
        }
        [Persistent("SalesInvoiceID")] // this line for read-only columns mapping
        private int _SalesInvoiceID;
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique va lue
        [PersistentAlias("_SalesInvoiceID")] // This line for read-only column mapping
        [Browsable(false)]
        public int SalesInvoiceID
        {
            get { return _SalesInvoiceID; }
        }


        private Location fLocation;
        [DataSourceProperty("Employee.Locations")]
        [RuleRequiredField(DefaultContexts.Save)] // Validation for Required
        [ImmediatePostData(true)]
        [ModelDefault("LookupProperty", "LocationLookupDisplayText")]
        public Location Location
        {
            get
            {
                return fLocation;
            }
            set
            {
                SetPropertyValue(nameof(Location), ref fLocation, value);
                if (!IsLoading && !IsSaving && fLocation != null)
                    GetDocumentNumbering();
            }
        }

        Guid fDocSchemeOid;
        [Browsable(false)]
        public Guid DocSchemeOid
        {
            get { return fDocSchemeOid; }
            set { SetPropertyValue<Guid>("DocSchemeOid", ref fDocSchemeOid, value); }
        }

        bool fIsNew;
        [Browsable(false)]
        [DefaultValue(false)]
        public bool IsNew
        {
            get { return fIsNew; }
            set { SetPropertyValue<bool>("IsNew", ref fIsNew, value); }
        }


        string fDocumentNumber;
        [RuleRequiredField(DefaultContexts.Save)] // Validation for Required
        [Size(20)]
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        DateTime fDocumentDate;
        [RuleRequiredField(DefaultContexts.Save)] // Validation for Required
        [Appearance("DocumentNoDateCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public DateTime DocumentDate
        {
            get { return fDocumentDate; }
            set { SetPropertyValue<DateTime>("DocumentDate", ref fDocumentDate, value); }
        }

        private Customer fCustomer;
        [RuleRequiredField(DefaultContexts.Save)] // Validation for Required
        [Appearance("DocumentNoCuGetCustomerDatastomerCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        [DataSourceProperty("CustomerDataSource")]
        [ImmediatePostData(true)]

        public Customer Customer
        {
            get
            {
                return fCustomer;
            }
            set
            {
                SetPropertyValue(nameof(Customer), ref fCustomer, value);
                if (!IsLoading && !IsSaving && fCustomer != null)
                    GetCustomerData();
            }
        }

        decimal fOutstandingAmount;
        [Appearance("DocumentNoOutstandingAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public decimal OutstandingAmount
        {
            get { return fOutstandingAmount; }
            set { SetPropertyValue<decimal>("OutstandingAmount", ref fOutstandingAmount, value); }
        }

        decimal fOutstandingOrder;
        [Appearance("DocumentNoOutstandingOrderCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public decimal OutstandingOrder
        {
            get { return fOutstandingOrder; }
            set { SetPropertyValue<decimal>("OutstandingOrder", ref fOutstandingOrder, value); }
        }

        int fCreditDays;
        [Appearance("DocumentNoCreditDaysCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public int CreditDays
        {
            get { return fCreditDays; }
            set { SetPropertyValue<int>("CreditDays", ref fCreditDays, value); }
        }

        int fEnricoCreditDays;
        [Appearance("DocumentNoEnricoCreditDaysCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public int EnricoCreditDays
        {
            get { return fEnricoCreditDays; }
            set { SetPropertyValue<int>("EnricoCreditDays", ref fEnricoCreditDays, value); }
        }

        decimal fCreditLimit;
        [Appearance("DocumentNoCreditLimitCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public decimal CreditLimit
        {
            get { return fCreditLimit; }
            set { SetPropertyValue<decimal>("CreditLimit", ref fCreditLimit, value); }
        }


        private Agent fAgent;
        [Appearance("DocumentNoAgentCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        [DataSourceCriteria("Oid = '@this.Employee.Agent.Oid'")]
        [ImmediatePostData(true)]
        public Agent Agent
        {
            get
            {
                return fAgent;

            }
            set
            {
                SetPropertyValue(nameof(Agent), ref fAgent, value);
                if (!IsLoading && (customerDataSource != null))
                {
                    customerDataSource.Criteria = new BinaryOperator("Agent", fAgent);
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public IList<Customer> CustomerDataSource
        {
            get
            {
                if (customerDataSource == null)
                {
                    //customerDataSource = new XPCollection<Customer>(Session, new BinaryOperator("Agent", fAgent));
                    customerDataSource = fAgent.Customers;
                    //RefreshCustomer();
                }
                return customerDataSource;
            }
        }

        private void RefreshCustomer()
        {
            if (customerDataSource == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Agent == null)
            {
                // Show only Global Collection when the Party is not specified
                //fAvailableBookingOrders.Criteria = CriteriaOperator.Parse("1=1");
            }
            else
            {
                // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                customerDataSource.Criteria = ContainsOperator.Parse("Agent = ?", Agent);
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source
            Customer = null;
        }

        private Currency fCurrency;
        [Appearance("DocumentNoCurrencyCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public Currency Currency
        {
            get
            {
                return fCurrency;
            }
            set
            {
                SetPropertyValue(nameof(Currency), ref fCurrency, value);
            }
        }

        decimal fCurrencyRate;
        [Appearance("DocumentNoCurrencyRateCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        [Appearance("CurrencyCurrencyRateCond", Enabled = false, Criteria = "IsNullOrEmpty(Currency)", Context = "DetailView")]
        public decimal CurrencyRate
        {
            get { return fCurrencyRate; }
            set { SetPropertyValue<decimal>("CurrencyRate", ref fCurrencyRate, value); }
        }

        decimal fTotalAmount;
        [Appearance("DocumentNoTotalAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public decimal TotalAmount
        {
            get
            {
                if (!IsLoading && !IsSaving && fTotalAmount == null)
                    UpdateTotalAmount(false);
                return fTotalAmount;
            }
            set { SetPropertyValue<decimal>("TotalAmount", ref fTotalAmount, value); }
        }

        string fRemarks;
        [Size(2000)]
        [Appearance("DocumentNoRemarksCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public string Remarks
        {
            get { return fRemarks; }
            set { SetPropertyValue<string>("Remarks", ref fRemarks, value); }
        }

        FileData fAttachment;
        [Appearance("DocumentNoAttachmentCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public FileData Attachment
        {
            get { return fAttachment; }
            set { SetPropertyValue<FileData>("Attachment", ref fAttachment, value); }
        }

        DocumentStatus fDocumentStatus;
        [Appearance("DocumentNoDocumentStatusCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        public DocumentStatus DocumentStatus
        {
            get { return fDocumentStatus; }
            set { SetPropertyValue<DocumentStatus>("DocumentStatus", ref fDocumentStatus, value); }
        }

        private Company fCompany;
        [Browsable(false)]
        public Company Company
        {
            get
            {
                return fCompany;
            }
            set
            {
                SetPropertyValue(nameof(Company), ref fCompany, value);
            }
        }

        private PermissionPolicyUser fOwner;
        [Browsable(false)]
        public PermissionPolicyUser Owner
        {
            get
            {
                return fOwner;
            }
            set
            {
                SetPropertyValue(nameof(PermissionPolicyUser), ref fOwner, value);
            }
        }

        private Employee fEmployee;
        [Browsable(false)]
        public Employee Employee
        {
            get
            {
                return fEmployee;
            }
            set
            {
                SetPropertyValue(nameof(Employee), ref fEmployee, value);
            }
        }

        private DateTime fCreatedDateTime;
        [Browsable(false)]
        public DateTime CreatedDateTime
        {
            get
            {
                return fCreatedDateTime;
            }
            set
            {
                SetPropertyValue<DateTime>("CreatedDateTime", ref fCreatedDateTime, value);
            }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [RuleRequiredField(DefaultContexts.Save)] // Validation for Required
        [Appearance("DocumentNoSalesInvoiceDetailsCond", Enabled = false, Criteria = "IsNullOrEmpty(DocumentNumber)", Context = "DetailView")]
        [Appearance("CustomerSalesInvoiceDetailsCond", Enabled = false, Criteria = "IsNullOrEmpty(Customer)", Context = "DetailView")]
        [Appearance("DocumentStatusSalesInvoiceDetailsCond", Enabled = false, Criteria = "DocumentStatus=3", Context = "DetailView")]

        public XPCollection<SalesInvoiceDetail> SalesInvoiceDetails
        {
            get
            {

                return GetCollection<SalesInvoiceDetail>(nameof(SalesInvoiceDetails));
            }
        }

        private XPCollection<AuditDataItemPersistent> auditTrail;
        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public XPCollection<AuditDataItemPersistent> AuditTrail
        {
            get
            {
                if (auditTrail == null)
                {
                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return auditTrail;
            }
        }

        private void GetDocumentNumbering()
        {
            if (Location != null)
            {
                XPCollection<DocumentNumberingScheme> docnos = new XPCollection<DocumentNumberingScheme>(Session);
                docnos.Criteria = CriteriaOperator.Parse("[Location]=? and [Module]=?", Location, Modules.SalesInvoice);
                if (docnos.Count == 1)
                {
                    foreach (DocumentNumberingScheme docno in docnos)
                    {
                        DocumentNumber = docno.Prefix + new String('X', docno.Body) + docno.Suffix;
                        DocSchemeOid = docno.Oid;
                        IsNew = true;
                    }
                }
            }
        }

        private void GetCustomerData()
        {
            DocumentStatus = DocumentStatus.Approved;
            string errorMessage = string.Empty;
            //try
            //{
            string sqlquery = "";
            sqlquery = " select balance=isnull((select sum(net_amt) from bill where custcode is not null AND custcode=C.Custcode and bill_no<>'" + DocumentNumber + "'),0) ";
            sqlquery += " +isnull((select sum(net_amt) from rphead where extype='P' and custcode is not null  AND custcode=C.Custcode ),0) ";
            sqlquery += " +isnull((select sum(net_amt) from rphead where extype='R' and custcode is not null  AND custcode=C.Custcode),0)*-1 ";
            sqlquery += " +isnull((select sum(net_amt) from DBNOTE where custcode is not null AND custcode=C.Custcode),0) ";
            sqlquery += " +isnull((select sum(net_amt) from CRNOTE where custcode is not null  AND custcode=C.Custcode),0)*-1 ";
            sqlquery += " +isnull((select sum(net_amt) from CHEQRET where custcode is not null  AND custcode=C.Custcode),0) ";
            sqlquery += " +isnull((select sum(GAINLOSS) from GAINLOSS where custcode is not null  AND custcode=C.Custcode),0) , ";
            sqlquery += " OutstandingOrder=Isnull((select isnull(sum(d.bal_qty*d.rate),0) ";
            sqlquery += " from sorddet d inner join sorder h on d.doc_no=h.doc_no where h.AMENDMENT=0 and h.custcode=c.Custcode and h.doc_no<>'" + DocumentNumber + "'),0), ";
            sqlquery += " c.CREDIT_DAYS,isnull((select credit_lt from credit_list CL where cl.lst_rec='L' and cl.custcode=c.custcode ),c.Credit_lt) ";
            sqlquery += " ,[CreditDaysExceededInvoiceNo]=case when CR_Control ='B' or (CR_Control='S' and isnull((select sy_cbcontrol from einstall),'')='B') then ";
            sqlquery += " isnull((select top 1 b.bill_no from bill b where b.custcode=c.custcode and b.due_date<'" + this.DocumentDate.ToString("yyyyMMdd") + "'";
            sqlquery += " and (b.net_amt-isnull((select sum(ca.amount) from customer_adj ca ";
            sqlquery += " where ca.debit_no=b.bill_no and ca.doc_date<='" + this.DocumentDate.ToString("yyyyMMdd") + "' and ca.db_type='SB'),0))>0 order by bill_date desc),'') else '' end ";
            sqlquery += " ,cast(dbo.udf_GetNumeric(OLD_ID_NO) as int) ";
            sqlquery += " from Customer c where custcode='" + Customer.ERPCode + "'";
            //Initialize your data layer.
            //By default if you don't do this, XPO will try and use an access databse (jet)
            Session session = new Session();
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(Company.ServerName, Company.ERPMasterDB + Company.Initials), AutoCreateOption.None);
            //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", Company.ERPMasterDB + Company.Initials), AutoCreateOption.None);
            XpoDefault.Session = session;

            //Equivalent of SELECT * FROM TableName in SQL
            // YourClassName would be your XPO object (your persistent object)
            using (var uow = new UnitOfWork())
            {
                SelectedData sd = uow.ExecuteQuery(sqlquery);


                foreach (var item in sd.ResultSet)
                {
                    OutstandingAmount = Convert.ToDecimal(item.Rows[0].Values[0]);
                    OutstandingOrder = Convert.ToDecimal(item.Rows[0].Values[1]);
                    CreditDays = Convert.ToInt32(item.Rows[0].Values[2]);
                    EnricoCreditDays = Convert.ToInt32(item.Rows[0].Values[5]);
                    CreditLimit = Convert.ToInt32(item.Rows[0].Values[3]);
                    if (item.Rows[0].Values[4].ToString().Trim() != "")
                    {
                        //isError = true;
                        errorMessage = "This invoice number " + item.Rows[0].Values[4].ToString().Trim() + " has been exceeded the Credit Days, Cannot Proceed!";
                        DocumentStatus = DocumentStatus.CreditLimitExceeded;
                        //return;
                    }
                }
            }

            //RefreshAgent();
            //Agent = Customer.Agent;
            //}
            //catch (Exception ex) { }
            //if (isError)
            //    throw new UserFriendlyException(errorMessage);
        }


        public void UpdateTotalAmount(bool forceChangeEvents)
        {
            decimal? oldTotalAmount = fTotalAmount;
            decimal tempTotalAmount = 0m;
            foreach (SalesInvoiceDetail detail in SalesInvoiceDetails)
                tempTotalAmount += detail.GrossAmount;
            fTotalAmount = tempTotalAmount;
            if (forceChangeEvents)
                OnChanged(nameof(TotalAmount), oldTotalAmount, fTotalAmount);
        }

        protected override void OnSaving()
        {
            if (!(this.Session is NestedUnitOfWork) && Session.IsNewObject(this))
            {


                int newnum = 1;
                DocumentNumberingScheme docnos = Session.GetObjectByKey<DocumentNumberingScheme>(this.DocSchemeOid);
                if (this.IsNew == true)
                {
                    newnum = docnos.CurrentNo + 1;
                    this.DocumentNumber = docnos.Prefix + new string('0', docnos.Body - (newnum).ToString().Length) + (newnum).ToString() + docnos.Suffix;
                }
                base.OnSaving();
                if (this.IsNew == true)
                {
                    docnos.CurrentNo = newnum;
                    docnos.Save();

                    //=======================Insertion of ERP =================================
                    //----------Header Section Insertion --------------------------------------
                    string insertHeaderSQL = string.Empty;
                    string insertDetailSQL = string.Empty;
                    string insertPTermsSQL = string.Empty;
                    insertHeaderSQL = "INSERT INTO BILL (agentcode,BILL_NO,sy_obj_name,sy_docno,BILL_DATE";
                    insertHeaderSQL += ",CUSTCODE,REMARKS,NET_AMT,CURR_VAL,";
                    insertHeaderSQL += "BAL_AMT,BAL_CURR,CURR_RATE,BASIC,";
                    insertHeaderSQL += " EXUSER,SQLUSER,APPROVED_AMT,ADDRESS1,FROM_MOD,Br_init,valid_day)";
                    insertHeaderSQL += " values ( ";
                    insertHeaderSQL += " '" + this.Agent.Code + "','" + this.DocumentNumber + "','SAL_INV',0,'" + this.DocumentDate.ToString("yyyyMMdd") + "'";
                    insertHeaderSQL += " ,'" + this.Customer.ERPCode + "','" + this.Remarks + "'," + this.TotalAmount.ToString() + "," + this.TotalAmount.ToString();
                    insertHeaderSQL += " ," + this.TotalAmount.ToString() + "," + this.TotalAmount.ToString() + ",1," + this.TotalAmount.ToString();
                    insertHeaderSQL += " , '" + this.Employee.UserName + "','dbo'," + this.TotalAmount + ",'" + this.Customer.Address + "'";
                    insertHeaderSQL += " , 'INV','" + this.Location.ERPCode + "','365'";
                    insertHeaderSQL += " )";
                    //---------- End Header Section Insertion ----------------------------------

                    //---------- Detail Section Insertion --------------------------------------

                    int slno = 0;
                    Guid unique_number;
                    foreach (SalesInvoiceDetail sod in this.SalesInvoiceDetails)
                    {
                        slno += 1;
                        unique_number = Guid.NewGuid();

                        insertDetailSQL += " insert into BILLDET";
                        insertDetailSQL += " (agentcode,sy_obj_name, sy_docno,  BILL_NO, BILL_DATE,  ";
                        insertDetailSQL += " REMARKS,  PRICE, MRP, TOT_AMT,";
                        insertDetailSQL += " PRODCODE,QUANTITY, BAL_QTY,";
                        insertDetailSQL += " QUANTITY2, BAL_QTY2, CONV_FAC, ";
                        insertDetailSQL += " CONV_BASIS, br_init, CUSTCODE, TERM_AMT,  AMOUNT, BAL_AMT, ";
                        insertDetailSQL += " slno,FROM_MOD,unique_number,godowncode) ";
                        insertDetailSQL += " values (";
                        insertDetailSQL += " '" + this.Agent.Code + "','SAL_INV',0,'" + this.DocumentNumber + "','" + this.DocumentDate.ToString("yyyyMMdd") + "'";
                        insertDetailSQL += " ,'" + sod.AdditionalDescription.Replace("'", "''") + "'," + sod.Price.ToString() + "," + sod.Price.ToString() + "," + sod.GrossAmount.ToString();
                        insertDetailSQL += " ,'" + sod.Product.Code + "'," + sod.Quantity.ToString() + "," + sod.Quantity + ",";
                        insertDetailSQL += " " + sod.Quantity2.ToString() + "," + sod.Quantity2.ToString() + "," + sod.ConvFac.ToString() + ",";
                        insertDetailSQL += " 'M', '" + this.Location.ERPCode + "','" + this.Customer.ERPCode + "'," + sod.DiscountAmount.ToString() + "," + sod.ProductAmount.ToString() + "," + sod.GrossAmount.ToString();
                        if (sod.Warehouse == null)
                            insertDetailSQL += " ," + slno.ToString() + ",'INV','" + unique_number.ToString() + "','null'";
                        else
                            insertDetailSQL += " ," + slno.ToString() + ",'INV','" + unique_number.ToString() + "','" + sod.Warehouse.Code.ToString() + "'";
                        insertDetailSQL += " )";
                        //insertDetailSQL += Environment.NewLine + " Go ";
                        insertDetailSQL += Environment.NewLine;

                        if (sod.DiscountAmount != 0)
                        {
                            insertPTermsSQL += "INSERT INTO SBIPTERM(BILL_NO, TERMS_CODE, Q_V, SLNO, EXPERCENT, ";
                            insertPTermsSQL += " TERM_AMT, BR_INIT, UNIQUE_NUMBER, CHK_APP, AMOUNT_ON, SY_OBJ_NAME, SY_DOCNO,";
                            insertPTermsSQL += " DOC_DATE, SERVICE, CUSTCODE, AGENTCODE)";
                            insertPTermsSQL += " values (";
                            if (sod.DiscountType == DiscountType.Percentage)
                                insertPTermsSQL += " '" + this.DocumentNumber + "', '38','V','" + slno.ToString() + "', " + sod.DiscountPercentageOrAmount;
                            else
                                insertPTermsSQL += " '" + this.DocumentNumber + "', '38','V','" + slno.ToString() + "', " + Math.Round((sod.DiscountPercentageOrAmount * 100) / sod.ProductAmount, 2);
                            insertPTermsSQL += " ," + sod.DiscountAmount.ToString() + ",'" + this.Location.ERPCode + "', '" + unique_number.ToString() + "', 1, " + sod.ProductAmount.ToString() + ",'SAL_ORDER',0";
                            insertPTermsSQL += " ,'" + this.DocumentDate.ToString("yyyyMMdd") + "', 0,'" + this.Customer.ERPCode + "','" + this.Agent.Code + "'";
                            insertPTermsSQL += " ) ";
                            insertDetailSQL += Environment.NewLine;
                        }
                    }
                    //---------- End Detail Section Insertion ----------------------------------


                    if (this.DocumentStatus == DocumentStatus.Approved)
                    {
                        Session session = new Session();
                        XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(Company.ServerName, Company.ERPMasterDB + Company.Initials), AutoCreateOption.None);
                        //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", Company.ERPMasterDB + Company.Initials), AutoCreateOption.None);
                        XpoDefault.Session = session;

                        //Equivalent of SELECT * FROM TableName in SQL
                        // YourClassName would be your XPO object (your persistent object)
                        using (var uow = new UnitOfWork())
                        {
                            if (insertHeaderSQL.ToString().Trim() != "")
                            {
                                try
                                {
                                    uow.ExecuteNonQuery(insertHeaderSQL.ToString());
                                    uow.ExecuteNonQuery(insertDetailSQL.ToString());
                                    if (insertPTermsSQL != string.Empty)
                                        uow.ExecuteNonQuery(insertPTermsSQL.ToString());
                                }
                                catch (Exception ex) { throw new UserFriendlyException(ex.Message); }
                            }
                        }
                    }
                    //=======================End Insertion of ERP =============================


                    this.IsNew = false;
                    this.Save();
                }
            }


        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue(nameof(PersistentProperty), ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}
    }
}