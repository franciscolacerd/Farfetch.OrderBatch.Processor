// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderDomainModel.cs" company="">
//
// </copyright>
// <summary>
//   The order domain model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Farfetch.OrderBatchProcessor.DomainModel.Order
{
    using Common;
    using Common.Exceptions;
    using Common.Helpers;
    using Dtos;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    ///https://www.c-sharpcorner.com/article/measure-your-code-using-code-metrics/
    /// <summary>
    /// The order domain model.
    /// </summary>
    public class OrderDomainModel : IOrderDomainModel
    {
        /// <summary>
        /// The calculate boutiques orders commissions.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="commissionPercentage">
        /// The commission percentage.
        /// </param>
        /// <returns>
        /// </returns>
        //TODO: remove max order
        public List<BoutiqueDto> CalculateBoutiquesOrdersCommissions(string path, decimal commissionPercentage)
        {
            ValidateIfCsvFile(path);

            ValidateIfCsvWasFound(path);

            return (from order in from file in File.ReadAllLines(path)
                                  let lines = file.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                  select new OrderDto()
                                  {
                                      BoutiqueId = lines[0],
                                      OrderId = lines[1],
                                      TotalOrderPrice = FormaterHelpers.ConvertFromStringToDecimal(lines[2])
                                  }
                    group order by new { order.BoutiqueId, order.TotalOrderPrice }
                            into grouping
                    select new BoutiqueDto()
                    {
                        BoutiqueId = grouping.Key.BoutiqueId,
                        TotalOrdersCommission = grouping.Sum(x => x.TotalOrderPrice / commissionPercentage)
                    }).ToList();
        }

        /// <summary>
        /// The is valid file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <exception cref="InvalidFileFormatException">
        /// </exception>
        private static void ValidateIfCsvFile(string path)
        {
            if (path.IndexOf("csv", System.StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidFileFormatException(Contants.Exceptions.MustBeCsv);
            }
        }

        /// <summary>
        /// The is csv not found.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <exception cref="CsvNotFoundException">
        /// </exception>
        private static void ValidateIfCsvWasFound(string path)
        {
            if (!File.Exists(path))
            {
                throw new CsvNotFoundException(Contants.Exceptions.FileNotFound);
            }
        }
    }
}