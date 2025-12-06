using Application.Services.Interfaces;
using HashidsNet;
using Microsoft.Extensions.Options;

namespace Application.Services
{
  public class CodeGenerator : ICodeGenerator
  {
    private readonly Hashids _hashids;
    private readonly CodeGeneratorOptions _options;

    public CodeGenerator(IOptions<CodeGeneratorOptions> options)
    {
      _options = options.Value;
      _hashids = new Hashids(_options.Salt, _options.MinHashLength);
    }

    public string Encode(long id)
    {
      var obfuscated = (id + _options.Offset) ^ _options.XorKey;
      return _hashids.EncodeLong(obfuscated);
    }

    public long Decode(string code)
    {
      var numbers = _hashids.DecodeLong(code);
      if (numbers.Length == 0)
      {
        throw new InvalidCodeException("Invalid short code");
      }

      var deob = numbers[0];
      return (deob ^ _options.XorKey) - _options.Offset;
    }
  }
}
