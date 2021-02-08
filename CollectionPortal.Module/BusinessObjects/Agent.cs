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
using System.Xml;
using DevExpress.Xpo.DB;

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).

    [RuleIsReferenced("SalesOrder", DefaultContexts.Delete, typeof(SalesOrder), "Agent", InvertResult = true, MessageTemplateMustBeReferenced = "Linked to Sales Order, Deletion is not allowed!")]
    public class Agent : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Agent(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _AgentID = (Session.Evaluate<Agent>(CriteriaOperator.Parse("Max(AgentID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<Agent>(CriteriaOperator.Parse("Max(AgentID)"), CriteriaOperator.Parse("")))) + 1;
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

        [Persistent("AgentID")] // this line for read-only columns mapping
        private int _AgentID;
        [RuleRequiredField] // Validation for Required
        [RuleUniqueValue] // Validation for unique value
        [PersistentAlias("_AgentID")] // This line for read-only column mapping
        [Browsable(false)]
        public int AgentID
        {
            get { return _AgentID; }
        }

        string fCode;
        [Size(10)]
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        string fName;
        [Size(50)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }

        string fAddress;
        [Size(500)]
        public string Address
        {
            get { return fAddress; }
            set { SetPropertyValue<string>("Address", ref fAddress, value); }
        }

        private Employee fEmployee;
        public Employee Employee
        {
            get
            {
                return fEmployee;
            }
            //set
            //{
            //    if (fEmployee == value)
            //        return;

            //    // Store a reference to the former owner.
            //    Employee prevEmployee = fEmployee;
            //    fEmployee = value;

            //    if (IsLoading) return;

            //    // Remove an owner's reference to this building, if exists.
            //    if (prevEmployee != null && prevEmployee.Agent == this)
            //        prevEmployee.Agent = null;

            //    // Specify that the building is a new owner's house.
            //    if (fEmployee != null)
            //        fEmployee.Agent = this;
            //    //OnChanged(nameof(Employee));
            //}
            set { SetPropertyValue<Employee>("Employee", ref fEmployee, value); }
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
        public XPCollection<Customer> Customers
        {
            get { return GetCollection<Customer>(nameof(Customers)); }
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


        public static ImportDataDelegate<Agent> CreateCoolAgentImportDataFromXmlFileDelegate(XafApplication application)
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

                        Agent agent = os.CreateObject<Agent>();
                        agent.Name = result.Rows[index].Values[0].ToString();
                        agent.Address = result.Rows[index].Values[1].ToString();
                        agent.Code = result.Rows[index].Values[2].ToString();
                        return agent;
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

                    List<Agent> agents = objectspace.GetObjects<Agent>().ToList();
                    string notinQuery = string.Empty;
                    foreach (Agent agent in agents)
                    {
                        if (notinQuery != "")
                        {
                            notinQuery += ",'" + agent.Code + "'";
                        }
                        else
                        {
                            notinQuery = "'" + agent.Code + "'";
                        }
                    }
                    string sqlquery = string.Empty;
                    if (notinQuery == "")
                        sqlquery = "Select Name, Address1, Agentcode from Agent ";
                    else
                        sqlquery = "Select Name, Address1, Agentcode from Agent where agentcode not in (" + notinQuery + ")";
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