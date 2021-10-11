using System.Collections.Generic;
using System.Linq;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Helper
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Get the entities from the clustered pagination of the list
        /// </summary>
        /// <param name="entities">the list of entities</param>
        /// <param name="pageSize">the size of the page</param>
        /// <param name="pageNumber">the page number to get the entities, start from 1</param>
        /// <param name="totalEntity">the amount of entities in the list</param>
        /// <typeparam name="TEntity">the entity</typeparam>
        /// <returns>the list of entities from the paginated list</returns>
        public static IEnumerable<TEntity> GetPage<TEntity>(this IEnumerable<TEntity> entities, int pageSize, int pageNumber, out int totalEntity)
        {
            totalEntity = entities.Count();
            return entities
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
        }

        /// <summary>
        /// Get the entities from the clustered pagination of the list
        /// </summary>
        /// <param name="entities">the list of entities</param>
        /// <param name="pageSize">the size of the page</param>
        /// <param name="pageNumber">the page number to get the entities, start from 1</param>
        /// <typeparam name="TEntity">the entity</typeparam>
        /// <returns>the list of entities from the paginated list</returns>
        public static IEnumerable<TEntity> GetPage<TEntity>(this IEnumerable<TEntity> entities, int pageSize, int pageNumber)
        {
            return GetPage<TEntity>(entities, pageSize, pageNumber, out _);
        }

        /// <summary>
        /// Get the entities from the clustered pagination of the list
        /// </summary>
        /// <param name="entities">the list of entities</param>
        /// <param name="parameter">the page parameter</param>
        /// <param name="totalEntity">the amount of entities in the list</param>
        /// <typeparam name="TEntity">the entity</typeparam>
        /// <returns>the list of entities from the paginated list</returns>
        public static IEnumerable<TEntity> GetPage<TEntity>(this IEnumerable<TEntity> entities, PaginationParameter parameter, out int totalEntity)
        {
            return GetPage<TEntity>(entities, parameter.PageSize, parameter.PageNumber, out totalEntity);
        }

        /// <summary>
        /// Get the entities from the clustered pagination of the list
        /// </summary>
        /// <param name="entities">the list of entities</param>
        /// <param name="parameter">the page parameter</param>
        /// <typeparam name="TEntity">the entity</typeparam>
        /// <returns>the list of entities from the paginated list</returns>
        public static IEnumerable<TEntity> GetPage<TEntity>(this IEnumerable<TEntity> entities, PaginationParameter parameter)
        {
            return GetPage<TEntity>(entities, parameter.PageSize, parameter.PageNumber);
        }
    }
}