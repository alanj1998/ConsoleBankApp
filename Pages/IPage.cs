namespace SSD.Pages
{
    public class IPage
    {
        public Router _router;

        public IPage(Router r)
        {
            this._router = r;
        }
        public virtual void Render()
        {
            throw new System.Exception("Make sure to override this method!");
        }
    }
}