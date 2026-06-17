namespace MackySoft.SerializeReferenceExtensions.Editor
{
    public static class TypeSearchService
    {

        public static readonly IIntrinsicTypePolicy IntrinsicTypePolicy;
        public static readonly ITypeCompatibilityPolicy TypeCompatibilityPolicy;
        public static readonly ITypeCandiateProvider TypeCandiateProvider;
        public static readonly TypeCandiateService TypeCandiateService;

        static TypeSearchService ()
        {
            IntrinsicTypePolicy = DefaultIntrinsicTypePolicy.Instance;

            TypeCompatibilityPolicy = Unity2023OrNewerGVTCompatibilityPolicy.Instance;
            TypeCandiateProvider = Unity_2023_2_OrNewer_TypeCandiateProvider.Instance;


            TypeCandiateService = new TypeCandiateService(TypeCandiateProvider, IntrinsicTypePolicy, TypeCompatibilityPolicy);
        }
    }
}