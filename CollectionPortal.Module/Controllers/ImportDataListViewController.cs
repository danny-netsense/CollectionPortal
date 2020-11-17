using CollectionPortal.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using ImportData;

namespace CollectionPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ImportDataListViewController : ViewController
    {
        public const int MaxImportedRecordsCount = 10;
        public const string ActiveKeyImportAction = "ActiveKeyImportAction";
        //1st Change
        public const string ActiveKeyImportActionItemRootListViewForAgent = "Item is active only in the root ListView for Agent";
        public const string ActiveKeyImportActionItemRootListViewForCustomer = "Item is active only in the root ListView for Customer";
        public const string ActiveKeyImportActionItemRootListViewForProduct = "Item is active only in the root ListView for Product";
        private SingleChoiceAction importDataActionCore = null;

        public ImportDataListViewController()
        {
            TargetViewType = ViewType.ListView;
            importDataActionCore = CreateImportAction();
        }
        private SingleChoiceAction CreateImportAction()
        {
            SingleChoiceAction importDataAction = new SingleChoiceAction(this, "ImportData", PredefinedCategory.RecordEdit);
            importDataAction.Execute += importDataAction_Execute;
            //4th Change
            ChoiceActionItem item1 = new ChoiceActionItem();
            ChoiceActionItem item2 = new ChoiceActionItem();
            ChoiceActionItem item3 = new ChoiceActionItem();
            importDataAction.Caption = "Import Data";
            importDataAction.ImageName = "Attention";
            //5th Change
            item1.Caption = "Agent";
            item1.Data = "Agent_ListView";
            item2.Caption = "Customer ListView";
            item2.Data = "Customer_ListView";
            item3.Caption = "Product ListView";
            item3.Data = "Product_ListView";
            //6th Change
            importDataAction.Items.Add(item1);
            importDataAction.Items.Add(item2);
            importDataAction.Items.Add(item3);
            importDataAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importDataAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            return importDataAction;
        }
        protected virtual void ImportData(SingleChoiceActionExecuteEventArgs e)
        {

            ChoiceActionItem activeItem = e.SelectedChoiceActionItem;
            ListView lv = (ListView)View;
            ImportDataManager importManager = new ImportDataManager(Application);
            switch (activeItem.Data.ToString())
            {
                //7th Change
                case "Agent_ListView":
                    importManager.ImportData<Agent>(MaxImportedRecordsCount, Agent.CreateCoolAgentImportDataFromXmlFileDelegate(Application), null, lv, true);
                    //Dennis: This line won't be executed unless you handle the exception thrown from the previus ImportData call.
                    //ImportDataManager.ImportData<Person>(MaxImportedRecordsCount, ImportDataLogic.CreateDummyPersonImportDataDelegate(), ImportDataLogic.CreateDummyPersonValidateDataDelegate(), lv, true);
                    break;
                case "Customer_ListView":
                    //PropertyCollectionSource pcs = lv.CollectionSource as PropertyCollectionSource;
                    //if (pcs != null)
                    //{
                    importManager.ImportData<Customer>(MaxImportedRecordsCount, Customer.CreateCoolCustomerImportDataFromXmlFileDelegate(Application), null, lv, true);
                    //}
                    break;
                case "Product_ListView":
                    importManager.ImportData<Product>(MaxImportedRecordsCount, Product.CreateCoolProductImportDataFromXmlFileDelegate(Application), null, lv, true);
                    //Dennis: This line won't be executed unless you handle the exception thrown from the previus ImportData call.
                    //ImportDataManager.ImportData<Person>(MaxImportedRecordsCount, ImportDataLogic.CreateDummyPersonImportDataDelegate(), ImportDataLogic.CreateDummyPersonValidateDataDelegate(), lv, true);
                    break;

            }
        }
        private void importDataAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            ImportData(e);
        }
        protected override void UpdateActionActivity(ActionBase action)
        {
            base.UpdateActionActivity(action);
            //2nd Change
            bool rootListViewForPersonCondition = View.IsRoot && View is ListView && View.ObjectTypeInfo.Type == typeof(Agent);
            //bool nestedListViewForPhoneNumberCondition = !View.IsRoot && View is ListView && View.ObjectTypeInfo.Type == typeof(Customer);
            bool nestedListViewForPhoneNumberCondition = View.IsRoot && View is ListView && View.ObjectTypeInfo.Type == typeof(Customer);
            bool rootListViewForProductCondition = View.IsRoot && View is ListView && View.ObjectTypeInfo.Type == typeof(Product);
            //3rd Change
            ImportDataAction.Active[ActiveKeyImportAction] = rootListViewForPersonCondition || nestedListViewForPhoneNumberCondition || rootListViewForProductCondition;
            ImportDataAction.Items[0].Active[ActiveKeyImportActionItemRootListViewForAgent] = rootListViewForPersonCondition;
            ImportDataAction.Items[1].Active[ActiveKeyImportActionItemRootListViewForCustomer] = nestedListViewForPhoneNumberCondition;
            ImportDataAction.Items[2].Active[ActiveKeyImportActionItemRootListViewForProduct] = rootListViewForProductCondition;
            
        }
        public SingleChoiceAction ImportDataAction
        {
            get
            {
                return importDataActionCore;
            }
        }
    }
}
