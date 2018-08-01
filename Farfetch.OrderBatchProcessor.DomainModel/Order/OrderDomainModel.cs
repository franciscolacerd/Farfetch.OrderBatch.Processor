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
    using Farfetch.OrderBatchProcessor.Dtos.Structs;
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
        public List<BoutiqueDto> CalculateBoutiquesOrdersCommissions(string path, decimal commissionPercentage)
        {
            ValidateIfCsvFile(path);

            ValidateIfCsvWasFound(path);

            return (from order in (from file in File.ReadAllLines(path)
                                   let lines = file.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                   select new OrderDto()
                                   {
                                       BoutiqueId = lines[OrderFormat.BoutiqueId],
                                       OrderId = lines[OrderFormat.OrderId],
                                       TotalOrderPrice = FormaterHelpers.ConvertFromStringToDecimal(lines[OrderFormat.TotalOrderPrice])
                                   })
                    group order by new { order.BoutiqueId } into boutiqueGrouping
                    let boutiqueComissions = boutiqueGrouping.Sum(x => x.TotalOrderPrice / commissionPercentage)
                    let boutiqueMaxComission = boutiqueGrouping.Max(x => x.TotalOrderPrice / commissionPercentage)
                    select new BoutiqueDto()
                    {
                        BoutiqueId = boutiqueGrouping.Key.BoutiqueId,
                        TotalOrdersCommission = boutiqueGrouping.Count() > 1 ? boutiqueComissions - boutiqueMaxComission : boutiqueComissions
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