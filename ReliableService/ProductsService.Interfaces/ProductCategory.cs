using System.Runtime.Serialization;

namespace ProductsService.Interfaces
{
    [DataContract]
    public enum ProductCategory
    {
        [EnumMember]
        Home = 100,
        [EnumMember]
        KitchenAndDining = 101,
        [EnumMember]
        Furniture = 102,
        [EnumMember]
        BeddingAndBath = 103,
        [EnumMember]
        Appliances = 104,
        [EnumMember]
        PatioAndGarden = 105,
        [EnumMember]
        FineArt = 106,
        [EnumMember]
        ArtsCraftsAndSewing = 106,
        [EnumMember]
        PetSupplies = 107,
        [EnumMember]
        WeddingRegistry = 108,
        [EnumMember]
        HomeImprovement = 109,
        [EnumMember]
        PowerAndHandTools = 110,
        [EnumMember]
        LampsAndLightFixtures = 111,
        [EnumMember]
        KitchenAndBathFixtures = 112,
        [EnumMember]
        Hardware = 113,
        [EnumMember]
        SmartHome = 114,


        [EnumMember]
        AllBeauty = 200,
        [EnumMember]
        LuxuryBeauty = 201,
        [EnumMember]
        SalonAndSpa = 202,
        [EnumMember]
        MenGrooming = 203,
        [EnumMember]
        HealthAndBabyCare = 204,
        [EnumMember]
        VitaminsAndDietarySupplements = 205,
        [EnumMember]
        GroceryAndGourmetFood = 206,
        [EnumMember]
        SpecialtyDiets = 207,
        [EnumMember]
        Wine = 208,
        [EnumMember]
        SubscribeAndSave = 209,
        [EnumMember]
        PrimePantry = 210,
        [EnumMember]
        Coupons = 211
    }
}