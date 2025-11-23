using MongoDB.Bson;

namespace Shared.Exceptions
{
  public class NotFoundException : BusinessException
  {
    public NotFoundException(Type type, ObjectId id)
      : base(string.Format(MessageTemplate.NOT_FOUND, type.Name, id)) { }

    public NotFoundException(Type type, ObjectId id, Exception? innerException)
      : base(string.Format(MessageTemplate.NOT_FOUND, type.Name, id.ToString()), innerException) { }

    public NotFoundException(Type type, string id)
      : base(
        string.Format(MessageTemplate.NOT_FOUND, type.Name, SanitizerHelper.SanitizeAllHtml(id))
      ) { }
  }
}
