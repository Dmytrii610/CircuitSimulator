namespace CircuitCore.Model.Components.Base
{
    public class ElementNode
    {
        public string Label { get; set; }
        public bool IsGround { get; set; }
        public double Voltage { get; set; }
        public ElementNode(string label, bool isGround = false)
        {
            Label = label;
            IsGround = isGround;
        }
    }
}
