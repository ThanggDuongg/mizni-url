namespace Application.Services.Interfaces
{
  public interface ICodeGenerator
  {
    string Encode(long id);
    long Decode(string code);
  }
}
