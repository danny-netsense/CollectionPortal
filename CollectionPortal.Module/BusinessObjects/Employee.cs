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
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [DefaultClassOptions, DefaultProperty(nameof(UserName))]
    public class Employee : PermissionPolicyUser
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Employee(Session session) : base(session) { }
        private Company fCompany;
        [Association("Company-Employees")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue(nameof(Company), ref fCompany, value); }
        }

        Agent agent = null;
        public Agent Agent
        {
            get { return agent; }
            set { SetPropertyValue<Agent>("Agent", ref agent, value); }
            //set
            //{
            //    if (agent == value)
            //        return;

            //    // Store a reference to the person's former house.
            //    Agent prevAgent = agent;
            //    agent = value;

            //    if (IsLoading) return;

            //    // Remove a reference to the house's owner, if the person is its owner.
            //    if (prevAgent != null && prevAgent.Employee == this)
            //        prevAgent.Employee= null;

            //    // Specify the person as a new owner of the house.
            //    if (agent != null)
            //        agent.Employee = this;

            //    //OnChanged(nameof(Agent));
            //}
        }

        [Association]
        public XPCollection<Location> Locations
        {
            get { return GetCollection<Location>(nameof(Locations)); }
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