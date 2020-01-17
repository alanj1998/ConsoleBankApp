namespace SSD.Pages
{
    internal class IPage
    {
        internal Router _router;

        internal IPage(Router r)
        {
            this._router = r;
        }
        internal virtual void Render()
        {
            throw new System.Exception("Make sure to override this method!");
        }
    }
}