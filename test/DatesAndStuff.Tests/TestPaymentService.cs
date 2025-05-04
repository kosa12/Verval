using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatesAndStuff.Tests
{
    internal class TestPaymentService : IPaymentService
    {
        uint startCallCount = 0;
        uint specifyCallCount = 0;
        uint confirmCallCount = 0;
        double balance = 1000;
        
        public TestPaymentService(double initialBalance = 1000)
        {
            this.balance = initialBalance;
        }
        
        

        public void StartPayment()
        {
            if (startCallCount != 0 || specifyCallCount > 0 || confirmCallCount > 0)
                throw new Exception();

            startCallCount++;
        }

        public void SpecifyAmount(double amount)
        {
            if (startCallCount != 1 || specifyCallCount > 0 || confirmCallCount > 0)
                throw new Exception();
            
            if (amount >= balance)
                throw new Exception();

            specifyCallCount++;
            balance -= amount;
        }

        public void ConfirmPayment()
        {
            if (startCallCount != 1 || specifyCallCount != 1 || confirmCallCount > 0)
                throw new Exception();

            confirmCallCount++;
        }

        public bool SuccessFul()
        {
            return startCallCount == 1 && specifyCallCount == 1 && confirmCallCount == 1;
        }

        public double GetBalance()
        {
            return balance;
        }

        public void Cancel()
        {
            if (startCallCount != 1 || specifyCallCount > 0 || confirmCallCount > 0)
                throw new Exception("Cancel can only be called after StartPayment and before SpecifyAmount or ConfirmPayment.");

            startCallCount = 0;
        }

        public void Confirm()
        {
            if (startCallCount != 1 || specifyCallCount != 1 || confirmCallCount > 0)
                throw new Exception("ConfirmPayment must be called once after SpecifyAmount.");

            confirmCallCount++;
        }
    }
}
