namespace myoddweb.classifier.core
{
  public class CategorizeResponse
  {
    public int CategoryId { get; }

    public bool WasMagnetUsed { get; }

    public CategorizeResponse(int categoryId, bool wasMagnetUsed)
    {
      CategoryId = categoryId;
      WasMagnetUsed = wasMagnetUsed;
    }

  }
}
