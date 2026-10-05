using Box2D.NET;

namespace model.Models.Enums
{
    public enum BodyTypeEnum
    {
        DynamicBody = B2BodyType.b2_dynamicBody,
        StaticBody = B2BodyType.b2_staticBody,
        KinematicBody = B2BodyType.b2_kinematicBody
    }
}