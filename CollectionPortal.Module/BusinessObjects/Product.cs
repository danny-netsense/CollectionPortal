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
    [DefaultProperty("Description")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Product : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Product(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _ProductID = (Session.Evaluate<Product>(CriteriaOperator.Parse("Max(ProductID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Product>(CriteriaOperator.Parse("Max(ProductID)"), CriteriaOperator.Parse("")))) + 1;
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
        [Persistent("ProductID")] // this line for read-only columns mapping
        private int _ProductID;
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [PersistentAlias("_ProductID")] // This line for read-only column mapping
        [Browsable(false)]
        public int ProductID
        {
            get { return _ProductID; }
        }

        string fCode;
        [Size(10)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        string fDescription;
        [Size(100)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>("Description", ref fDescription, value); }
        }

        private UOMSchema fUOMSchema;
        public UOMSchema UOMSchema
        {
            get
            {
                return fUOMSchema;
            }
            set
            {
                SetPropertyValue(nameof(UOMSchema), ref fUOMSchema, value);
            }
        }


        [PersistentAlias("iif(IsNullOrEmpty(Code), Description, concat(Description, ' (', Code, ')'))")]
        public string ProductLookupDisplayText
        {
            get { return Convert.ToString(EvaluateAlias("ProductLookupDisplayText")); }
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

        public static ImportDataDelegate<Product> CreateCoolProductImportDataFromXmlFileDelegate(XafApplication application)
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

                        Product product = os.CreateObject<Product>();
                        product.Code = result.Rows[index].Values[0].ToString();
                        product.Description = result.Rows[index].Values[1].ToString();
                        return product;
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

                    List<Product> products = objectspace.GetObjects<Product>().ToList();
                    string notinQuery = string.Empty;
                    foreach (Product product in products)
                    {
                        if (notinQuery != "")
                        {
                            notinQuery += ",'" + product.Code + "'";
                        }
                        else
                        {
                            notinQuery = "'" + product.Code + "'";
                        }
                    }
                    string sqlquery = string.Empty;
                    if (notinQuery == "")
                        sqlquery = "Select Prodcode, descript from Product ";
                    else
                        sqlquery = "Select Prodcode, descript from Product where prodcode not in (" + notinQuery + ")";
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