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
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Xpo.DB;

namespace CollectionPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class SalesOrderDetail : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SalesOrderDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _SalesOrderDetailID = (Session.Evaluate<SalesOrderDetail>(CriteriaOperator.Parse("Max(SalesOrderDetailID)"), CriteriaOperator.Parse("")) == DBNull.Value ? 0 : Convert.ToInt32(Session.Evaluate<SalesOrderDetail>(CriteriaOperator.Parse("Max(SalesOrderDetailID)"), CriteriaOperator.Parse("")))) + 1;
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Persistent("SalesOrderDetailID")] // this line for read-only columns mapping
        private int _SalesOrderDetailID;
        //[RuleRequiredField] // Validation for Required
        //[RuleUniqueValue] // Validation for unique value
        [Browsable(false)]

        [PersistentAlias("_SalesOrderDetailID")] // This line for read-only column mapping
        public int SalesOrderDetailID
        {
            get { return _SalesOrderDetailID; }
        }


        private Product fProduct;
        [ImmediatePostData(true)]
        [RuleRequiredField] // Validation for Required
        [ModelDefault("LookupProperty", "ProductLookupDisplayText")]
        public Product Product
        {
            get
            {
                return fProduct;
            }
            set
            {
                SetPropertyValue(nameof(Product), ref fProduct, value);
                if (!IsLoading && !IsSaving && fProduct != null)
                {
                    ClearValues();
                    RefreshUnit1();
                    RefreshUnit2();
                    GetSalesRate();
                }
            }
        }

        private XPCollection<Unit> fAvailableUnit1;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<Unit> AvailableUnit1
        {
            get
            {
                if (fAvailableUnit1 == null)
                {
                    // Retrieve all Sample objects
                    fAvailableUnit1 = new XPCollection<Unit>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshUnit1();
                }
                // Return the filtered collection of Sample objects
                return fAvailableUnit1;
            }
        }

        public void RefreshUnit1()
        {
            if (fAvailableUnit1 == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Product == null)
            {
                // Show only Global Collection when the Party is not specified
                //fAvailableBookingOrders.Criteria = CriteriaOperator.Parse("1=1");
            }
            else
            {
                if (Product.UOMSchema != null)
                {
                    // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                    List<Unit> xunit = new List<Unit>();
                    xunit.Add(Product.UOMSchema.BaseUnit);
                    foreach (UOMSchemaDetail uod in Product.UOMSchema.UOMSchemaDetails)
                    {
                        if (!xunit.Contains(uod.Unit))
                        {
                            if (uod.Type == EntryType.Both || uod.Type == EntryType.Sales)
                                xunit.Add(uod.Unit);
                        }
                    }

                    string[] unitOids = new string[xunit.Count()];
                    int index = 0;
                    foreach (Unit ut in xunit)
                    {
                        unitOids[index] = ut.Oid.ToString();
                        index++;
                    }

                    fAvailableUnit1.Criteria = new InOperator("Oid", unitOids);
                    if (Unit1 == null)
                        Unit1 = Product.UOMSchema.BaseUnit;
                }
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source

        }

        private XPCollection<Unit> fAvailableUnit2;
        [Browsable(false)] // Prohibits showing the AvailableAccessories collection separately
        public XPCollection<Unit> AvailableUnit2
        {
            get
            {
                if (fAvailableUnit2 == null)
                {
                    // Retrieve all Sample objects
                    fAvailableUnit2 = new XPCollection<Unit>(Session);
                    // Filter the retrieved collection according to current conditions
                    RefreshUnit2();
                }
                // Return the filtered collection of Sample objects
                return fAvailableUnit2;
            }
        }

        public void RefreshUnit2()
        {
            if (fAvailableUnit2 == null)
                return;
            // Process the situation when the Party is not specified (see the Scenario 3 above)
            if (Product == null)
            {
                // Show only Global Collection when the Party is not specified
                //fAvailableBookingOrders.Criteria = CriteriaOperator.Parse("1=1");
            }
            else
            {
                if (Product.UOMSchema != null)
                {
                    // Leave only the current Party's Collection in the fAvailableSampleCollection collection
                    List<Unit> xunit = new List<Unit>();
                    xunit.Add(Product.UOMSchema.BaseUnit);
                    //foreach (UOMSchemaDetail uod in Product.UOMSchema.UOMSchemaDetails)
                    //{
                    //    if (!xunit.Contains(uod.Unit))
                    //    {
                    //        if (uod.Type == EntryType.Both || uod.Type == EntryType.Sales)
                    //            xunit.Add(uod.Unit);
                    //    }
                    //}

                    string[] unitOids = new string[xunit.Count()];
                    int index = 0;
                    foreach (Unit ut in xunit)
                    {
                        unitOids[index] = ut.Oid.ToString();
                        index++;
                    }

                    fAvailableUnit2.Criteria = new InOperator("Oid", unitOids);
                    Unit2 = Product.UOMSchema.BaseUnit;
                }
            }
            // Set null for the Collection property to allow an end-user 
            //to set a new value from the refreshed data source

        }

        private Warehouse fWarehouse;
        [Appearance("ProductWarehouseCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        [ModelDefault("LookupProperty", "WarehouseLookupDisplayText")]
        [DataSourceCriteria("[Locations][[Oid] = '@this.SalesOrder.Location.Oid']")]
        public Warehouse Warehouse
        {
            get
            {
                return fWarehouse;
            }
            set
            {
                SetPropertyValue(nameof(Warehouse), ref fWarehouse, value);
            }
        }

        decimal fQuantity;
        [RuleRequiredField] // Validation for Required
        [Appearance("ProductQuantityCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        [ImmediatePostData(true)]
        public decimal Quantity
        {
            get { return fQuantity; }
            set
            {
                SetPropertyValue<decimal>("Quantity", ref fQuantity, value);
                SetQuantity2();
                CalculateAmount();
            }
        }

        Unit fUnit1;
        [RuleRequiredField] // Validation for Required
        [DataSourceProperty(nameof(AvailableUnit1))]
        [Appearance("ProductUnit1Cond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView", Priority = 2)]
        [ImmediatePostData(true)]
        public Unit Unit1
        {
            get
            {
                return fUnit1;
            }
            set { SetPropertyValue<Unit>("Unit1", ref fUnit1, value);
                SetQuantity2();
                CalculateAmount();
            }
        }

        decimal fConvFac;
        [Browsable(false)]
        public decimal ConvFac
        {
            get { return fConvFac; }
            set { SetPropertyValue<decimal>("ConvFac", ref fConvFac, value); }
        }

        decimal fQuantity2;
        [Appearance("ProductQuantity2Cond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        [ImmediatePostData(true)]
        public decimal Quantity2
        {
            get { return fQuantity2; }
            set
            {
                SetPropertyValue<decimal>("Quantity2", ref fQuantity2, value);
            }
        }

        Unit fUnit2;
        [DataSourceProperty(nameof(AvailableUnit2))]
        [Appearance("ProductUnit2Cond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        public Unit Unit2
        {
            get { return fUnit2; }
            set { SetPropertyValue<Unit>("Unit2", ref fUnit2, value); }
        }

        decimal fPrice;
        [Appearance("ProductPriceCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        [ImmediatePostData(true)]
        public decimal Price
        {
            get { return fPrice; }
            set
            {
                SetPropertyValue<decimal>("Price", ref fPrice, value);
                CalculateAmount();
            }
        }

        decimal fProductAmount;
        [Appearance("ProductProductAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        public decimal ProductAmount
        {
            get { return fProductAmount; }
            set { SetPropertyValue<decimal>("ProductAmount", ref fProductAmount, value); }
        }

        decimal fDiscountPercentageOrAmount;
        [Appearance("ProductDiscountPercentageOrAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        [ImmediatePostData(true)]
        public decimal DiscountPercentageOrAmount
        {
            get { return fDiscountPercentageOrAmount; }
            set
            {
                SetPropertyValue<decimal>("DiscountPercentageOrAmount", ref fDiscountPercentageOrAmount, value);
                CalculateAmount();
            }
        }

        DiscountType fDiscountType;
        [Appearance("ProductDiscountTypeCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        [ImmediatePostData(true)]
        public DiscountType DiscountType
        {
            get { return fDiscountType; }
            set
            {
                SetPropertyValue<DiscountType>("DiscountType", ref fDiscountType, value);
                CalculateAmount();
            }
        }


        decimal fDiscountAmount;
        [Appearance("ProductDiscountAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        public decimal DiscountAmount
        {
            get { return fDiscountAmount; }
            set {

                SetPropertyValue<decimal>("DiscountAmount", ref fDiscountAmount, value); }
        }

        decimal fGrossAmount;
        [Appearance("ProductGrossAmountCond", Enabled = false, Criteria = "IsNullOrEmpty(Product)", Context = "DetailView")]
        public decimal GrossAmount
        {
            get { return fGrossAmount; }
            set {
                bool modified = SetPropertyValue(nameof(GrossAmount), ref fGrossAmount, value);
                if (!IsLoading && !IsSaving && SalesOrder != null && modified)
                {
                    SalesOrder.UpdateTotalAmount(true);
                }
            }
        }

        private SalesOrder _SalesOrder;
        [Association]
        public SalesOrder SalesOrder
        {
            get { return _SalesOrder; }
            set { SetPropertyValue<SalesOrder>(nameof(SalesOrder), ref _SalesOrder, value); }
        }

        private void CalculateAmount()
        {

            ProductAmount = Math.Round((Quantity * Price), 2);

            switch (DiscountType)
            {
                case DiscountType.Amount:
                    DiscountAmount = fDiscountPercentageOrAmount * -1;
                    break;
                case DiscountType.Percentage:
                    DiscountAmount = Math.Round(((ProductAmount / 100) * DiscountPercentageOrAmount), 2) * -1;
                    break;
            }

            GrossAmount = ProductAmount + DiscountAmount;
        }

        private void SetQuantity2()
        {
            ConvFac = 1;
            Quantity2 = Quantity;
            Unit2 = Unit1;
            try
            {
                if (Unit1 == null)
                    return;
                if (Product == null)
                    return;
                if (Product.UOMSchema == null)
                    return;
                if (Product.UOMSchema.BaseUnit == null)
                    return;

                if (Unit1 != null)
                {
                    if (Unit1 == Product.UOMSchema.BaseUnit)
                    {
                        ConvFac = 1;
                        Quantity2 = Quantity;
                        Unit2 = Unit1;
                    }
                    else
                    {
                        Unit2 = Product.UOMSchema.BaseUnit;
                        XPCollection<UOMSchemaDetail> getRecords = new XPCollection<UOMSchemaDetail>(Session);
                        //getRecords.Criteria = CriteriaOperator.Parse("[Unit]= ? and (Type=" + EntryType.Both + " or Type=" + EntryType.Sales + ")", Unit1);
                        getRecords.Criteria = CriteriaOperator.Parse("[Unit]= ? and UOMSchema = ? and (Type=? or Type=?)", Unit1, Product.UOMSchema, EntryType.Both, EntryType.Sales);
                        foreach (var item in getRecords)
                        {
                            if (item.Quantity > 0)
                            {
                                ConvFac = item.BaseQuantity / item.Quantity;
                                Quantity2 = Quantity * ConvFac;
                            }
                        }

                    }
                }
            }
            catch (Exception ex) { }
        }

        private void GetSalesRate()
        {
            try
            {
                string sqlquery = "";
                sqlquery = " select top 1 price from billdet where prodcode='" + Product.Code + "' and Custcode='" + SalesOrder.Customer.ERPCode + "' order by bill_date desc";
                //Initialize your data layer. 
                //By default if you don't do this, XPO will try and use an access databse (jet)
                Session session = new Session();
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(SalesOrder.Company.ServerName,  SalesOrder.Company.ERPMasterDB + SalesOrder.Company.Initials), AutoCreateOption.None);
                //XpoDefault.DataLayer = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString("ACC00", SalesOrder.Company.ERPMasterDB + SalesOrder.Company.Initials), AutoCreateOption.None);
                XpoDefault.Session = session;

                //Equivalent of SELECT * FROM TableName in SQL
                // YourClassName would be your XPO object (your persistent object)
                using (var uow = new UnitOfWork())
                {
                    SelectedData sd = uow.ExecuteQuery(sqlquery);


                    foreach (var item in sd.ResultSet)
                    {
                        Price = Convert.ToDecimal(item.Rows[0].Values[0]);
                    }
                }

                
            }
            catch (Exception ex) { }
        }

        private void ClearValues()
        {
            Warehouse = null;
            Quantity = 0;
            Unit1 = null;
            Quantity2 = 0;
            Unit2 = null;
            Price = 0;
            ProductAmount = 0;
            DiscountPercentageOrAmount = 0;
            DiscountAmount = 0;
            GrossAmount = 0;
        }
    }

    public enum DiscountType
    {
        Percentage,
        Amount
    }
}