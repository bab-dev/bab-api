using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Entities.Helpers
{
    public class SortHelper<T> : ISortHelper<T>
    {
        const string DESC = "DESC";
        const string ASC = "ASC";
        public IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
        {
            if (!entities.Any())
                return entities;

            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }

            var orderParams = orderByQueryString.Trim().Split(','); //get the individual fields

            //Check if the field received through the query string really exists in the <T> class
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var parts = param.Split(" ");
                var propertyFromQueryName = parts[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                //If we don’t find such a property we skip the step in the foreach loop and go to the next parameter in the list
                if (objectProperty == null)
                    continue;
               
                var direction = param.EndsWith(" desc") ? DESC : ASC;
                orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
            }

            if (orderQueryBuilder.Length > 0)
            {
                orderQueryBuilder.Remove(orderQueryBuilder.Length - 2, 2); //remove last ", " => same as TrimEnd(',', ' ')
            }

            return entities.OrderBy(orderQueryBuilder.ToString());
        }
    }
}
