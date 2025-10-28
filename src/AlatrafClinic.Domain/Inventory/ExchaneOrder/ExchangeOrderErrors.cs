using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.ExchaneOrder
{
    public static class ExchangeOrderErrors
    {

        public static readonly Error InvalidType =
                  Error.Validation("Inventory.ExchangeOrder.InvalidType", "Exchange order type is invalid or missing.");

        public static readonly Error InvalidPatient =
            Error.Validation("Inventory.ExchangeOrder.InvalidPatient", "Patient ID must be greater than zero.");

        public static readonly Error InvalidStore =
            Error.Validation("Inventory.ExchangeOrder.InvalidStore", "Store ID must be greater than zero.");

        public static readonly Error AlreadyCompleted =
            Error.Validation("Inventory.ExchangeOrder.AlreadyCompleted", "Exchange order is already completed.");

        public static readonly Error CannotCancelCompleted =
            Error.Conflict("Inventory.ExchangeOrder.CannotCancelCompleted", "Completed exchange orders cannot be canceled.");

        public static readonly Error NotFound =
            Error.NotFound("Inventory.ExchangeOrder.NotFound", "Exchange order not found.");

        public static readonly Error CannotDelete =
            Error.Conflict("Inventory.ExchangeOrder.CannotDelete", "Exchange order cannot be deleted after processing.");

    }
}