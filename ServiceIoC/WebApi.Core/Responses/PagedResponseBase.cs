using WebApi.Core.DTO;

namespace WebApi.Core.Responses
{
    public abstract class PagedResponseBase<TResponseEntity> : 
        ResponseBase<PageResponseDto<TResponseEntity>>
    {
 
    }
}
