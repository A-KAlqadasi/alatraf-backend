using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Stores.Items
{
    public class Items : AudiableEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public Unit Unit { get; private set; }
        public int Quantity { get; private set; }
        public int MinQuantity { get; private set; }
        public decimal MinPriceToPay { get; private set; }
        public decimal MaxPriceToPay { get; private set; }
        public bool IsActive { get; private set; }

        private Items() { }

        private Items(string name, decimal price, Unit unit, int quantity, int minQuantity, decimal minPriceToPay, decimal maxPriceToPay, bool isActive)
        {
            Name = name;
            Price = price;
            Unit = unit;
            Quantity = quantity;
            MinQuantity = minQuantity;
            MinPriceToPay = minPriceToPay;
            MaxPriceToPay = maxPriceToPay;
            IsActive = isActive;
        }
        public static Created<Items> Created(string name, decimal price, Unit unit, int quantity, int minQuantity, decimal minPriceToPay, decimal maxPriceToPay, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ItemsErrors.NameRequired;

            if (unit is null)
                return ItemsErrors.UnitRequired;

            if (price <= 0)
                return ItemsErrors.InvalidPrice;

            if (quantity < 0)
                return ItemsErrors.InvalidQuantity;

            if (minQuantity < 0)
                return ItemsErrors.InvalidMinQuantity;

            if (minPriceToPay > maxPriceToPay)
                return ItemsErrors.InvalidPriceRange;

            return new Items(name, price, unit, quantity, minQuantity, minPriceToPay, maxPriceToPay, isActive);

        }

        public static Updated<Items> Updated(string name, decimal price, Unit unit, int quantity, int minQuantity, decimal minPriceToPay, decimal maxPriceToPay, bool isActive)
        {

            if (string.IsNullOrWhiteSpace(name))
                return ItemsErrors.NameRequired;

            if (unit is null)
                return ItemsErrors.UnitRequired;

            if (price <= 0)
                return ItemsErrors.InvalidPrice;

            if (quantity < 0)
                return ItemsErrors.InvalidQuantity;

            if (minQuantity < 0)
                return ItemsErrors.InvalidMinQuantity;

            if (minPriceToPay > maxPriceToPay)
                return ItemsErrors.InvalidPriceRange;
            Name = name;
            Price = price;
            Unit = unit;
            Quantity = quantity;
            MinQuantity = minQuantity;
            MinPriceToPay = minPriceToPay;
            MaxPriceToPay = maxPriceToPay;
            IsActive = isActive;

            return Result.Updated;
        }
        public Result DecreaseQuantity(int amount)
        {
            if (amount <= 0)
                return ItemsErrors.InvalidAmount;

            if (Quantity < amount)
                return ItemsErrors.QuantityNotEnough;

            Quantity -= amount;
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return ItemsErrors.ItemAlreadyActive;
            IsActive = true;
            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return ItemsErrors.ItemNotActive;

            IsActive = false;
            return Result.Success();
        }

    }
}
