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
    [DefaultProperty("Code")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class UOMSchema : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public UOMSchema(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _UOMSchemaID = (Session.Evaluate<UOMSchema>(CriteriaOperator.Parse("Max(UOMSchemaID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<UOMSchema>(CriteriaOperator.Parse("Max(UOMSchemaID)"), CriteriaOperator.Parse("")))) + 1;
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
        [Persistent("UOMSchemaID")] // this line for read-only columns mapping
        private int _UOMSchemaID;
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [Browsable(false)]

        [PersistentAlias("_UOMSchemaID")] // This line for read-only column mapping
        public int UOMSchemaID
        {
            get { return _UOMSchemaID; }
        }

        string fCode;
        [Size(10)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        Unit fBaseUnit;
        [VisibleInLookupListView(true)]
        [Custom("LookupProperty", "UnitLookup")]
        [Custom("IsVisibleInLookupListView", "True")]
        [Association]
        public Unit BaseUnit
        {
            get
            {
                return fBaseUnit;
            }
            set
            {
                SetPropertyValue<Unit>("BaseUnit", ref fBaseUnit, value);
                if (!IsSaving)
                {
                    RefreshUOMDetails(true);
                }
            }
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

        [DevExpress.Xpo.Aggregated, Association]
        public XPCollection<UOMSchemaDetail> UOMSchemaDetails
        {
            get
            {

               return GetCollection<UOMSchemaDetail>(nameof(UOMSchemaDetails));
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

        private void RefreshUOMDetails(bool forceChangeEvents)
        {
            try
            {
                Unit oldBaseUnit = fBaseUnit;
                if (UOMSchemaDetails.Count == 0)
                {
                    if (fBaseUnit != null)
                    {
                        UOMSchemaDetail detail = new UOMSchemaDetail(Session);
                        detail.Unit = fBaseUnit;
                        detail.Type = EntryType.Both;
                        detail.BaseUnit = fBaseUnit;
                        detail.Type = EntryType.Both;
                        detail.Quantity = 1;
                        detail.BaseQuantity = 1;
                        detail.UOMSchema = this;
                        //UOMSchemaDetails.Add(detail);
                    }
                }
            }
            catch
            { }

        }

    }
}