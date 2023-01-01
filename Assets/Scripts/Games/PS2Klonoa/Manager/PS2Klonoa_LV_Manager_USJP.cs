using BinarySerializer.Klonoa.LV;

namespace Ray1Map.PS2Klonoa
{
    public class PS2Klonoa_LV_Manager_USJP : PS2Klonoa_LV_BaseManager
    {
        public override KlonoaSettings_LV GetKlonoaSettings(GameSettings settings) => new KlonoaSettings_LV_US();
    }
}