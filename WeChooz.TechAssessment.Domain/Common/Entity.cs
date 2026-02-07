namespace WeChooz.TechAssessment.Domain.Common;

public abstract record Entity
{
    public Guid Id { get; set; }
    public virtual bool Equals(Entity? other) => other is not null && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}