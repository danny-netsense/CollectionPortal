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
    public class Company : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Company(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(nameof(Name), ref name, value);
            }
        }

        private string fServerName;
        public string ServerName
        {
            get { return fServerName; }
            set
            {
                SetPropertyValue(nameof(ServerName), ref fServerName, value);
            }
        }

        private string fERPMasterDB;
        public string ERPMasterDB
        {
            get { return fERPMasterDB; }
            set
            {
                SetPropertyValue(nameof(ERPMasterDB), ref fERPMasterDB, value);
            }
        }

        private string fInitials;
        public string Initials
        {
            get { return fInitials; }
            set
            {
                SetPropertyValue(nameof(Initials), ref fInitials, value);
            }
        }

        [Association("Company-Employees")]
        public XPCollection<Employee> Employees
        {
            get { return GetCollection<Employee>(nameof(Employees)); }
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