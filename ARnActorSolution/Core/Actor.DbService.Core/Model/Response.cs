using Actor.Base;

namespace Actor.DbService.Core.Model
{
    public class Response
    {
        public Response(IndexRouter router, IActor asker, IQuery query)
        {
            Router = router;
            Asker = asker;
            Query = query;
        }
        public IndexRouter Router { get; private set; }
        public IActor Asker { get; private set; }
        public IQuery Query { get; private set; }
    }
}
