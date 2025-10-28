using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.ExchaneOrder
{
    public class ExchangeOreder
    {
        public int ExchangeOrederId { get; private set; }
        public enExchangeOrder Type { get; private set; }
        public int PaitentId { get; private set; }
        public int StoreId { get; private set; }

        private ExchangeOreder()
        {

        }
        private ExchangeOreder(enExchangeOrder type, int paitentId, int storeId)
        {
            Type = type;
            PaitentId = paitentId;
            StoreId = storeId;
        }
        public static Result<ExchangeOrder> Create(enExchangeOrder type, int patientId, int storeId)
        {
            if (type == enExchangeOrder.None)
                return ExchangeOrderErrors.InvalidType;

            if (patientId <= 0)
                return ExchangeOrderErrors.InvalidPatient;

            if (storeId <= 0)
                return ExchangeOrderErrors.InvalidStore;

            return new ExchangeOrder(type, patientId, storeId);
        }

        public Result Complete()
        {
            if (IsCompleted)
                return ExchangeOrderErrors.AlreadyCompleted;

            IsCompleted = true;
            return Result.Success();
        }

        public Result Cancel()
        {
            if (IsCompleted)
                return ExchangeOrderErrors.CannotCancelCompleted;

            return Result.Success();
        }

    }
}
