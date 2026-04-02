namespace Demo.Common
{
    public class CalculationRequest
    {
        public int Number1 { get; set; }

        public int Number2 { get; set; }

        public OperationType Operation { get; set; }

        CalculationRequest() { }

        public CalculationRequest(int number1, int number2, OperationType operation)
        {
            this.Number1 = number1; 
            this.Number2 = number2; 
            this.Operation = operation;
        }

        public override string ToString()
        {
            return Number1 + (this.Operation == OperationType.Add ? "+" : "-") + Number2;
        }
    }
}
