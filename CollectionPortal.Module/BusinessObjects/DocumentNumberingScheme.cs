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
using DevExpress.ExpressApp.ConditionalAppearance;

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class DocumentNumberingScheme : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DocumentNumberingScheme(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            //_DocumentNumberingSchemeID = (Session.Evaluate<DocumentNumberingScheme>(CriteriaOperator.Parse("Max(DocumentNumberingSchemeID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<DocumentNumberingScheme>(CriteriaOperator.Parse("Max(DocumentNumberingSchemeID)"), CriteriaOperator.Parse("")))) + 1;
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


        private Modules fModule;
        public Modules Module
        {
            get
            {
                return fModule;
            }
            set
            {
                SetPropertyValue(nameof(Module), ref fModule, value);
            }
        }

        private Location fLocation;
        public Location Location
        {
            get
            {
                return fLocation;
            }
            set
            {
                SetPropertyValue(nameof(Location), ref fLocation, value);
            }
        }

        private DocumentNumberingCategory fCategory;
        public DocumentNumberingCategory Category
        {
            get
            {
                return fCategory;
            }
            set
            {
                SetPropertyValue(nameof(Category), ref fCategory, value);
            }
        }

        private bool fAutomatic;
        public bool Automatic
        {
            get
            {
                return fAutomatic;
            }
            set
            {
                SetPropertyValue(nameof(Automatic), ref fAutomatic, value);
            }
        }

        private bool fDatewise;
        [Appearance("DatewiseAutoCond", Enabled = false, Criteria = "Automatic", Context = "DetailView")]
        //[Appearance("DatewiseAlphaCond", Enabled = false, Criteria = "Alpha", Context = "DetailView")]
        public bool Datewise
        {
            get
            {
                return fDatewise;
            }
            set
            {
                SetPropertyValue(nameof(Datewise), ref fDatewise, value);
            }
        }

        private bool fMonthwise;
        [Appearance("MonthwiseAutoCond", Enabled = false, Criteria = "Automatic", Context = "DetailView")]
        public bool Monthwise
        {
            get
            {
                return fMonthwise;
            }
            set
            {
                SetPropertyValue(nameof(Monthwise), ref fMonthwise, value);
            }
        }

        private bool fNumeric;
        [Appearance("NumericAutoCond", Enabled = false, Criteria = "Automatic", Context = "DetailView")]
        public bool Numeric
        {
            get
            {
                return fNumeric;
            }
            set
            {
                SetPropertyValue(nameof(Numeric), ref fNumeric, value);
            }
        }

        private bool fAlpha;
        [Appearance("AlphaAutoCond", Enabled = false, Criteria = "Automatic", Context = "DetailView")]
        public bool Alpha
        {
            get
            {
                return fAlpha;
            }
            set
            {
                SetPropertyValue(nameof(Alpha), ref fAlpha, value);
            }
        }

        private string fPrefix;
        public string Prefix
        {
            get
            {
                return fPrefix;
            }
            set
            {
                SetPropertyValue(nameof(Prefix), ref fPrefix, value);
            }
        }


        private string fSuffix;
        public string Suffix
        {
            get
            {
                return fSuffix;
            }
            set
            {
                SetPropertyValue(nameof(Suffix), ref fSuffix, value);
            }
        }

        private int fBody;
        public int Body
        {
            get
            {
                return fBody;
            }
            set
            {
                SetPropertyValue(nameof(Body), ref fBody, value);
            }
        }

        private bool fLeftFill;
        public bool LeftFill
        {
            get
            {
                return fLeftFill;
            }
            set
            {
                SetPropertyValue(nameof(LeftFill), ref fLeftFill, value);
            }
        }

        private string fFillCharacter;
        public string FillCharacter
        {
            get
            {
                return fFillCharacter;
            }
            set
            {
                SetPropertyValue(nameof(FillCharacter), ref fFillCharacter, value);
            }
        }


        private int fStartNo;
        public int StartNo
        {
            get
            {
                return fStartNo;
            }
            set
            {
                SetPropertyValue(nameof(StartNo), ref fStartNo, value);
            }
        }

        private int fEndNo;
        public int EndNo
        {
            get
            {
                return fEndNo;
            }
            set
            {
                SetPropertyValue(nameof(EndNo), ref fEndNo, value);
            }
        }

        private int fCurrentNo;
        public int CurrentNo
        {
            get
            {
                return fCurrentNo;
            }
            set
            {
                SetPropertyValue(nameof(CurrentNo), ref fCurrentNo, value);
            }
        }

        private DateTime fStartDate;
        public DateTime StartDate
        {
            get
            {
                return fStartDate;
            }
            set
            {
                SetPropertyValue(nameof(StartDate), ref fStartDate, value);
            }
        }

        private DateTime fEndDate;
        public DateTime EndDate
        {
            get
            {
                return fEndDate;
            }
            set
            {
                SetPropertyValue(nameof(EndDate), ref fEndDate, value);
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

    public enum Modules
    {
        SalesOrder,
        SalesInvoice,
        CustomerReceipt
    }
}