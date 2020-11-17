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

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Location : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Location(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _LocationID = (Session.Evaluate<Location>(CriteriaOperator.Parse("Max(LocationID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Location>(CriteriaOperator.Parse("Max(LocationID)"), CriteriaOperator.Parse("")))) + 1;
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
        }

        [Persistent("LocationID")] // this line for read-only columns mapping
        private int _LocationID;
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]

        [PersistentAlias("_LocationID")] // This line for read-only column mapping
        public int LocationID
        {
            get { return _LocationID; }
        }

        string fName;
        [Size(50)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fERPCode;
        [Size(3)]
        public string ERPCode
        {
            get { return fERPCode; }
            set { SetPropertyValue<string>("ERPCode", ref fERPCode, value); }
        }

        [PersistentAlias("iif(IsNullOrEmpty(ERPCode), Name, concat(Name, ' (', ERPCode, ')'))")]
        public string LocationLookupDisplayText
        {
            get { return Convert.ToString(EvaluateAlias("LocationLookupDisplayText")); }
        }

        [Association]
        public XPCollection<Employee> Employees
        {
            get { return GetCollection<Employee>(nameof(Employees)); }
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

        [Association]
        public XPCollection<Warehouse> Warehouses
        {
            get { return GetCollection<Warehouse>(nameof(Warehouses)); }
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

    }
}