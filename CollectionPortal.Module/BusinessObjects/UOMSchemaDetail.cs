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

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class UOMSchemaDetail : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public UOMSchemaDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        EntryType fType;
        public EntryType Type
        {
            get { return fType; }
            set { SetPropertyValue<EntryType>("Type", ref fType, value); }
        }

        Unit fBaseUnit;
        [DataSourceCriteria("Oid == '@This.UOMSchema.BaseUnit.Oid' ")]
        public Unit BaseUnit
        {
            get { return fBaseUnit; }
            set { SetPropertyValue<Unit>("BaseUnit", ref fBaseUnit, value); }
        }

        decimal fBaseQuantity;
        public decimal BaseQuantity
        {
            get { return fBaseQuantity; }
            set { SetPropertyValue<decimal>("BaseQuantity", ref fBaseQuantity, value); }
        }

        Unit fUnit;
        [Association]
        public Unit Unit
        {
            get { return fUnit; }
            set { SetPropertyValue<Unit>("Unit", ref fUnit, value); }
        }

        decimal fQuantity;
        public decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<decimal>("Quantity", ref fQuantity, value); }
        }

        private UOMSchema _UOMSchema;
        [Association]
        public UOMSchema UOMSchema
        {
            get { return _UOMSchema; }
            set { SetPropertyValue<UOMSchema>(nameof(UOMSchema), ref _UOMSchema, value); }
        }
    }

    public enum EntryType
    {
        Both,
        Purchase,
        Sales
    }
}