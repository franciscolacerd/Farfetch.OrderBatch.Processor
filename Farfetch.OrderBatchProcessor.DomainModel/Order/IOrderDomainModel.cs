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
        /// <summary>
        /// The get order lines from document async.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<string>> GetOrderLinesFromDocumentAsync(string path);

        /// <summary>
        /// The get orders.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <returns>
        /// </returns>
        List<OrderDto> GetOrders(List<string> lines);

        /// <summary>
        /// The calculate boutiques orders commissions.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <param name="commissionPercentage">
        /// The commission percentage.
        /// </param>
        /// <returns>
        /// </returns>
        List<BoutiqueDto> CalculateBoutiquesOrdersCommissions(List<OrderDto> orders, decimal commissionPercentage);
    }
}