using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Stores
{
    public class Stores:AuditableEntity
    {
        public int StoreId { get;private set; }
        public string Name { get;private set; }
        public bool IsActive { get; private set; }

        private readonly List<StoreItem> _storeItems = new();
        public IReadOnlyCollection<StoreItem> StoreItems => _storeItems.AsReadOnly();

        private Stores()
        {

        }
        private Stores(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public static Result<Created> Created(string name,bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
                return StoresErrors.NameRequired;
            return new Stores(name,isActive);

        }
        public Result Activate()
        {
            if (IsActive)
                return StoresErrors.StoreAlreadyActive;

            IsActive = true;
            return Result.Success();
        }
        public Result Deactivate()
        {
            if (!IsActive)
                return StoresErrors.StoreNotActive;

            IsActive = false;
            return Result.Success();
        }

        public Result AddItem(Item item, int quantity, decimal localPrice)
        {
            if (!IsActive)
                return StoresErrors.StoreNotActive;

            if (item == null)
                return StoreItemsErrors.ItemRequired;

            if (quantity < 0)
                return StoreItemsErrors.InvalidQuantity;

            if (localPrice < 0)
                return StoreItemsErrors.InvalidPrice;

            if (_storeItems.Any(si => si.Item.Id == item.Id))
                return StoresErrors.ItemAlreadyExists;

            var storeItem = StoreItem.Create(this, item, quantity, localPrice);
            _storeItems.Add(storeItem);

            return Result.Success();
        }

    }
}
