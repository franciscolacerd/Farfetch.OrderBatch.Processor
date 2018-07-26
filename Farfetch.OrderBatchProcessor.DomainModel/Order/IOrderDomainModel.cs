// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderDomainModel.cs" company="">
//
// </copyright>
// <summary>
//   The OrderDomainModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Farfetch.OrderBatchProcessor.DomainModel.Order
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Dtos;

    /// <summary>
    /// The OrderDomainModel interface.
    /// </summary>
    public interface IOrderDomainModel
    {
        List<BoutiqueDto> CalculateBoutiquesOrdersCommissions(string path, decimal commissionPercentage);
    }
}