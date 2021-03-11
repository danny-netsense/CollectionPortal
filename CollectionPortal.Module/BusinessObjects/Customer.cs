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
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ImportData;
using DevExpress.Xpo.DB;

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Customer : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Customer(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _CustomerID = (Session.Evaluate<Customer>(CriteriaOperator.Parse("Max(CustomerID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Customer>(CriteriaOperator.Parse("Max(CustomerID)"), CriteriaOperator.Parse("")))) + 1;
            if (SecuritySystem.CurrentUserId != null)
            {
                fOwner = Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
                XPCollection<Employee> emp = new XPCollection<Employee>(Session);
                if (fOwner.UserName != null)
                {
                    emp.Criteria = CriteriaOperator.Parse("UserName='" + fOwner.UserName.ToString() + "'");
                    foreach (var item in emp)
                    {
                        Company = item.Company;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Please provide atleast users view rights to create a document!");
                }

                CreatedDateTime = DateTime.Now;
            }

            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        [Persistent("CustomerID")] // this line for read-only columns mapping
        private int _CustomerID;
        [RuleRequiredField] // Validation for Required
        //[RuleUniqueValue] // Validation for unique value
        [Browsable(false)]

        [PersistentAlias("_CustomerID")] // This line for read-only column mapping
        public int CustomerID
        {
            get { return _CustomerID; }
        }

        string fCode;
        [Size(10)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        string fName;
        [Size(100)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fAddress;
        [Size(4000)]
        public string Address
        {
            get { return fAddress; }
            set { SetPropertyValue<string>("Address", ref fAddress, value); }
        }

        int fCreditDays;

        public int CreditDays
        {
            get { return fCreditDays; }
            set { SetPropertyValue<int>("CreditDays", ref fCreditDays, value); }
        }

        decimal fCreditLimit;
        public decimal CreditLimit
        {
            get { return fCreditLimit; }
            set { SetPropertyValue<decimal>("CreditLimit", ref fCreditLimit, value); }
        }

        private Agent fAgent;
        [Association]
        public Agent Agent
        {
            get
            {
                return fAgent;
            }
            set
            {
                SetPropertyValue(nameof(Agent), ref fAgent, value);
            }
        }


        string fERPCode;
        [Size(10)]
        public string ERPCode
        {
            get { return fERPCode; }
            set { SetPropertyValue<string>("ERPCode", ref fERPCode, value); }
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

        public static ImportDataDelegate<Customer> CreateCoolCustomerImportDataFromXmlFileDelegate(XafApplication application)
        {
            IObjectSpace os1 = application.CreateObjectSpace();
            SelectedData data = GetData(os1);
            return delegate (IObjectSpace os, object[] args)
            {
                object masterObject = null;
                int index = 0;
                if (args != null)
                {
                    if (args.Length == 2)
                    {
                        masterObject = args[0];
                        index = Convert.ToInt32(args[1]);
                    }
                }
                foreach (var result in data.ResultSet)
                {
                    if (index < result.Rows.Count())
                    {

                        Customer customer = os.CreateObject<Customer>();
                        customer.Name = result.Rows[index].Values[0].ToString();
                        customer.Address = result.Rows[index].Values[1].ToString();
                        customer.Code = result.Rows[index].Values[2].ToString();
                        customer.CreditDays = Convert.ToInt32( result.Rows[index].Values[3]);
                        customer.CreditLimit = Convert.ToDecimal(result.Rows[index].Values[4]);
                        customer.ERPCode = result.Rows[index].Values[2].ToString();
                        return customer;
                    }
                }
                return null;
            };
        }

        private static SelectedData GetData(IObjectSpace objectspace)
        {
            SelectedData sd = null;
            try
            {

                PermissionPolicyUser user;
                Session session = new Session();
                user = objectspace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
                Company cmp = null;
                try
                {
                    Employee em = (Employee)user;
                    cmp = em.Company;
                }
                catch
                {

                }
                if (cmp == null)
                {
                    XPCollection<Employee> emp = new XPCollection<Employee>(session);
                    if (user.UserName != null)
                    {
                        emp.Criteria = CriteriaOperator.Parse("UserName='" + user.UserName.ToString() + "'");
                        foreach (var item in emp)
                        {
                            cmp = item.Company;
                        }
                    }
                }
                if (cmp != null)
                {
                    //Initialize your data layer.
                    //By default if you don't do this, XPO will try and use an access databse (jet)

                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(cmp.ServerName, cmp.ERPMasterDB + cmp.Initials), AutoCreateOption.None);
                    //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", cmp.ERPMasterDB + cmp.Initials), AutoCreateOption.None);
                    XpoDefault.Session = session;

                    List<Customer> customers = objectspace.GetObjects<Customer>().ToList();
                    string notinQuery = string.Empty;
                    foreach (Customer customer in customers)
                    {
                        if (notinQuery != "")
                        {
                            notinQuery += ",'" + customer.Code + "'";
                        }
                        else
                        {
                            notinQuery = "'" + customer.Code + "'";
                        }
                    }
                    string sqlquery = string.Empty;
                    if (notinQuery == "")
                        sqlquery = "Select Name, Address1, Custcode, credit_days, credit_lt from Customer ";
                    else
                        sqlquery = "Select Name, Address1, Custcode, credit_days, credit_lt from Customer where custcode not in (" + notinQuery + ")";
                    //Equivalent of SELECT * FROM TableName in SQL
                    // YourClassName would be your XPO object (your persistent object)
                    using (var uow = new UnitOfWork())
                    {
                        sd = uow.ExecuteQuery(sqlquery);
                    }

                }
            }
            catch (Exception ex) { }


            return sd;
        }

    }
}