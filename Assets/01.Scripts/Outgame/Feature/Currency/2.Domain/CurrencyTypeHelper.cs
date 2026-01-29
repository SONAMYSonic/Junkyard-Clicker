using JunkyardClicker.Core;

public static class CurrencyTypeHelper
{
    public static ECurrencyType ToECurrencyType(PartType partType)
    {
        return partType switch
        {
            PartType.Scrap => ECurrencyType.Scrap,
            PartType.Glass => ECurrencyType.Glass,
            PartType.Plate => ECurrencyType.Plate,
            PartType.Rubber => ECurrencyType.Rubber,
            _ => ECurrencyType.Scrap
        };
    }
}
