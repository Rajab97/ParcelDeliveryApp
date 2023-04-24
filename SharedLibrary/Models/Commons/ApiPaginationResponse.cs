using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.Commons
{
    public class ApiPaginationResponse<TElement> : ApiResponse<IEnumerable<TElement>>
    {
        protected readonly PaginationFilterModel _filterModel;

        public int PageIndex { get; set; }         
        public int TotalPage { get; set; }             
        public int PageSize { get; set; }                 
        public int MaxPageSize { get; set; }             
        public int TotalRecord { get; set; } 


        public ApiPaginationResponse(int maxPageSize, PaginationFilterModel filterModel)
        {
            MaxPageSize = maxPageSize;
            _filterModel = filterModel;
        }

        public void Success<T>(IQueryable<T> elements, Func<T, TElement> mapping)
        {
            PageIndex = _filterModel.PageIndex;
            TotalRecord = elements.Count();
            PageSize = _filterModel.PageSize > MaxPageSize ? MaxPageSize : _filterModel.PageSize;
            TotalPage = (int)Math.Ceiling(TotalRecord / (double)PageSize);

            var elementList = elements
                               .Skip((PageIndex - 1) * PageSize)
                               .Take(PageSize)
                               .ToList();

            var mappedElements = new List<TElement>();
            foreach (var item in elementList ?? Enumerable.Empty<T>())
            {
                mappedElements.Add(mapping(item));
            }

            Success(mappedElements);
        }

        public void Success(IQueryable<TElement> elements)
        {
            PageIndex = _filterModel.PageIndex;
            TotalRecord = elements.Count();
            PageSize = _filterModel.PageSize > MaxPageSize ? MaxPageSize : _filterModel.PageSize;
            TotalPage = (int)Math.Ceiling(TotalRecord / (double)PageSize);

            var elementList = elements
                                .Skip((PageIndex - 1) * PageSize)
                                .Take(PageSize)
                                .ToList();

            Success(elementList);
        }
    }
}
