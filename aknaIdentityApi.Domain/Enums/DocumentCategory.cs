namespace aknaIdentityApi.Domain.Enums
{
    public enum DocumentCategory
    {
        Unknown = 0,
        Identity = 1,        // TCKN, kimlik
        DriverLicense = 2,   // Ehliyet
        SRC = 3,
        ADR = 4,
        Tax = 5,             // Vergi levhası
        Transport = 6,       // Taşıma yetki belgesi (K1, K2, K3...)
        Contract = 7,        // Sözleşme
        Other = 99
    }
}
