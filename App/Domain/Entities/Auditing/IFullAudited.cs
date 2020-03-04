

namespace App.Domain.Entities.Auditing
{
    public interface IFullAudited : ICreationAudited, ICreationTime, IDeletionAudited, IDeletionTime
    {
    }

    public interface IFullAudited<TUser> : ICreationAudited<TUser>, ICreationAudited, ICreationTime, IDeletionAudited<TUser>, IDeletionAudited, IDeletionTime where TUser : IEntity<long>
    {
    }
}
