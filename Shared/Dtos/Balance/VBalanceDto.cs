using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManager.Shared.Dtos.Balance
{
    public class VBalanceDto
    {
        public int? ResourceId { get; set; }
        public string? ResourceName { get; set; }
        public int? ResourceStatus { get; set; }
        public int? MeasureId { get; set; }
        public string? MeasureName { get; set; }
        public int? MeasureStatus { get; set; }
        public decimal? BalanceQuantity { get; set; }
    }
}
