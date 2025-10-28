using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores.Items;
using AlatrafClinic.Domain.Inventory.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Stores
{
    public class StoreItems:AuditableEntity
    {
        public int StoreId { get; private set; }

        public int ItemId { get; private set; }

        public int QuantityPerSrore { get; private set; }
        public Units Units {  get; private set; }
        private StoreItem() { }

        private StoreItem(int storeId, int itemId, int quantityPerSrore, Units units)
        {
            StoreId = storeId;
            ItemId = itemId;
            QuantityPerSrore = quantityPerSrore;
            Units = units;
        }

        public static StoreItem Create(int storeId, int itemId, int quantityPerSrore,Units units)
        {
            if (store == null)
                return StoreItemsErrors.StoreRequired;
            if (item == null)
                return StoreItemsErrors.ItemRequired;
            if (quantity < 0)
                return StoreItemsErrors.InvalidQuantity;
            if (units is null)
                return UnitErrors.NameIsRequired;

            return new StoreItem(storeId, itemId, quantityPerSrore,units);
        }

        public Result AdjustQuantity(int amount)
        {
            if (amount == 0)
                return Result.Failure(ItemsErrors.InvalidAmount);

            if (Quantity + amount < 0)
                return Result.Failure(StoreItemsErrors.QuantityNotEnough);

            Quantity += amount;
            return Result.Success();
        }

    }
}
