﻿using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models.Commons;

namespace UserManagementService.Application.Common.Models
{
    public class EFApiPaginationResponse<TElement> : ApiPaginationResponse<TElement>
    {
        public EFApiPaginationResponse(int maxPageSize, PaginationFilterModel filterModel) : base(maxPageSize, filterModel) { }
        public async Task SuccessAsync<T>(IQueryable<T> elements, Func<T, TElement> mapping)
        {
            PageIndex = _filterModel.PageIndex;
            TotalRecord = await elements.CountAsync();
            PageSize = _filterModel.PageSize > MaxPageSize ? MaxPageSize : _filterModel.PageSize;
            TotalPage = (int)Math.Ceiling(TotalRecord / (double)PageSize);

            var elementList = await elements
                               .Skip((PageIndex - 1) * PageSize)
                               .Take(PageSize)
                               .ToListAsync();

            var mappedElements = new List<TElement>();
            foreach (var item in elementList ?? Enumerable.Empty<T>())
            {
                mappedElements.Add(mapping(item));
            }

            Success(mappedElements);
        }

        public async Task SuccessAsync(IQueryable<TElement> elements)
        {
            PageIndex = _filterModel.PageIndex;
            TotalRecord = await elements.CountAsync();
            PageSize = _filterModel.PageSize > MaxPageSize ? MaxPageSize : _filterModel.PageSize;
            TotalPage = (int)Math.Ceiling(TotalRecord / (double)PageSize);

            var elementList = await elements
                                .Skip((PageIndex - 1) * PageSize)
                                .Take(PageSize)
                                .ToListAsync();

            Success(elementList);
        }
    }
}
