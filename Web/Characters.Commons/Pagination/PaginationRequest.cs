namespace Characters.Commons.Pagination;

public record PaginationRequest(int PageIndex = 0, int PageSize = 20);
